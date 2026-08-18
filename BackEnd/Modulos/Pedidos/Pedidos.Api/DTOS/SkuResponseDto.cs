using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Api.Dtos
{
    public record SkuResponseDto(
        string Sku,
        int Cantidad
    );
}