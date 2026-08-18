using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Pedidos.Infraestructura.Entities;
using Shared.Events;

namespace Worker.Pedidos.Repos
{
    public interface IPedidoRepository
    {
        Task<bool> ValidarPedidoProcesado(Guid EventoId, CancellationToken cancellationToken);
        Task<bool> ValidarColaPedidoProcesado(Guid EventoId, CancellationToken cancellationToken);
        Task MarcarEstadoOutboxAsync (Guid eventoId,EstadoOutbox estado,CancellationToken cancellationToken);
        Task<Pedido?> BuscarPedidoAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task<Stock?> BuscarStockAsync(string sku, CancellationToken cancellationToken);
        void GuardarColaPedido(MensajesOutbox cola);
        void GuardarPedidoProcesado(PedidoProcesado pedidoProcesado);
        Task GuardarCambiosAsync(CancellationToken cancellationToken);
    }

    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidosDbContext _context;

        public PedidoRepository(PedidosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidarPedidoProcesado(Guid EventoId, CancellationToken cancellationToken)
        {
            return await _context.PedidosProcesados
                .AnyAsync(p => p.EventoId == EventoId, cancellationToken);
        }
        public async Task<bool> ValidarColaPedidoProcesado(Guid EventoId, CancellationToken cancellationToken)
        {
            return await _context.MensajesOutbox
                .AnyAsync(p => p.Id == EventoId && p.Estado == EstadoOutbox.Procesado, cancellationToken);
        }

        public async Task MarcarEstadoOutboxAsync (Guid eventoId,EstadoOutbox estado,CancellationToken cancellationToken)
        {
            
            await _context.MensajesOutbox
                .Where(p => p.Id == eventoId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Estado, estado),
                    cancellationToken);
        }

        public async Task<Pedido?> BuscarPedidoAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            return await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == pedidoId, cancellationToken);
        }

        public async Task<Stock?> BuscarStockAsync(string sku, CancellationToken cancellationToken)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Sku == sku, cancellationToken);
        }

        public void GuardarPedidoProcesado(PedidoProcesado pedidoProcesado)
        {
            _context.PedidosProcesados.Add(pedidoProcesado);
        }

        public void GuardarColaPedido(MensajesOutbox cola)
        {
            _context.MensajesOutbox.Add(cola);
        }
        public async Task GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

    
    }
}