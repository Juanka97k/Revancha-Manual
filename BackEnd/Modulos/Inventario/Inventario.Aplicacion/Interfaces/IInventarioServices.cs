using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Aplicacion.Interfaces
{
    public interface IInventarioServices
    {
        Task<bool> VerificarExistenciaSkuAsync(string sku, CancellationToken cancellationToken);
    }
}