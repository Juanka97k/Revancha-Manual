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
        Task PublicarPedidoProcesadoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken);
        Task InicializarConexionAsync(CancellationToken cancellationToken);
    }

    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly ILogger<RabbitPublisher> _logger;
        private readonly IConfiguration _configuration;

        private IConnection? _connection;
        private IChannel? _channelPedidos;
        private IChannel? _channelProcesados;

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

            var properties = new BasicProperties
            {
                Persistent = true
            };
            await PublicarPedidoAsync(_channelPedidos, pedido, properties, cancellationToken); 
        }

        public async Task PublicarPedidoProcesadoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {

            var properties = new BasicProperties
            {
                Persistent = true
            };
            await PublicarPedidoProcesadoAsync(_channelProcesados, pedido, properties, cancellationToken); 
        }

        public async Task InicializarConexionAsync(CancellationToken cancellationToken)
        {
            if (_connection is not null &&
                _connection.IsOpen &&
                _channelPedidos is not null &&
                _channelPedidos.IsOpen &&
                _channelProcesados is not null &&
                _channelProcesados.IsOpen)
            {
                return;
            }

            var factory = CrearConnectionFactory();

            _connection = await factory.CreateConnectionAsync(
                cancellationToken);

            _channelPedidos = await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            
            _channelProcesados = await _connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

            await DeclararColaPedidosAsync(
                _channelPedidos,
                cancellationToken);

            await DeclararColaProcesadosAsync(
                _channelProcesados,
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

        private async Task DeclararColaPedidosAsync(IChannel channel,CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                queue: _configuration["RabbitMQ:QueuePedidos"] ?? "Pedido-created-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
        }

        private async Task DeclararColaProcesadosAsync(IChannel channel,CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                queue: _configuration["RabbitMQ:QueueProcesados"] ?? "pedido-processed-queue",
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
                    routingKey: _configuration["RabbitMQ:QueuePedidos"] ?? "Pedido-created-queue",
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Pedido publicado en RabbitMQ: {PedidoId}",
                    pedido.PedidoId);
        }

        private async Task PublicarPedidoProcesadoAsync(
            IChannel channel,
            PedidoCreateEvent pedido,
            BasicProperties properties,
            CancellationToken cancellationToken)
        {
            var message = JsonSerializer.Serialize(pedido);

            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _configuration["RabbitMQ:QueueProcesados"] ?? "pedido-processed-queue",
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "Pedido procesado publicado en RabbitMQ: {PedidoId}",
                    pedido.PedidoId);
        }
    }
}