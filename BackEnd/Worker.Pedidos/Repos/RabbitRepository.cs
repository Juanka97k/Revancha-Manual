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
    public interface IRabbitRepository
    {
        Task<bool> ValidarPedidoProcesado(PedidoCreateEvent pedido, CancellationToken cancellationToken);
        Task<Pedido?> BuscarPedidoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken);
        Task<Stock?> BuscarStockAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken);
        void GuardarPedidoProcesadoAsync(PedidoProcesado pedidoProcesado, CancellationToken cancellationToken);
        Task GuardarCambiosAsync(CancellationToken cancellationToken);
    }

    public class RabbitRepository : IRabbitRepository
    {
        private readonly PedidosDbContext _context;

        public RabbitRepository(PedidosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidarPedidoProcesado(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {
            return await _context.PedidosProcesados
                .AnyAsync(p => p.EventoId == pedido.EventoId, cancellationToken);
        }

        public async Task<Pedido?> BuscarPedidoAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {
            return await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == pedido.PedidoId, cancellationToken);
        }

        public async Task<Stock?> BuscarStockAsync(PedidoCreateEvent pedido, CancellationToken cancellationToken)
        {
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Sku == pedido.Sku, cancellationToken);
        }

        public void GuardarPedidoProcesadoAsync(PedidoProcesado pedidoProcesado, CancellationToken cancellationToken)
        {
            _context.PedidosProcesados.Add(pedidoProcesado);
        }
        public async Task GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

    
    }
}