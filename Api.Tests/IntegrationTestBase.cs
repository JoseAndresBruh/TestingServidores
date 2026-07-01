using Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
        // Forzamos el entorno "Testing" para que Program.cs lo detecte inmediatamente
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });

        Client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await DbContext.Database.EnsureCreatedAsync();

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