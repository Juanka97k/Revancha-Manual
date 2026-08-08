using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos;
using Worker.RabbitQM.Repos;

namespace Worker.RabbitQM.Services
{
    public interface IRabbitServices
    {
        Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken);
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

    }
}