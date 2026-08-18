using System;
using System.Threading.Tasks;
using Pedidos.Aplicacion.Dtos;

namespace Pedidos.Aplicacion.Interfaces
{
    public interface IPedidosServices
    {
        public Task<PedidosResponseDto> CrearPedidoAsync(PedidosCreateDto request, CancellationToken cancellationToken);
    }
}