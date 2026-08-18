using Pedidos.Aplicacion.Interfaces;
using Pedidos.Infraestructura.Context;
using Pedidos.Dominio.Entidades;

namespace Pedidos.Infraestructura.Repos
{
    public class PedidosRepository : IPedidosRepository
    {
        private readonly PedidosDbContext _context;

        public PedidosRepository(PedidosDbContext context)
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