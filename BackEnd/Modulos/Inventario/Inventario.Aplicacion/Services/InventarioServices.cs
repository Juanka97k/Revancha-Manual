using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Aplicacion.Interfaces;

namespace Inventario.Aplicacion.Services
{
    public class InventarioServices : IInventarioServices
    {
        private readonly IInventarioRepository _skuRepository;

        public InventarioServices(IInventarioRepository skuRepository)
        {
            _skuRepository = skuRepository;
        }

        public async Task<bool> VerificarExistenciaSkuAsync(string sku, CancellationToken cancellationToken)
        {
            return await _skuRepository.SkuExisteAsync(sku, cancellationToken);
        }
    }
}