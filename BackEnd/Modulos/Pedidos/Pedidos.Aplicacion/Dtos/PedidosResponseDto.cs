using System;
using Pedidos.Dominio.Entidades;

namespace Pedidos.Aplicacion.Dtos
{
    public record PedidosResponseDto(
        Guid Id,
        string ClienteNombre,
        string Sku,
        int Cantidad,
        EstadoPedido Estado,
        DateTime CreadoEn
    );
}