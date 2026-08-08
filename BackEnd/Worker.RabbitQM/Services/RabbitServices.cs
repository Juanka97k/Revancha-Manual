using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Events;
using Worker.RabbitQM.Mapper;
using Worker.RabbitQM.Repos;

namespace Worker.RabbitQM.Services
{
    public interface IRabbitServices
    {
        Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken);
        Task<List<PedidoCreateEvent>> GenerarColaPedidos(List<PedidoColaDto> pedidos);
    }

        public class RabbitServices : IRabbitServices
    {
        private readonly ILogger<RabbitServices> _logger;
        private readonly IRabbitRepository _rabbitRepository;
        public RabbitServices(ILogger<RabbitServices> logger, IRabbitRepository rabbitRepository)
        {
            _logger = logger;
            _rabbitRepository = rabbitRepository;
        }

        public async Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pedidos = await _rabbitRepository.BuscarPedidosSinProcesarAsync(cancellationToken);
                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar pedidos sin procesar.");
                throw;
            }
        }

        public async Task<List<PedidoCreateEvent>> GenerarColaPedidos(List<PedidoColaDto> pedidos)
        {
            try
            {
                var pedidosCompletos = await _rabbitRepository.BuscarPedidosSinProcesarAsync(pedidos, CancellationToken.None);

                var eventos = WRabbitMapper.MapearPedidosAEventos(pedidosCompletos);

                return eventos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar pedidos.");
                throw;
            }
        }

        

    }
}