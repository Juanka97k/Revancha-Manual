using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pedidos.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class CambioNombreColumnaPedidosProcesados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PedidoId",
                table: "PedidosProcesados",
                newName: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventoId",
                table: "PedidosProcesados",
                newName: "PedidoId");
        }
    }
}
