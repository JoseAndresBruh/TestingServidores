using Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql; // Importamos Testcontainers

namespace Api.Tests;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = default!;
    protected HttpClient Client = default!;
    protected AppDbContext DbContext = default!;
    private Respawner _respawner = default!;
    
    // 1. Definimos un contenedor temporal de SQL Server
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        // 2. Arrancamos el contenedor y obtenemos su cadena de conexión
        await _msSqlContainer.StartAsync();
        var connectionString = _msSqlContainer.GetConnectionString();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            // 3. Reemplazamos la conexión fallida de LocalDB con nuestro contenedor
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
        
        await DbContext.Database.EnsureCreatedAsync();

        _respawner = await Respawner.CreateAsync(DbContext.Database.GetDbConnection(), new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(DbContext.Database.GetDbConnection());

    public async Task DisposeAsync() 
    {
        _factory?.Dispose();
        await _msSqlContainer.DisposeAsync(); // Apagamos y borramos el contenedor al terminar
    }
}