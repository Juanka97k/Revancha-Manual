using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventario.Dominio.Entities
{
    public class MensajesOutbox
    {
        public Guid Id { get; set; }

        public Guid PedidoId { get; set; }

        public string TipoEvento { get; set; } = string.Empty;

        public string Payload { get; set; } = string.Empty;

        public DateTime CreadoEn { get; set; }

        public EstadoOutbox Estado { get; set; }
    }
}