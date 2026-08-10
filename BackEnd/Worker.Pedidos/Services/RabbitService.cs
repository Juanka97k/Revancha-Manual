using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using Shared.Events;
using Worker.Pedidos.Mapper;
using Worker.Pedidos.Repos;

namespace Worker.Pedidos.Services
{
    public interface IRabbitService
    {
        Task ProcesarPedidoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken);
    }

    public class RabbitService : IRabbitService
    {
        private readonly IRabbitRepository _rabbitRepository;

        private readonly ILogger<RabbitService> _logger;
        public RabbitService(IRabbitRepository rabbitRepository, ILogger<RabbitService> logger)
        {
            _rabbitRepository = rabbitRepository;
            _logger = logger;
        }

        public async Task ProcesarPedidoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {
            try
            {
                var PedidoProcesado = await _rabbitRepository.ValidarPedidoProcesado(pedido, cancellationToken);

                if(PedidoProcesado)
                {
                    _logger.LogInformation("El pedido con EventId {EventId} ya ha sido procesado previamente.", pedido.EventoId);
                    return;
                }

                var pedidoExistente = await _rabbitRepository.BuscarPedidoAsync(pedido, cancellationToken);
                var stockExistente = await _rabbitRepository.BuscarStockAsync(pedido, cancellationToken);

                if (pedidoExistente == null)
                {
                    _logger.LogWarning("El pedido con EventId {EventId} no existe en la base de datos.", pedido.EventoId);
                    return;
                }

                if (stockExistente != null && stockExistente.Cantidad >= pedido.Cantidad)
                {
                    // Lógica para procesar el pedido y actualizar el stock
                    stockExistente.Cantidad -= pedido.Cantidad;
                    pedidoExistente.Estado = Status.Confirmada;
                    _logger.LogInformation("Pedido con EventId {EventId} procesado correctamente. Stock actualizado.", pedido.EventoId);
                }
                else
                {
                    pedidoExistente.Estado = Status.Rechazada;
                    _logger.LogInformation("Pedido con EventId {EventId} rechazado debido a stock insuficiente.", pedido.EventoId);
                }

                var pedidoProcesadoEvent = PedidosMapper.PedidoProcesadoEvent(pedido);

                _rabbitRepository.GuardarPedidoProcesadoAsync(pedidoProcesadoEvent, cancellationToken);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el pedido con EventId {EventId}.", pedido.EventoId);
                throw;
            }
        }
    }
}