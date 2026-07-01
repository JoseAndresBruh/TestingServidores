using Api.Infrastructure;
using Api.Features.Usuarios;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos inyectada por Aspire
builder.AddSqlServerDbContext<AppDbContext>("sqldata");

// Registro de servicios
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Endpoint de prueba para crear usuario
app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) =>
{
    var id = await sender.Send(comando);
    return Results.Ok(new { Id = id });
});

app.MapGet("/", () => "API de Servidores funcionando con CQRS y Aspire!");

app.Run();