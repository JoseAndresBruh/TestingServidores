using Api.Infrastructure;
using Api.Features.Usuarios;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Intenta obtener la conexión desde la configuración
var connectionString = builder.Configuration.GetConnectionString("sqldata");

if (string.IsNullOrEmpty(connectionString))
{
    // Si no encuentra la conexión de Aspire, intenta buscarla en la sección estándar
    builder.AddSqlServerDbContext<AppDbContext>("sqldata");
}
else
{
    // Forzamos la conexión manualmente si estamos en testing
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) =>
{
    var id = await sender.Send(comando);
    return Results.Ok(new { Id = id });
});

app.MapGet("/", () => "API funcionando!");

app.Run();