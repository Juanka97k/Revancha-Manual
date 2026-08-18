using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Infraestructura.Entities
{
    public class PedidoProcesado
    {
        public Guid EventoId { get; set; }
        public DateTime FechaProcesamiento { get; set; } = DateTime.UtcNow;
    }
}