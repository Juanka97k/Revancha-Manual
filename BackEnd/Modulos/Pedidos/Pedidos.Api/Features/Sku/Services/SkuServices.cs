using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedidos.Api.Features.Sku
{
    public interface ISkuServices
    {
        Task<bool> VerificarExistenciaSkuAsync(string sku, CancellationToken cancellationToken);
    }

    public class SkuServices : ISkuServices
    {
        private readonly ISkuRepository _skuRepository;

        public SkuServices(ISkuRepository skuRepository)
        {
            _skuRepository = skuRepository;
        }

        public async Task<bool> VerificarExistenciaSkuAsync(string sku, CancellationToken cancellationToken)
        {
            return await _skuRepository.SkuExisteAsync(sku, cancellationToken);
        }
    }
}