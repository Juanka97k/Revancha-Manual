using Worker.Pedidos;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<PedidosWorker>();

var host = builder.Build();
host.Run();
