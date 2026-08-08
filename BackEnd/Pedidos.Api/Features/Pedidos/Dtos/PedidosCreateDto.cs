using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Api.Dtos
{
    public record PedidosCreateDto(
        string ClienteNombre,
        string Sku,
        int Cantidad
    );
}