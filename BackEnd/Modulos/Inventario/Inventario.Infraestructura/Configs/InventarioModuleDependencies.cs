using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.Aplicacion.Interfaces;
using Inventario.Aplicacion.Services;
using Inventario.Infraestructura.Context;
using Inventario.Infraestructura.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventario.Infraestructura.Configs
{
    public static class InventarioModuleDependencies
    {
        public static IServiceCollection AddInventarioModule(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configurar DbContext del módulo
            services.AddDbContext<InventarioDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));

            // 2. Registrar Repositorios y Servicios
            services.AddScoped<IInventarioRepository, InventarioRespository>();
            services.AddScoped<IInventarioServices, InventarioServices>();
            return services;
        }
    }
}