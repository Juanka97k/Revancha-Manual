using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using Shared.Dtos;
using Shared.Events;
using Worker.RabbitQM.Mapper;
using Worker.RabbitQM.Repos;

namespace Worker.RabbitQM.Services
{
    public interface IRabbitServices
    {
        Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken);
        Task<List<PedidoCreateEvent>> GenerarColaPedidos(List<PedidoColaDto> pedidos);
        Task PrcoesarPedidosAsync(CancellationToken cancellationToken);
    }

    public class RabbitServices : IRabbitServices
    {
        private readonly ILogger<RabbitServices> _logger;
        private readonly IRabbitRepository _rabbitRepository;
        private readonly IRabbitPublisher _rabbitPublisher;

        public RabbitServices(ILogger<RabbitServices> logger, IRabbitRepository rabbitRepository, IRabbitPublisher rabbitPublisher)
        {
            _logger = logger;
            _rabbitRepository = rabbitRepository;
            _rabbitPublisher = rabbitPublisher;
        }

        public async Task PrcoesarPedidosAsync(CancellationToken cancellationToken) 
        {
            try
            {
                var pedidos = await _rabbitRepository.BuscarPedidosSinProcesarAsync(cancellationToken);

                if (pedidos.Count == 0)
                {
                    _logger.LogInformation("No hay pedidos sin procesar.");
                    return;
                }

                var pedidosCompletos = await _rabbitRepository.BuscarPedidosSinProcesarAsync(pedidos, cancellationToken);

                var eventos = WRabbitMapper.MapearPedidosAEventos(pedidosCompletos);

                foreach (var evento in eventos)
                {
                    try
                    {
                        await _rabbitPublisher.PublicarPedidosAsync(evento, cancellationToken);
                        await _rabbitRepository.ActualizarEstadoPedidoAsync(evento, EstadosProcesamiento.Publicado, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error procesando pedido {PedidoId}",
                            evento.PedidoId);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar pedidos.");
                throw;
            }
        }

        public async Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pedidos = await _rabbitRepository.BuscarPedidosSinProcesarAsync(cancellationToken);
                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar pedidos sin procesar.");
                throw;
            }
        }

        public async Task<List<PedidoCreateEvent>> GenerarColaPedidos(List<PedidoColaDto> pedidos)
        {
            try
            {
                var pedidosCompletos = await _rabbitRepository.BuscarPedidosSinProcesarAsync(pedidos, CancellationToken.None);

                var eventos = WRabbitMapper.MapearPedidosAEventos(pedidosCompletos);

                return eventos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar pedidos.");
                throw;
            }
        }


    }
}