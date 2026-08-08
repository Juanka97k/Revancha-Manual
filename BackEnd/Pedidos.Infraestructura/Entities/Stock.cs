using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Infraestructura.Entities
{
    public class Stock
    {
        public String Sku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}