using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pedidos.Infraestructura.Context
{
    public class PedidosDbContext : DbContext
    {
        public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.Pedido> Pedidos => Set<Entities.Pedido>();
        public DbSet<Entities.Stock> Stocks => Set<Entities.Stock>();
        public DbSet<Entities.PedidoProcesado> PedidosProcesados => Set<Entities.PedidoProcesado>();
        public DbSet<Entities.MensajesOutbox> MensajesOutbox => Set<Entities.MensajesOutbox>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.MensajesOutbox>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PedidoId).IsRequired();
                entity.Property(e => e.TipoEvento).IsRequired();
                entity.Property(e => e.Payload).IsRequired();
                entity.Property(e => e.CreadoEn).IsRequired();
                entity.Property(e => e.Estado).IsRequired();
            });

            modelBuilder.Entity<Entities.Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Estado).IsRequired();
                entity.Property(e => e.Sku).IsRequired();
                entity.Property(e => e.Cantidad).IsRequired();
            });

            modelBuilder.Entity<Entities.Stock>(entity =>
            {
                entity.HasKey(e => e.Sku);
                entity.Property(e => e.Sku).ValueGeneratedOnAdd();
                entity.Property(e => e.Cantidad).IsRequired();

                entity.HasData(
                new Entities.Stock { Sku = "SKU001", Cantidad = 100 },
                new Entities.Stock { Sku = "SKU002", Cantidad = 50 },   
                new Entities.Stock { Sku = "SKU003", Cantidad = 75 }
                );
                
            });

            modelBuilder.Entity<Entities.PedidoProcesado>(entity =>
            {
                entity.HasKey(e => e.EventoId);
                entity.Property(e => e.FechaProcesamiento).IsRequired();
            });
        }
    }
}