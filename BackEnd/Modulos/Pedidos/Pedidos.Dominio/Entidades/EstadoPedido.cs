using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Dominio.Entidades
{
    public enum EstadoPedido
    {
        Pendiente = 0,
        Procesando = 1,
        Enviado = 2,
        Entregado = 3,
        Cancelado = 4
    }

    public static class EstadoPedidoExtensions
    {
        public static string ToDescriptionString(this EstadoPedido estado)
        {
            return estado switch
            {
                EstadoPedido.Pendiente => "Pendiente",
                EstadoPedido.Procesando => "Procesando",
                EstadoPedido.Enviado => "Enviado",
                EstadoPedido.Entregado => "Entregado",
                EstadoPedido.Cancelado => "Cancelado",
                _ => "Desconocido",
            };
        }
    }
}