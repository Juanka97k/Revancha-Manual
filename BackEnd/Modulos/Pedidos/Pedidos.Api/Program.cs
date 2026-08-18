using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Pedidos.Infraestructura.Context;
using Pedidos.Infraestructura.Repos;
using Pedidos.Aplicacion.Interfaces;
using Pedidos.Aplicacion.Services;
using Pedidos.Aplicacion.Validators;
using FluentValidation;
using Pedidos.Api.Features.WebApi;
using Pedidos.Api.Features.BackGround;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// 1. Agregar servicios de SignalR
builder.Services.AddSignalR();

// 6. Registrar Consumidor de RabbitMQ en Segundo Plano (Transmisor a SignalR)
builder.Services.AddHostedService<PredidosProcesadosConsumerServices>();

//conexion a la base de datos
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseNpgsql(connectionString));

PedidosConfigureServices(builder.Services);

builder.Services.AddSingleton<IRabbitConfig, RabbitConfig>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PedidosDbContext>();
    await dbContext.Database.MigrateAsync();
}

// 2. Mapear la ruta del Hub
app.MapHub<PedidosHub>("/hubs/pedidos");

app.Run();


static void PedidosConfigureServices(
    IServiceCollection services)
{
    services.AddScoped<IPedidosServices, PedidosServices>();
    //services.AddScoped<IPedidosMapper, PedidosMapper>();
    services.AddScoped<IPedidosRepository, PedidosRepository>();
    services.AddValidatorsFromAssemblyContaining<PedidosCreateDtoValidator>();
}