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
    public interface IPedidosPublishRepository
    {
        Task<List<MensajeOutboxDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken);
        Task<List<MensajeOutboxDto>> BuscarPedidosYaProcesadosAsync(CancellationToken cancellationToken);
        Task<List<Pedido>> BuscarPedidosAsync(List<MensajeOutboxDto> Pedidos, CancellationToken cancellationToken);
        Task ActualizarEstadoOutboxAsync(Guid outboxId, EstadoOutbox nuevoEstado, CancellationToken cancellationToken);
    }

    public class PedidosPublishRepository : IPedidosPublishRepository
    {
        private readonly PedidosDbContext _context;

        public PedidosPublishRepository(PedidosDbContext context)
        {
            _context = context;
        }

        public async Task<List<MensajeOutboxDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken)
        {
            return await _context.MensajesOutbox
            .Where(p => p.Estado == EstadoOutbox.SinProcesar 
            && p.TipoEvento == nameof(PedidoCreateEvent).ToString())
            .OrderBy(p => p.CreadoEn)
            .Select(p => new MensajeOutboxDto(
                p.Id,
                p.PedidoId,
                p.Estado,
                p.Payload,
                p.CreadoEn
            ))
            .ToListAsync(cancellationToken);
        }

        public async Task<List<MensajeOutboxDto>> BuscarPedidosYaProcesadosAsync(CancellationToken cancellationToken)
        {
            return await _context.MensajesOutbox
            .Where(p => p.Estado == EstadoOutbox.SinProcesar 
            && p.TipoEvento == nameof(PedidoProcesadoEvent).ToString())
            .OrderBy(p => p.CreadoEn)
            .Select(p => new MensajeOutboxDto(
                p.Id,
                p.PedidoId,
                p.Estado,
                p.Payload,
                p.CreadoEn
            ))
            .ToListAsync(cancellationToken);
        }

        public async Task<List<Pedido>> BuscarPedidosAsync(List<MensajeOutboxDto> Pedidos, CancellationToken cancellationToken)
        {
            var pedidoIds = Pedidos.Select(p => p.PedidoId).ToList();

            return await _context.Pedidos
                .Where(p => pedidoIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task ActualizarEstadoOutboxAsync(Guid outboxId, EstadoOutbox nuevoEstado, CancellationToken cancellationToken)
        {
            // Actualización atómica directa sin pasar por Change Tracker
            await _context.MensajesOutbox
                .Where(p => p.Id == outboxId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Estado, nuevoEstado),
                    cancellationToken);
        }

    }
}