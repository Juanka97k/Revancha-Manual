using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Api.DTOS
{
    public record PedidosCreateDto(
        string ClienteNombre,
        string Sku,
        int Cantidad
    );
}