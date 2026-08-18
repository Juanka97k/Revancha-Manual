using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Worker.RabbitQM;
using Worker.RabbitQM.Repos;
using Worker.RabbitQM.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPedidosPublishRepository, PedidosPublishRepository>();
builder.Services.AddScoped<IPedidosPublishServices, PedidosPublishServices>();
builder.Services.AddSingleton<IRabbitConfigs, RabbitConfigs>();

builder.Services.AddHostedService<RabbitMqWorker>();

var host = builder.Build();
host.Run();
