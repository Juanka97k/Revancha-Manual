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

                var pedidosCompletos = await _rabbitRepository.BuscarPedidosAsync(pedidos, cancellationToken);

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

    }
}