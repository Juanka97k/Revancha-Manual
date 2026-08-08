using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordenes.Infraestructura.Entities
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public Status Estado { get; set; } = Status.Pendiente;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}