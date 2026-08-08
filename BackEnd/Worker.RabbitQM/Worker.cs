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
            // logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            using var scope = serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IRabbitServices>();

            var pedidos = await service.BuscarPedidosSinProcesarAsync(stoppingToken);
            if (pedidos.Count > 0)
            {
                logger.LogInformation("Pedidos sin procesar encontrados: {count}", pedidos.Count);
                foreach (var pedido in pedidos)
                {
                    logger.LogInformation("PedidoId: {pedidoId}, Estado: {estado}, CreadoEn: {creadoEn}",
                        pedido.PedidoId, pedido.Estado, pedido.CreadoEn);
                }
            }
            else
            {
                logger.LogInformation("No se encontraron pedidos sin procesar.");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}
