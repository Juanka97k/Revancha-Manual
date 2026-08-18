using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pedidos.Aplicacion.Dtos;
using Pedidos.Dominio.Entidades;


namespace Pedidos.Aplicacion.Mappers
{

    public static class PedidosMapper
    {
        public static Pedido PedidoMapper(PedidosCreateDto request)
        {
            return new Pedido
            {
                Id = Guid.NewGuid(),
                NombreCliente = request.ClienteNombre.ToLower(),
                Sku = request.Sku.ToUpper(),
                Cantidad = request.Cantidad,
                Estado = Status.Pendiente,
                FechaCreacion = DateTime.UtcNow
            };
        }

        public static PedidoCreateEvent PedidoCreateEventMapper(PedidosCreateDto request, Guid pedidoId)
        {
            return new PedidoCreateEvent(
                Guid.NewGuid(),
                pedidoId,
                request.Sku.ToUpper(),
                request.Cantidad,
                DateTime.UtcNow
            );
        }

        public static MensajesOutbox PedidoColaMapper(PedidoCreateEvent evento)
        {
            var eventoSerialize = JsonSerializer.Serialize(evento);

            return new MensajesOutbox
            {
                Id = evento.EventoId,
                PedidoId = evento.PedidoId,
                TipoEvento = nameof(PedidoCreateEvent).ToString(),
                Payload = eventoSerialize,
                CreadoEn = DateTime.UtcNow,
                Estado = EstadoOutbox.SinProcesar

            };
        }

        public static PedidosResponseDto PedidoResponseMapper(Pedido pedido)
        {
            return new PedidosResponseDto(
                pedido.Id,
                pedido.NombreCliente,
                pedido.Sku,
                pedido.Cantidad,
                pedido.Estado,
                pedido.FechaCreacion
            );
        }
    }
}