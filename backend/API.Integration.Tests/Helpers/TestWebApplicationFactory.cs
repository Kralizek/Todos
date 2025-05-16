using DotNet.Testcontainers.Builders;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using Testcontainers.PostgreSql;

namespace API.Integration.Tests.Helpers;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly PostgreSqlContainer DatabaseContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("LOG:  database system is ready to accept connections"))
        .Build();
    
    public string ConnectionString => $"{DatabaseContainer.GetConnectionString()};Include Error Detail=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DatabaseContainer.StartAsync().GetAwaiter().GetResult();

        builder.UseSetting("ConnectionStrings:Database", ConnectionString);
        
        builder.ConfigureLogging((context, logging) => logging.ClearProviders());
    }

    public async ValueTask StopAsync()
    {
        await DatabaseContainer.StopAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await StopAsync();
        await DatabaseContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}