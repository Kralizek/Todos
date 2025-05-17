using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace EndToEnd.Tests;

[TestFixture]
public class SetupTests
{
    [Test]
    [AutoDataProvider]
    public async Task BackendHealthCheck(DistributedApplication app)
    {
        var http = app.CreateHttpClient("api");

        var response = await http.GetAsync("_health");
        
        Assert.That(response.IsSuccessStatusCode, Is.True);

        var content = await response.Content.ReadAsStringAsync();

        await TestContext.Out.WriteLineAsync(content);
    }
    
    [Test]
    [AutoDataProvider]
    public async Task FrontendHomePage(DistributedApplication app)
    {
        var http = app.CreateHttpClient("web");

        var response = await http.GetAsync("/");
        
        Assert.That(response.IsSuccessStatusCode, Is.True);

        var content = await response.Content.ReadAsStringAsync();

        await TestContext.Out.WriteLineAsync(content);
    }
}