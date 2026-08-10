using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using Shared.Events;

namespace Worker.Pedidos.Mapper
{
    public static class PedidosMapper
    {
        public static PedidoProcesado PedidoProcesadoEvent(PedidoCreateEvent pedido)
        {
            return new PedidoProcesado
            {
                EventoId            = pedido.EventoId,
                FechaProcesamiento  = DateTime.UtcNow
            };
        }
    }
}