using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Aplicacion.Interfaces;
using Inventario.Infraestructura.Context;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infraestructura.Repos
{
    public class InventarioRespository : IInventarioRepository
    {
        private readonly InventarioDbContext _context;

        public InventarioRespository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SkuExisteAsync(string sku, CancellationToken cancellationToken)
        {
            return await _context.Stocks.AnyAsync(s => s.Sku.ToUpper() == sku.ToUpper(), cancellationToken);
        }

    }
}