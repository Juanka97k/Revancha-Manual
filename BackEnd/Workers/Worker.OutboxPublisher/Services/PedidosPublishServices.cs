using System.Text.Json;
using Pedidos.Infraestructura.Entities;
using Shared.Events;
using Worker.RabbitQM.Repos;

namespace Worker.RabbitQM.Services
{
    public interface IPedidosPublishServices
    {
        Task PublicarPedidosAsync(CancellationToken cancellationToken);
        Task PublicarPedidosProcesadoAsync(CancellationToken cancellationToken) ;
    }

    public class PedidosPublishServices : IPedidosPublishServices
    {
        private readonly ILogger<PedidosPublishServices> _logger;
        private readonly IPedidosPublishRepository _pedidosRepository;
        private readonly IRabbitConfigs _rabbitConfigs;

        public PedidosPublishServices(ILogger<PedidosPublishServices> logger, IPedidosPublishRepository pedidosRepository, IRabbitConfigs rabbitPublisher)
        {
            _logger = logger;
            _pedidosRepository = pedidosRepository;
            _rabbitConfigs = rabbitPublisher;
        }

        public async Task PublicarPedidosAsync(CancellationToken cancellationToken) 
        {
            try
            {
                var pedidos = await _pedidosRepository.BuscarPedidosSinProcesarAsync(cancellationToken);

                if (pedidos.Count == 0)
                {
                    _logger.LogInformation("No hay pedidos sin procesar.");
                    return;
                }

                await _rabbitConfigs.InicializarConexionAsync(cancellationToken);

                foreach (var pedidoPublicar in pedidos)
                {
                    try
                    {
                        var evento = JsonSerializer.Deserialize<PedidoCreateEvent>(pedidoPublicar.PayLoad);

                        if (evento == null) continue;

                        await _rabbitConfigs.PublicarPedidosAsync(evento, cancellationToken);

                        await _pedidosRepository.ActualizarEstadoOutboxAsync(pedidoPublicar.OutboxId, EstadoOutbox.Publicado, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error procesando pedido {PedidoId}",
                            pedidoPublicar.PedidoId);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar pedidos.");
                throw;
            }
        }

        public async Task PublicarPedidosProcesadoAsync(CancellationToken cancellationToken) 
        {
            _logger.LogInformation("Ejecuto PublicarPedidosProcesadoAsync");
            try
            {
                var yaProcesados = await _pedidosRepository.BuscarPedidosYaProcesadosAsync(cancellationToken);

                if (yaProcesados.Count == 0)
                {
                    _logger.LogInformation("No hay pedidos sin procesar.");
                    return;
                }

                await _rabbitConfigs.InicializarConexionAsync(cancellationToken);

                foreach (var procesado in yaProcesados)
                {
                    try
                    {
                        var evento = JsonSerializer.Deserialize<PedidoProcesadoEvent>(procesado.PayLoad);

                        if (evento == null) continue;

                        await _rabbitConfigs.PublicarPedidoProcesadoAsync(evento, cancellationToken);
                        await _pedidosRepository.ActualizarEstadoOutboxAsync(procesado.OutboxId, EstadoOutbox.Publicado, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error procesando pedido {PedidoId}",
                            procesado.PedidoId);
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