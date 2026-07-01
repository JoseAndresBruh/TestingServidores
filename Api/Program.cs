using Api.Infrastructure;
using Api.Features.Usuarios;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de base de datos condicional para pruebas
if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ApiTestingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
}
else
{
    builder.AddSqlServerDbContext<AppDbContext>("sqldata");
}

// Registro de servicios de la aplicación
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Endpoints mínimos de la aplicación
app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) => 
    Results.Ok(await sender.Send(comando)));

app.MapListarUsuarios();
app.MapGetUserById();

app.MapGet("/", () => "API funcionando!");

app.Run();