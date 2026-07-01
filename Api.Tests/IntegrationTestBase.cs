using Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;

namespace Api.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = default!;
    protected HttpClient Client = default!;
    protected AppDbContext DbContext = default!;
    private Respawner _respawner = default!;
    
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        var connectionString = _msSqlContainer.GetConnectionString();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);
                
                services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(connectionString));
            });
        });

        Client = _factory.CreateClient();
        
        using var scope = _factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Aseguramos la base de datos
        await DbContext.Database.EnsureCreatedAsync();

        // AQUÍ ESTÁ LA CORRECCIÓN: Abrir la conexión antes de pasarla a Respawn
        var dbConnection = DbContext.Database.GetDbConnection();
        await dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async Task ResetDatabaseAsync() 
    {
        var dbConnection = DbContext.Database.GetDbConnection();
        if (dbConnection.State != System.Data.ConnectionState.Open) await dbConnection.OpenAsync();
        await _respawner.ResetAsync(dbConnection);
    }

    public async Task DisposeAsync() 
    {
        _factory?.Dispose();
        await _msSqlContainer.DisposeAsync();
    }
}