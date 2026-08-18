

namespace Shared.Dtos
{
    public record MensajeOutboxDto(
        Guid OutboxId, // Clave primaria del mensaje en la tabla MensajesOutbox
        Guid PedidoId,
        EstadoOutbox Estado,
        string PayLoad,
        DateTime CreadoEn
    );

}