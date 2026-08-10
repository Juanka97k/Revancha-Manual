using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record PedidoProcesadoEvent
    (
        Guid EventoId,
        DateTime FechaProcesamiento
    );
}