using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ordenes.Infraestructura.Entities;

namespace Pedidos.Api.DTOS
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