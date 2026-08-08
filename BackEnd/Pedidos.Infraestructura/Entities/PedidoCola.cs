using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Infraestructura.Entities
{
    public class PedidoCola
    {
        public string ClienteNombre { get; set; } 
        public string Sku { get; set; }
        public int Cantidad { get; set; }
    }
}