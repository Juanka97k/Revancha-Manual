using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordenes.Infraestructura.Entities
{
    public class PedidoProcesado
    {
        public Guid PedidoId { get; set; }
        public DateTime FechaProcesamiento { get; set; } = DateTime.UtcNow;
    }
}