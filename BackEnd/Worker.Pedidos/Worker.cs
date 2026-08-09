using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;

namespace Worker.Pedidos;

public class PedidosWorker: BackgroundService
{
    private readonly ILogger<PedidosWorker> _logger;
    private readonly IRabbitConfig _rabbitConfig;
    
    public PedidosWorker( 
        ILogger<PedidosWorker> logger,
        IRabbitConfig rabbitConfig
    )
    {
        _logger = logger;
        _rabbitConfig = rabbitConfig;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var (connection, channel) =await _rabbitConfig.CrearConexionAsync(stoppingToken);

        if (stoppingToken.IsCancellationRequested) return;

        await _rabbitConfig.DeclararColaAsync(channel,stoppingToken);

        await _rabbitConfig.ConfiguracionDeProcesamientoAsync(channel, stoppingToken);

        var consumidor = new AsyncEventingBasicConsumer(channel);

        consumidor.ReceivedAsync += async (model, ea) =>
        {
            await ProcesarMensajeAsync(
                channel,
                ea,
                stoppingToken);
        };

        await _rabbitConfig.ConsumirColaAsync(channel,consumidor,stoppingToken);

        _logger.LogInformation("Worker escuchando la cola RabbitMQ.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcesarMensajeAsync(IChannel channel,BasicDeliverEventArgs ea,CancellationToken cancellationToken)
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

                // Crear un Scope fresco para resolver los servicios Scoped (DbContext)
                // using var scope = _scopeFactory.CreateScope();
                // var processor = scope.ServiceProvider.GetRequiredService<IInventoryProcessor>();

                // await processor.ProcessOrderCreatedAsync(evento, stoppingToken);
            }
            // Confirmación exitosa 
            //await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
            await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
         }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando el mensaje de RabbitMQ. Se enviará Nack para reintento.");
            await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
        }
    }
}
