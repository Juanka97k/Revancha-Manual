using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Infraestructura.Entities
{
    public class PedidoCola
    {
        public Guid PedidoId { get; set; }
        public EstadosProcesamiento Estado { get; set; }
        public DateTime CreadoEn { get; set; }
    }
}