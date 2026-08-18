using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;
using Worker.Pedidos.Services;

namespace Worker.Pedidos;

public class PedidosWorker: BackgroundService
{
    private readonly ILogger<PedidosWorker> _logger;
    private readonly IRabbitConfig _rabbitConfig;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public PedidosWorker( 
        ILogger<PedidosWorker> logger,
        IRabbitConfig rabbitConfig,
        IServiceScopeFactory scopeFactory
    )
    {
        _logger = logger;
        _rabbitConfig = rabbitConfig;
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        //var (connection, channel) =await _rabbitConfig.CrearConexionAsync(stoppingToken);

        await _rabbitConfig.InicializarConexionAsync(stoppingToken);

        if (stoppingToken.IsCancellationRequested) return;

        //await _rabbitConfig.DeclararColaPedidosAsync(channel,stoppingToken);

        var consumidor =  _rabbitConfig.DeclararConsumidor();

        consumidor.ReceivedAsync += async (model, ea) =>
        {
            await ProcesarMensajeAsync(
                ea,
                stoppingToken);
        };

        await _rabbitConfig.ConsumirColaPedidosAsync(consumidor,stoppingToken);

        _logger.LogInformation("Worker escuchando la cola RabbitMQ.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(2000, stoppingToken);
        }
    }

    private async Task ProcesarMensajeAsync(BasicDeliverEventArgs ea,CancellationToken cancellationToken)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
             var evento = JsonSerializer.Deserialize<PedidoCreateEvent>(message);

            if (evento != null)
            {
                _logger.LogInformation("Evento recibido desde RabbitMQ: EventId {EventId}",
                 evento.EventoId);

                // Crear un Scope fresco para resolver los servicios Scoped 
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IProcesarPedidoService>();

                 await processor.ProcesarPedidoAsync(evento, cancellationToken);
            }
            // Confirmación exitosa 
            //await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            //await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
            await _rabbitConfig.ExitosoProcesamientoPedidoAsync(ea,cancellationToken);
         }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando el mensaje de RabbitMQ. Se enviará Nack para reintento.");
            //await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
            await _rabbitConfig.FalloProcesamientoPedidoAsync(ea,cancellationToken);
        }
    }
}
