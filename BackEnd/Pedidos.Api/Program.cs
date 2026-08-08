using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Ordenes.Infraestructura.Context;
using Pedidos.Api.Features.Pedidos.interfaces;
using Pedidos.Api.Features.Pedidos.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//conexion a la base de datos
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<OrdenesDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped<IPedidosServices, PedidosServices>();

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

app.Run();
