using System;
using Pedidos.Aplicacion.Dtos;
using Pedidos.Aplicacion.Interfaces;
using Pedidos.Aplicacion.Mappers;

namespace Pedidos.Aplicacion.Services
{
    public class PedidosServices : IPedidosServices
    {
        private readonly ILogger<PedidosServices> _logger;
        //private readonly IPedidosMapper _pedidosMapper;

        private readonly ISkuServices _skuServices;

        private readonly IPedidosRepository _pedidosRepository;
        public PedidosServices(
            ILogger<PedidosServices> logger,
            //IPedidosMapper pedidosMapper, 
            ISkuServices skuServices,
            IPedidosRepository pedidosRepository
            )
        {
            _logger = logger;
            //_pedidosMapper = pedidosMapper;
            _skuServices = skuServices;
            _pedidosRepository = pedidosRepository;
        }


        public async Task<PedidosResponseDto> CrearPedidoAsync(PedidosCreateDto request, CancellationToken cancellationToken)
        {
            try
            {

                var skuExiste = await _skuServices.VerificarExistenciaSkuAsync(request.Sku, cancellationToken);
                if (!skuExiste)
                {
                    throw new Exception($"El SKU '{request.Sku}' no existe en la base de datos.");
                }

                var pedido = PedidosMapper.PedidoMapper(request);

                _pedidosRepository.CrearPedido(pedido);

                var evento = PedidosMapper.PedidoCreateEventMapper(request, pedido.Id);

                var eventoResult = PedidosMapper.PedidoColaMapper(evento);

                _pedidosRepository.GuardarColaPedido(eventoResult);

                await _pedidosRepository.GuardaCambios(cancellationToken);

                var response = PedidosMapper.PedidoResponseMapper(pedido);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear el pedido para el cliente {ClienteNombre}.", request.ClienteNombre);
                throw;
            }
        }

    }
}