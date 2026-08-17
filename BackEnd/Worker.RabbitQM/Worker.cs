using Worker.RabbitQM.Services;

namespace Worker.RabbitQM;

public class RabbitMqWorker(
    ILogger<RabbitMqWorker> logger,
    IServiceScopeFactory serviceScopeFactory
    ) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IPedidosPublishServices>();

            await service.PublicarPedidosAsync(stoppingToken);
            
            await service.PublicarPedidosProcesadoAsync(stoppingToken);

            logger.LogInformation("Procesando pedidos.");

            await Task.Delay(5000, stoppingToken);
        }
    }
}
