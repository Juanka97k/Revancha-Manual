using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pedidos.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddNuevaTablayestados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "PedidoCola",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "PedidoCola");
        }
    }
}
