using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

using Todos;

namespace API.Integration.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class AutoDataProviderAttribute() : AutoDataAttribute(() => FixtureHelpers.CreateFixture());

public interface IFixtureConfigurator
{
    void ConfigureFixture(IFixture fixture);
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AutoDataProviderAttribute<T>() : AutoDataAttribute(() => FixtureHelpers.CreateFixture(new T()))
    where T : IFixtureConfigurator, new();

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoDataProviderAttribute(params object[] args) : InlineAutoDataAttribute(() => FixtureHelpers.CreateFixture(), args);

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoDataProviderAttribute<T>(params object[] args) : InlineAutoDataAttribute(() => FixtureHelpers.CreateFixture(new T()), args)
    where T : IFixtureConfigurator, new();

internal static class FixtureHelpers
{
    public static IFixture CreateFixture(params IFixtureConfigurator[] configurators)
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoFakeItEasyCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        fixture.Register((HttpClient http) => new AppDbContext(Tests.OptionsBuilder()));

        fixture.AddWebApplicationFactorySupport(Tests.WebApplicationFactory);

        fixture.Customizations.Add(new TypeRelay(typeof(ILogger<>), typeof(FakeLogger<>)));

        fixture.Customize<DateTimeOffset>(o => o.FromFactory((DateTime dt) => new DateTimeOffset(dt, TimeSpan.Zero)));
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        fixture.Customize<TimeOnly>(o => o.FromFactory((DateTime dt) => TimeOnly.FromDateTime(dt)));

        foreach (var configurator in configurators)
        {
            configurator.ConfigureFixture(fixture);
        }

        return fixture;
    }
}