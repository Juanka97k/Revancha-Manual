using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pedidos.Api.Hubs;
using Pedidos.Aplicacion.Interfaces;
using Pedidos.Infraestructura.configs;
using RabbitMQ.Client.Events;
using Shared.Events;

namespace Pedidos.Api.BackgroundServices
{
    public class PredidosProcesadosConsumerServices : BackgroundService
    {
        private readonly IRabbitConfig _rabbitConfig;
        private readonly ILogger<PredidosProcesadosConsumerServices> _logger;

        private readonly IHubContext<PedidosHub, IPedidosClient> _hubContext;


        public PredidosProcesadosConsumerServices(
            IRabbitConfig rabbitConfig,
            ILogger<PredidosProcesadosConsumerServices> logger,
            IHubContext<PedidosHub, IPedidosClient> hubContext
            )
        {
            _rabbitConfig = rabbitConfig;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitConfig.InicializarConexionAsync(stoppingToken);

            if (stoppingToken.IsCancellationRequested) return;

            var consumidor = _rabbitConfig.DeclararConsumidor();

            consumidor.ReceivedAsync += async (model, ea) =>
            {
                await ProcesarMensajeAsync(
                    ea,
                    stoppingToken);
            };

            await _rabbitConfig.ConsumirColaPedidosProcesadosAsync(consumidor, stoppingToken);

            _logger.LogInformation("Backgrond escuchando la cola RabbitMQ.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000, stoppingToken);
            }
        }

        private async Task ProcesarMensajeAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var evento = JsonSerializer.Deserialize<PedidoProcesadoEvent>(message);

                if (evento != null)
                {
                    _logger.LogInformation("Pedido recibido desde RabbitMQ: PedidoId {PedidoId}",
                    evento.PedidoId);

                    //Proceso del hub


                    // 🚀 Transmitir a TODOS los clientes conectados a SignalR (Postman, Bruno, Angular, etc.)
                    await _hubContext.Clients.All.RecibirEstadoPedidoActualizado(
                        pedidoId: evento.PedidoId,
                        estado: evento.Estado.ToString(),
                        mensaje: $"El pedido {evento.PedidoId} fue procesado con estado: {evento.Estado}"
                    );
                    _logger.LogInformation("⚡ Evento SignalR transmitido a todos los clientes.");

                }
                // Confirmación exitosa 
                await _rabbitConfig.ExitosoProcesamientoPedidoAsync(ea, cancellationToken);
                //await _rabbitConfig.FalloProcesamientoPedidoAsync(ea,cancellationToken);

                //await Task.Delay(10000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando el mensaje de RabbitMQ. Se enviará Nack para reintento.");
                await _rabbitConfig.FalloProcesamientoPedidoAsync(ea, cancellationToken);
            }
        }
    }
}