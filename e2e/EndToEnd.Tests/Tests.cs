using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace EndToEnd.Tests;

[SetUpFixture]
public class Tests
{
    public static readonly Lazy<Task<DistributedApplication>> ApplicationFactory = new(() => Task.Factory.StartNew(CreateDistributedApplication).Unwrap(), LazyThreadSafetyMode.ExecutionAndPublication);
    
    public static DistributedApplication Application => ApplicationFactory.Value.GetAwaiter().GetResult();

    private static async Task<DistributedApplication> CreateDistributedApplication()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();

        var application = await builder.BuildAsync();
        
        await application.StartAsync();
        
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        
        await application.ResourceNotifications.WaitForResourceHealthyAsync("web", cts.Token);

        return application;
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (ApplicationFactory.IsValueCreated)
        {
            await Application.StopAsync();
            await Application.DisposeAsync();
        }
    }
}
