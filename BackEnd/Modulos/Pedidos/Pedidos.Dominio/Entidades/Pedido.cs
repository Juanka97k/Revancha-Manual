using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Dominio.Entidades
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}