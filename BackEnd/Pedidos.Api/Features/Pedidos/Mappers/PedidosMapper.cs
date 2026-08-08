using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Api.Dtos;
using Pedidos.Api.Features.Pedidos.interfaces;
using Pedidos.Infraestructura.Entities;

namespace Pedidos.Api.Features.Pedidos.Mappers
{
    public interface IPedidosMapper
    {
        Pedido PedidoMapper(PedidosCreateDto request);
        PedidoCola PedidoColaMapper(Pedido pedido);
        PedidosResponseDto PedidoResponseMapper(Pedido pedido);
    }

    public class PedidosMapper : IPedidosMapper
    {
        public Pedido PedidoMapper(PedidosCreateDto request)
        {
            return new Pedido
            {
                Id = Guid.NewGuid(),
                NombreCliente = request.ClienteNombre,
                Sku = request.Sku,
                Cantidad = request.Cantidad,
                Estado = Status.Pendiente,
                FechaCreacion = DateTime.UtcNow
            };
        }

        public PedidoCola PedidoColaMapper(Pedido pedido)
        {
            return new PedidoCola
            {
                PedidoId = pedido.Id,
                CreadoEn = DateTime.UtcNow
            };
        }

        public PedidosResponseDto PedidoResponseMapper(Pedido pedido)
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