using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record PedidoCreateEvent
    (
        Guid EventoId,
        Guid PedidoId,
        string Sku,
        int Cantidad,
        DateTime CreadoEn
    );

}