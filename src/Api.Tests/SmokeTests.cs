namespace Api.Tests;

public class SmokeTests
{
    [Test]
    public void Setup_IsWorking()
    {
        Assert.That(2 + 2, Is.EqualTo(4));;
    }
}