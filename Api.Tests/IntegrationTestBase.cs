using Api.Infrastructure;
using Microsoft.AspNetCore.Hosting; // <-- AQUÍ ESTÁ LA SOLUCIÓN
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;

namespace Api.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = default!;
    protected HttpClient Client = default!;
    protected AppDbContext DbContext = default!;
    private Respawner _respawner = default!;

    public async Task InitializeAsync()
    {
        // 1. Configuramos la fábrica para inyectar una cadena de conexión local para las pruebas
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    // Usamos LocalDB (incluido en Windows/Visual Studio) para pruebas rápidas
                    ["ConnectionStrings:sqldata"] = "Server=(localdb)\\mssqllocaldb;Database=ApiTestingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
                });
            });
        });

        Client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // 2. IMPORTANTE: Aseguramos que la base de datos de prueba se cree antes de limpiarla
        await DbContext.Database.EnsureCreatedAsync();

        // 3. Inicializamos Respawn
        _respawner = await Respawner.CreateAsync(DbContext.Database.GetDbConnection(), new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(DbContext.Database.GetDbConnection());

    public Task DisposeAsync() 
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }
}