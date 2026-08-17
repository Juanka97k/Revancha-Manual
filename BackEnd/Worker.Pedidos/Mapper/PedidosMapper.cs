using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Entities;
using Shared.Events;

namespace Worker.Pedidos.Mapper
{
    public static class PedidosMapper
    {
        public static PedidoProcesado PedidoProcesadoMapper(PedidoCreateEvent pedido)
        {
            return new PedidoProcesado
            {
                EventoId            = pedido.EventoId,
                FechaProcesamiento  = DateTime.UtcNow
            };
        }

        public static PedidoProcesadoEvent PedidoProcesadoEventMapper(Pedido request)
        {
            return new PedidoProcesadoEvent(
                request.Id,
                request.Estado.ToString(),
                request.FechaCreacion
            );
        }

        public static MensajesOutbox PedidoProcesadoColaMapper(PedidoProcesadoEvent evento)
        {
            var eventoSerialize = JsonSerializer.Serialize(evento);

            return new MensajesOutbox
            {
                Id = Guid.NewGuid(),
                PedidoId = evento.PedidoId,
                TipoEvento = nameof(PedidoProcesadoEvent).ToString(),
                Payload = eventoSerialize,
                CreadoEn = DateTime.UtcNow,
                Estado = EstadoOutbox.SinProcesar
                
            };
        }
    }
}