using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Worker.Pedidos;
using Worker.Pedidos.Repos;
using Worker.Pedidos.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddSingleton<IRabbitConfig, RabbitConfig>();
builder.Services.AddScoped<IRabbitService, RabbitService>();
builder.Services.AddScoped<IRabbitRepository, RabbitRepository>();

builder.Services.AddHostedService<PedidosWorker>();

var host = builder.Build();
host.Run();
