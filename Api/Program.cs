using Api.Infrastructure;
using Api.Features.Usuarios;
using Api.Application.Behaviors;
using MediatR;
using FluentValidation;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de base de datos condicional para pruebas de integración
if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContextPool<AppDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ApiTestingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
}
else
{
    builder.AddSqlServerDbContext<AppDbContext>("sqldata");
}

// Registro de MediatR con Pipeline Behaviors mapeados a tu nueva carpeta
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Mapeo de Endpoints del CRUD
app.MapPost("/usuarios", async (CrearUsuarioComando comando, ISender sender) => 
    Results.Ok(await sender.Send(comando)));

app.MapListarUsuarios();
app.MapGetUserById();
app.MapUpdateUser();
app.MapDeleteUser();

app.MapGet("/", () => "API funcionando!");

app.Run();