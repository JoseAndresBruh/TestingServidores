using Api.Infrastructure;
using Api.Features.Usuarios;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Detección 100% segura: Si estamos en testing, Aspire NO se ejecuta
if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ApiTestingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
}
else
{
    // Solo se ejecuta en desarrollo/producción con el orquestador
    builder.AddSqlServerDbContext<AppDbContext>("sqldata");
}

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) =>
{
    var id = await sender.Send(comando);
    return Results.Ok(new { Id = id });
});

app.MapGetUserById();
app.MapGet("/", () => "API funcionando!");
app.Run();