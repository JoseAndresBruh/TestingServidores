using Api.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore; // <-- Esta línea soluciona el error CS1061
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;

namespace Api.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory = new();
    
    // El "default!" elimina los warnings CS8618
    protected HttpClient Client = default!;
    protected AppDbContext DbContext = default!;
    private Respawner _respawner = default!;

    public async Task InitializeAsync()
    {
        Client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Inicializar Respawn para limpiar la DB
        _respawner = await Respawner.CreateAsync(DbContext.Database.GetDbConnection(), new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(DbContext.Database.GetDbConnection());

    public Task DisposeAsync() => Task.CompletedTask;
}