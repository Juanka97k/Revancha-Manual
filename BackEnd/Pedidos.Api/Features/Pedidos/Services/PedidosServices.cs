using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using Pedidos.Api.Dtos;
using Pedidos.Api.Features.Pedidos.interfaces;
using Pedidos.Api.Features.Pedidos.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Api.Features.Pedidos.Services
{
    public class PedidosServices : IPedidosServices
    {
        private readonly ILogger<PedidosServices> _logger;
        private readonly IPedidosMapper _pedidosMapper;

        private readonly IPedidosRepository _pedidosRepository;
        public PedidosServices( 
            ILogger<PedidosServices> logger, 
            IPedidosMapper pedidosMapper, 
            IPedidosRepository pedidosRepository
            )
        {
            _logger = logger;
            _pedidosMapper = pedidosMapper;
            _pedidosRepository = pedidosRepository;
        }
        

        public async Task<PedidosResponseDto> CrearPedidoAsync(PedidosCreateDto request, CancellationToken cancellationToken)
        {
            try
            {
                var pedido = _pedidosMapper.PedidoMapper(request);

                 _pedidosRepository.CrearPedido(pedido);

                var cola = _pedidosMapper.PedidoColaMapper(pedido);

                 _pedidosRepository.GuardarColaPedido(cola);

                await _pedidosRepository.GuardaCambios(cancellationToken);

                var response = _pedidosMapper.PedidoResponseMapper(pedido);

                return response;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al intentar guardar el pedido del cliente {ClienteNombre}.", request.ClienteNombre);
                throw new Exception("Error de persistencia al intentar registrar el pedido en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear el pedido para el cliente {ClienteNombre}.", request.ClienteNombre);
                throw;
            }
        }
        
    }
}