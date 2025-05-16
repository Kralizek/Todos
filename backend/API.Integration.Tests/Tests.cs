using API.Integration.Tests.Helpers;

using DotNet.Testcontainers.Containers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

using Npgsql;

using Respawn;

using Todos;

namespace API.Integration.Tests;

[SetUpFixture]
public class Tests
{
    private static readonly Lazy<TestWebApplicationFactory> WebApplicationFactoryFactory = new(() => new TestWebApplicationFactory(), LazyThreadSafetyMode.ExecutionAndPublication);
    
    public static TestWebApplicationFactory WebApplicationFactory => WebApplicationFactoryFactory.Value;

    private static Respawner _respawner = null!;
    
    public static readonly Func<DbContextOptions<AppDbContext>> OptionsBuilder = () =>
    {
        if (WebApplicationFactory.DatabaseContainer.State != TestcontainersStates.Running)
        {
            WebApplicationFactory.DatabaseContainer.StartAsync().Wait();
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
            .UseNpgsql(WebApplicationFactory.ConnectionString);

        return optionsBuilder.Options;
    };
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _ = WebApplicationFactory.CreateDefaultClient();

        await using var connection = new NpgsqlConnection(WebApplicationFactory.ConnectionString);

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (WebApplicationFactoryFactory.IsValueCreated)
        {
            await WebApplicationFactory.DisposeAsync();
        }
    }

    public static async Task ResetDatabase()
    {
        await using var connection = new NpgsqlConnection(WebApplicationFactory.ConnectionString);

        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
    }
}

[TestFixture]
public abstract class TestsWithBackend
{
    [TearDown]
    public async Task TearDown()
    {
        await Tests.ResetDatabase();
    }
}