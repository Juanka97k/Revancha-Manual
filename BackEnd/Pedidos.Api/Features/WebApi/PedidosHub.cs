using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Pedidos.Api.Features.WebApi
{
    public interface IPedidosClient
    {
        // El cliente escuchará este evento
        Task RecibirEstadoPedidoActualizado(Guid pedidoId, string estado, string mensaje);
    }
    public class PedidosHub : Hub<IPedidosClient>
    {
        
        private readonly ILogger<PedidosHub> _logger;
        
        public PedidosHub(ILogger<PedidosHub> logger)
        {
            _logger = logger;
        }

        // // Permite que un cliente se una a un "grupo" de un pedido específico
        // public async Task SuscribirAPedido(string pedidoId)
        // {
        //     await Groups.AddToGroupAsync(Context.ConnectionId, pedidoId);
        //     _logger.LogInformation("Cliente {ConnectionId} suscrito al pedido {PedidoId}", Context.ConnectionId, pedidoId);
        // }
        
        // Se ejecuta cuando un cliente se conecta
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("🟢 Cliente conectado a SignalR: ConnectionId = {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("🔴 Cliente desconectado de SignalR: ConnectionId = {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}