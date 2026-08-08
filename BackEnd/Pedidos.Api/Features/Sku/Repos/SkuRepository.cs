using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;

namespace Pedidos.Api.Features.Sku
{
    public interface ISkuRepository
    {
        Task<bool> SkuExisteAsync(string sku, CancellationToken cancellationToken);
    }

    public class SkuRepository : ISkuRepository
    {
        private readonly PedidosDbContext _context;

        public SkuRepository(PedidosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SkuExisteAsync(string sku, CancellationToken cancellationToken)
        {
            return await _context.Stocks.AnyAsync(s => s.Sku == sku.ToUpper(), cancellationToken);
        }

    }
}