using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ordenes.Infraestructura.Entities;

namespace Pedidos.Api.Features.Pedidos.interfaces
{
    public interface IPedidosRepository
    {
        public void CrearPedido(Pedido pedido);
        public void GuardarColaPedido();
        public void GuardaCambios();
    }
}
