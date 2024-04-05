using AutoFixture.AutoFakeItEasy;
using AutoFixture.NUnit3;

namespace Tests;

[AttributeUsage(AttributeTargets.Method)]
public class AutoDataProviderAttribute : AutoDataAttribute
{
    public AutoDataProviderAttribute() : base(FixtureHelpers.CreateFixture) {}
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InlineAutoDataProviderAttribute : InlineAutoDataAttribute
{
    public InlineAutoDataProviderAttribute(params object[] args) : base(() => FixtureHelpers.CreateFixture(), args)
    {
    }
}

internal static class FixtureHelpers
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoFakeItEasyCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        return fixture;
    }
}