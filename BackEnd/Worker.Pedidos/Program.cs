using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Worker.Pedidos;
using Worker.Pedidos.Repos;
using Worker.Pedidos.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddSingleton<IRabbitConfig, RabbitConfig>();
builder.Services.AddScoped<IProcesarPedidoService, ProcesarPedidoService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

builder.Services.AddHostedService<PedidosWorker>();

var host = builder.Build();
host.Run();
