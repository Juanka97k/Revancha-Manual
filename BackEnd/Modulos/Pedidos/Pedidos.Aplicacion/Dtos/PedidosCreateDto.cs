namespace Pedidos.Aplicacion.Dtos
{
    public record PedidosCreateDto(
        string ClienteNombre,
        string Sku,
        int Cantidad
    );
}