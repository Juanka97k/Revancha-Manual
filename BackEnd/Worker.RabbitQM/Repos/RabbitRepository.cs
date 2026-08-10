using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Pedidos.Infraestructura.Entities;
using Shared.Dtos;
using Shared.Events;

namespace Worker.RabbitQM.Repos
{
    public interface IRabbitRepository
    {
        Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken);
        Task<List<Pedido>> BuscarPedidosAsync(List<PedidoColaDto> Pedidos, CancellationToken cancellationToken);
        Task ActualizarEstadoPedidoAsync(PedidoCreateEvent pedidos, EstadoOutbox nuevoEstado, CancellationToken cancellationToken);
    }

    public class RabbitRepository : IRabbitRepository
    {
        private readonly PedidosDbContext _context;

        public RabbitRepository(PedidosDbContext context)
        {
            _context = context;
        }

        public async Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken)
        {
            return await _context.MensajesOutbox
            .Where(p => p.Estado == EstadoOutbox.SinProcesar 
            && p.TipoEvento == nameof(PedidoCreateEvent).ToString())
            .OrderBy(p => p.CreadoEn)
            .Select(p => new PedidoColaDto(
                p.PedidoId,
                p.Estado,
                p.Payload,
                p.CreadoEn
            ))
            .ToListAsync(cancellationToken);
        }

        public async Task<List<Pedido>> BuscarPedidosAsync(List<PedidoColaDto> Pedidos, CancellationToken cancellationToken)
        {
            var pedidoIds = Pedidos.Select(p => p.PedidoId).ToList();

            return await _context.Pedidos
                .Where(p => pedidoIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task ActualizarEstadoPedidoAsync(PedidoCreateEvent pedidos, EstadoOutbox nuevoEstado, CancellationToken cancellationToken)
        {
            var pedidoCola = await _context.MensajesOutbox
                .FirstOrDefaultAsync(p => p.PedidoId == pedidos.PedidoId, cancellationToken);

            if (pedidoCola != null)
            {
                pedidoCola.Estado = nuevoEstado;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

    }
}