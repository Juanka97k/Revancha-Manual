using Inventario.Dominio.Entities;
using Microsoft.EntityFrameworkCore;


namespace Inventario.Infraestructura.Context
{
    public class InventarioDbContext : DbContext
    {

        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<MensajesOutbox> MensajesOutbox => Set<MensajesOutbox>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            
        }
    }
}