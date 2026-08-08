using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Worker.RabbitQM;
using Worker.RabbitQM.Repos;
using Worker.RabbitQM.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRabbitRepository, RabbitRepository>();
builder.Services.AddScoped<IRabbitServices, RabbitServices>();

builder.Services.AddHostedService<RabbitMqWorker>();

var host = builder.Build();
host.Run();
