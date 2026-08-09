using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using RabbitMQ.Client;
using Shared.Events;
using Worker.RabbitQM.Repos;

namespace Worker.RabbitQM.Services
{
    public interface IRabbitPublisher
    {
        Task PublicarPedidosAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken);
    }

    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly ILogger<RabbitPublisher> _logger;
        private readonly IConfiguration _configuration;

        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitPublisher(
            ILogger<RabbitPublisher> logger,
            //abbitRepository rabbitRepository,
            IConfiguration configuration
        )
        {
            _logger = logger;
           //rabbitRepository = rabbitRepository;
            _configuration = configuration;
        }

        public async Task PublicarPedidosAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {
            await InicializarConexionAsync(cancellationToken);

            var properties = new BasicProperties
            {
                Persistent = true
            };
            await PublicarPedidoAsync(_channel, pedido, properties, cancellationToken); 
        }

        private async Task InicializarConexionAsync(CancellationToken cancellationToken)
        {
            if (_connection is not null &&
                _connection.IsOpen &&
                _channel is not null &&
                _channel.IsOpen)
            {
                return;
            }

            var factory = CrearConnectionFactory();

            _connection = await factory.CreateConnectionAsync(
                cancellationToken);

            _channel = await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            await DeclararColaAsync(
                _channel,
                cancellationToken);

            _logger.LogInformation(
                "Conexión establecida con RabbitMQ.");
        }
        private ConnectionFactory CrearConnectionFactory()
        {
            return new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };
        }

        private async Task DeclararColaAsync(IChannel channel,CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                queue: _configuration["RabbitMQ:QueueName"] ?? "pedidos_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
        }

        private async Task PublicarPedidoAsync(
            IChannel channel,
            PedidoCreateEvent pedido,
            BasicProperties properties,
            CancellationToken cancellationToken)
        {
            var message = JsonSerializer.Serialize(pedido);

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _configuration["RabbitMQ:QueueName"] ?? "pedidos_queue",
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Pedido publicado en RabbitMQ: {PedidoId}",
                    pedido.PedidoId);
        }
    }
}