using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using Shared.Dtos;
using Shared.Events;

namespace Worker.RabbitQM.Mapper
{
    public static class WRabbitMapper
    {
        public static List<PedidoCreateEvent> MapearPedidosAEventos(List<Pedido> pedidos)
        {
            var eventos = new List<PedidoCreateEvent>();

            foreach (var pedido in pedidos)
            {
                var evento = new PedidoCreateEvent(
                    EventoId: Guid.NewGuid(),
                    PedidoId: pedido.Id,
                    Sku: pedido.Sku,
                    Cantidad: pedido.Cantidad,
                    CreadoEn: pedido.FechaCreacion
                );

                eventos.Add(evento);
            }

            return eventos;
        }
    }
}