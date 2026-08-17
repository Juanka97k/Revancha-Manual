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
    public interface IProcesarPedidoService
    {
        Task ProcesarPedidoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken);
    }

    public class ProcesarPedidoService : IProcesarPedidoService
    {
        private readonly IPedidoRepository _pedidosRepository;

        private readonly ILogger<ProcesarPedidoService> _logger;
        public ProcesarPedidoService(IPedidoRepository pedidosRepository, ILogger<ProcesarPedidoService> logger)
        {
            _pedidosRepository = pedidosRepository;
            _logger = logger;
        }

        

        public async Task ProcesarPedidoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {
            try
            {
                var pedidoProcesado = await _pedidosRepository.ValidarPedidoProcesado(pedido.EventoId, cancellationToken);
            
                if(pedidoProcesado)
                {
                    var colaProcesada = await _pedidosRepository.ValidarColaPedidoProcesado(pedido.EventoId,cancellationToken);

                    if(!colaProcesada)
                    {
                        await MarcarProcesadoOutbox(pedido.EventoId,cancellationToken);
                    }
                    _logger.LogInformation("El pedido con EventId {EventId} ya ha sido procesado previamente.", pedido.EventoId);

                    return;
                }

                var pedidoExistente = await _pedidosRepository.BuscarPedidoAsync(pedido.PedidoId, cancellationToken);
                

                if (pedidoExistente == null)
                {
                    _logger.LogWarning("El pedido con EventId {EventId} no existe en la base de datos.", pedido.EventoId);
                    await MarcarProcesadoOutbox(pedido.EventoId,cancellationToken);
                    return;
                }

                var stockExistente = await _pedidosRepository.BuscarStockAsync(pedido.Sku, cancellationToken);

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

                var datoCola = PedidosMapper.PedidoProcesadoEventMapper(pedidoExistente);
                var evento = PedidosMapper.PedidoProcesadoColaMapper(datoCola);

                var yaProcesado = PedidosMapper.PedidoProcesadoMapper(pedido);

                

                _pedidosRepository.GuardarPedidoProcesado(yaProcesado);
                _pedidosRepository.GuardarColaPedido(evento);

                await _pedidosRepository.GuardarCambiosAsync(cancellationToken);

                await MarcarProcesadoOutbox(pedido.EventoId,cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el pedido con EventId {EventId}.", pedido.EventoId);
                throw;
            }
        }

        private async Task MarcarProcesadoOutbox(Guid EventoId,CancellationToken cancellationToken)
        {
            await _pedidosRepository.MarcarEstadoOutboxAsync(EventoId,EstadoOutbox.Procesado,cancellationToken);
        }
    }
}