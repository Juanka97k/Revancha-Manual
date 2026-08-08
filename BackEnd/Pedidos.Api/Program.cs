using Microsoft.EntityFrameworkCore;
using Ordenes.Infraestructura.Context;

var builder = WebApplication.CreateBuilder(args);

//conexion a la base de datos
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<OrdenesDbContext>(options =>
    options.UseNpgsql(connectionString));

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
