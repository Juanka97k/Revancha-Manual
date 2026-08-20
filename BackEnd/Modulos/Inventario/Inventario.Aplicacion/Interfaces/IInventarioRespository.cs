using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Aplicacion.Interfaces
{
    public interface IInventarioRepository
    {
        Task<bool> SkuExisteAsync(string sku, CancellationToken cancellationToken);
    }
}