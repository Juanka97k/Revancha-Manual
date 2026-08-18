using System;
using System.Threading.Tasks;
using Pedidos.Dominio.Entidades;
using Pedidos.Infraestructura.Entities;

namespace Pedidos.Aplicacion.Interfaces
{
    public interface IPedidosRepository
    {
        public void CrearPedido(Pedido pedido);
        public void GuardarColaPedido(MensajesOutbox cola);
        public Task GuardaCambios(CancellationToken cancellationToken = default);
    }
}
