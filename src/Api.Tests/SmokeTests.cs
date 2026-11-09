using Core;

namespace Api.Tests;

public class SmokeTests
{
    [Test]
    public void Setup_IsWorking()
    {
        Assert.That(2 + 2, Is.EqualTo(4));
    }

    [Test]
    public void Config_ApiKey_IsLoaded()
    {
        Assert.That(TestConfig.FootballDataApiKey, Is.Not.Empty);
    }
}