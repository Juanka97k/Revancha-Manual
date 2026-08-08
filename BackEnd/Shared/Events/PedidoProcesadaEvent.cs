using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Events
{
    public record OrdenProcesadaEvent
    (
        Guid OrdenId,
        string Estado,
        DateTime FechaProcesamiento
    );
}