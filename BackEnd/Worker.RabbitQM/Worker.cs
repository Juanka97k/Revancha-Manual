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

            var service = scope.ServiceProvider.GetRequiredService<IRabbitServices>();

            await service.PrcoesarPedidosAsync(stoppingToken);

            // var pedidos = await service.BuscarPedidosSinProcesarAsync(stoppingToken);

            // if (pedidos.Count > 0)
            // {
            //     var eventos = await service.GenerarColaPedidos(pedidos);

            //     await publisher.PublicarPedidosAsync(eventos, stoppingToken);
            // }
            // else
            // {

            // }

            logger.LogInformation("Procesando pedidos.");

            await Task.Delay(5000, stoppingToken);
        }
    }
}
