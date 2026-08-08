using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ordenes.Infraestructura.Context
{
    public class OrdenesDbContext : DbContext
    {
        public OrdenesDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entities.Pedido> Pedidos { get; set; }
        public DbSet<Entities.Stock> Stocks { get; set; }
        public DbSet<Entities.PedidoProcesado> PedidosProcesados { get; set; }

    }
}