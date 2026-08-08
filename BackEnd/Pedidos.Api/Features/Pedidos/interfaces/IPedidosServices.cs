using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Api.DTOS;

namespace Pedidos.Api.Features.Pedidos.interfaces
{
    public interface IPedidosServices
    {
        public Task<PedidosResponseDto> CrearPedidoAsync(PedidosCreateDto request);
    }
}