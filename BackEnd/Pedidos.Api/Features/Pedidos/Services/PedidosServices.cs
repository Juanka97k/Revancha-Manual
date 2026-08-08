using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ordenes.Infraestructura.Entities;
using Pedidos.Api.DTOS;
using Pedidos.Api.Features.Pedidos.interfaces;

namespace Pedidos.Api.Features.Pedidos.Services
{
    public class PedidosServices : IPedidosServices
    {
        private readonly ILogger<PedidosServices> _logger;
        public PedidosServices( ILogger<PedidosServices> logger)
        {
            _logger = logger;
        }
        

        public Task<PedidosResponseDto> CrearPedidoAsync(PedidosCreateDto request)
        {
            var response = new PedidosResponseDto
            (
                Id              : Guid.NewGuid(),
                ClienteNombre   : request.ClienteNombre,
                Sku             : request.Sku,
                Cantidad        :  request.Cantidad,
                Estado          : Status.Pendiente,
                CreadoEn        : DateTime.UtcNow
            );

            return Task.FromResult(response);
        }
        
    }
}