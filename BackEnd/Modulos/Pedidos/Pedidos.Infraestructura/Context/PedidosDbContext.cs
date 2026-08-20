using Microsoft.EntityFrameworkCore;
using Pedidos.Dominio.Entidades;

namespace Pedidos.Infraestructura.Context
{
    public class PedidosDbContext : DbContext
    {
        public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
        {
        }

        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<MensajesOutbox> MensajesOutbox => Set<MensajesOutbox>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("pedidos");

            modelBuilder.Entity<MensajesOutbox>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PedidoId).IsRequired();
                entity.Property(e => e.TipoEvento).IsRequired();
                entity.Property(e => e.Payload).IsRequired();
                entity.Property(e => e.CreadoEn).IsRequired();
                entity.Property(e => e.Estado).IsRequired();
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Estado).IsRequired();
                entity.Property(e => e.Sku).IsRequired();
                entity.Property(e => e.Cantidad).IsRequired();
            });
        }
    }
}
