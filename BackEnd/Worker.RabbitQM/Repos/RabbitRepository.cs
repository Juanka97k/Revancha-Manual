using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Pedidos.Infraestructura.Entities;
using Shared.Dtos;

namespace Worker.RabbitQM.Repos
{
    public interface IRabbitRepository
    {
        Task<List<PedidoColaDto>> BuscarPedidosSinProcesarAsync(CancellationToken cancellationToken);
        Task<List<Pedido>> BuscarPedidosSinProcesarAsync(List<PedidoColaDto> Pedidos, CancellationToken cancellationToken);
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
            return await _context.PedidoCola
            .Where(p => p.Estado == EstadosProcesamiento.SinProcesar)
            .OrderBy(p => p.CreadoEn)
            .Select(p => new PedidoColaDto(
                p.PedidoId,
                p.Estado,
                p.CreadoEn
            ))
            .ToListAsync(cancellationToken);
        }

        public async Task<List<Pedido>> BuscarPedidosSinProcesarAsync(List<PedidoColaDto> Pedidos, CancellationToken cancellationToken)
        {
            var pedidoIds = Pedidos.Select(p => p.PedidoId).ToList();

            return await _context.Pedidos
                .Where(p => pedidoIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

    }
}