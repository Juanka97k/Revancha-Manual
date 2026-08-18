using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;

namespace Pedidos.Api.Dtos
{
    public record PedidosResponseDto(
        Guid Id,
        string ClienteNombre,
        string Sku,
        int Cantidad,
        Status Estado,
        DateTime CreadoEn
    );
}