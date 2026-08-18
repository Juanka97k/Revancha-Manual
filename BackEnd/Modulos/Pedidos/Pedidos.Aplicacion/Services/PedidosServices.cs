using System;
using Microsoft.Extensions.Logging;
using Pedidos.Aplicacion.Dtos;
using Pedidos.Aplicacion.Interfaces;
using Pedidos.Aplicacion.Mappers;
using Pedidos.Dominio.Entidades;

namespace Pedidos.Aplicacion.Services
{
    public class PedidosServices : IPedidosServices
    {
        private readonly ILogger<PedidosServices> _logger;


        private readonly IPedidosRepository _pedidosRepository;
        
        public PedidosServices(
            ILogger<PedidosServices> logger,
            IPedidosRepository pedidosRepository
            )
        {
            _logger = logger;
            _pedidosRepository = pedidosRepository;
        }


        public async Task<PedidosResponseDto> CrearPedidoAsync(PedidosCreateDto request, CancellationToken cancellationToken)
        {
            try
            {
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