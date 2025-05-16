using System.Text.Json;

namespace API.Integration.Tests;

[TestFixture]
public class SetupTests
{
    [Test]
    [InlineAutoDataProvider("/_health")]
    [InlineAutoDataProvider("/_health/all")]
    public async Task Health_checks_are_green(string path, HttpClient http)
    {
        using var response = await http.GetAsync(path);

        var content = await response.Content.ReadAsStringAsync();

        await TestContext.Out.WriteLineAsync(content);

        Assert.That(response.IsSuccessStatusCode, Is.True);
    }
    
    [Test]
    [InlineAutoDataProvider("/schema/v1/openapi.json")]
    public async Task OpenAPI_schema_is_valid(string path, HttpClient http)
    {
        using var response = await http.GetAsync(path);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed request");

            Assert.That(content, Is.Not.Empty, "Empty content");

            Assert.That(() => JsonDocument.Parse(content), Throws.Nothing, "Invalid JSON");
        });
    }
}