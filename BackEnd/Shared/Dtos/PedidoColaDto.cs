using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;

namespace Shared.Dtos
{
    public record PedidoColaDto(
        Guid PedidoId,
        EstadoOutbox Estado,
        string PayLoad,
        DateTime CreadoEn
    );

}