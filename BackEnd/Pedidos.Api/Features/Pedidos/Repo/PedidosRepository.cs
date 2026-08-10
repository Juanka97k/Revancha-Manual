using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pedidos.Infraestructura.Context;
using Pedidos.Infraestructura.Entities;
using Pedidos.Api.Features.Pedidos.interfaces;

namespace Pedidos.Api.Features.Pedidos.Repo
{
    public class PedidosRepository : IPedidosRepository
    {
        private readonly PedidosDbContext _context;

        public PedidosRepository( PedidosDbContext context)
        {
            _context = context;
        }

        public void CrearPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
        }

        public async Task GuardaCambios(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }


        public void GuardarColaPedido(MensajesOutbox cola)
        {
            _context.MensajesOutbox.Add(cola);
        }
    }
}