using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Dominio.Entities
{
    public class Stock
    {
        public String Sku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}