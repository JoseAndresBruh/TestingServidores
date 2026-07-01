using Api.Infrastructure;
using Api.Features.Usuarios;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ApiTestingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
}
else
{
    builder.AddSqlServerDbContext<AppDbContext>("sqldata");
}

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Registro explícito de endpoints
app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) => 
    Results.Ok(await sender.Send(comando)));

app.MapGet("/usuarios", async (int? page, int? pageSize, ISender sender) => 
    Results.Ok(await sender.Send(new ListarUsuariosQuery(page ?? 1, pageSize ?? 10))));

app.MapGetUserById(); // Asegúrate de que esto mapea a /usuarios/{id}

app.MapGet("/", () => "API funcionando!");

app.Run();