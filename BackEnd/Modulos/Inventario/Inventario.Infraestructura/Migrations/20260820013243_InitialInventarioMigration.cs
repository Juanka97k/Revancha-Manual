using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inventario.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InitialInventarioMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventario");

            migrationBuilder.CreateTable(
                name: "MensajesOutbox",
                schema: "inventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoEvento = table.Column<string>(type: "text", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                schema: "inventario",
                columns: table => new
                {
                    Sku = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Sku);
                });

            migrationBuilder.InsertData(
                schema: "inventario",
                table: "Stocks",
                columns: new[] { "Sku", "Cantidad" },
                values: new object[,]
                {
                    { "SKU001", 100 },
                    { "SKU002", 50 },
                    { "SKU003", 75 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajesOutbox",
                schema: "inventario");

            migrationBuilder.DropTable(
                name: "Stocks",
                schema: "inventario");
        }
    }
}
