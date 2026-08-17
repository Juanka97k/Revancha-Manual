using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker.Pedidos
{
    public interface IRabbitConfig
    {
        Task InicializarConexionAsync(CancellationToken cancellationToken);
        // Task<(IConnection Connection, IChannel Channel)> CrearConexionAsync(CancellationToken cancellationToken);
        Task DeclararColaPedidosAsync(IChannel channel,CancellationToken cancellationToken);
        Task ConfiguracionDeProcesamientoAsync(IChannel channel, CancellationToken cancellationToken);
        Task ConsumirColaPedidosAsync(AsyncEventingBasicConsumer consumidor, CancellationToken cancellationToken);
        AsyncEventingBasicConsumer DeclararConsumidor();
        Task ExitosoProcesamientoPedidoAsync(BasicDeliverEventArgs ea,CancellationToken cancellationToken);
        Task FalloProcesamientoPedidoAsync(BasicDeliverEventArgs ea,CancellationToken cancellationToken);
        
    }

    public class RabbitConfig : IRabbitConfig
    {
        private readonly ILogger<RabbitConfig> _logger;
        private readonly IConfiguration _configuration;

        private IConnection?    _connection;
        private IChannel?       _channelPedidos;
        private IChannel?       _channelProcesados;


        public RabbitConfig(
            ILogger<RabbitConfig> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;
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

            await ConfiguracionDeProcesamientoAsync(_channelPedidos, cancellationToken);

            // await DeclararColaProcesadosAsync(
            //     _channelProcesados,
            //     cancellationToken);

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

        public async Task DeclararColaPedidosAsync(IChannel channel,CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                queue: _configuration["RabbitMQ:QueuePedidos"] ?? "Pedido-created-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );
        }

        public async Task ConfiguracionDeProcesamientoAsync(IChannel channel, CancellationToken cancellationToken)
        {
            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken);
        }

        public async Task ConsumirColaPedidosAsync(AsyncEventingBasicConsumer consumidor, CancellationToken cancellationToken)
        {
            await _channelPedidos.BasicConsumeAsync(
                queue: _configuration["RabbitMQ:QueuePedidos"] ?? "Pedido-created-queue",
                autoAck: false, 
                consumer: consumidor,
                cancellationToken: cancellationToken
                );
        }

        public AsyncEventingBasicConsumer DeclararConsumidor()
        {
            return new AsyncEventingBasicConsumer(_channelPedidos);
        }

        public async Task ExitosoProcesamientoPedidoAsync(BasicDeliverEventArgs ea,CancellationToken cancellationToken)
        {
            await _channelPedidos.BasicAckAsync(
                deliveryTag: ea.DeliveryTag, 
                multiple: false, 
                cancellationToken: cancellationToken);
        }

        public async Task FalloProcesamientoPedidoAsync(BasicDeliverEventArgs ea,CancellationToken cancellationToken)
        {
            await _channelPedidos.BasicNackAsync(
                deliveryTag: ea.DeliveryTag, 
                multiple: false, 
                requeue: true, 
                cancellationToken: cancellationToken);
        }

    }
}