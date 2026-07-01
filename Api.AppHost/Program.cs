using Api.Infrastructure;
using Api.Features.Usuarios;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de base de datos
if (builder.Environment.EnvironmentName == "Testing")
{
    // Usamos LocalDB para pruebas de integración estables
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ApiTestingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
}
else
{
    // Configuración para producción/desarrollo normal
    builder.AddSqlServerDbContext<AppDbContext>("sqldata");
}

// Registro de servicios
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Mapeo de Endpoints
// Nota: MapGetUserById y MapListarUsuarios son métodos de extensión definidos en sus respectivos archivos
app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) => 
    Results.Ok(await sender.Send(comando)));

app.MapListarUsuarios();
app.MapGetUserById();

app.MapGet("/", () => "API funcionando!");

app.Run();