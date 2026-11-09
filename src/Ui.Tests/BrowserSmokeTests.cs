using OpenQA.Selenium.Chrome;

namespace Ui.Tests;

public class BrowserSmokeTests
{
    [Test]
    public void Chrome_CanOpenWikipedia()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");

        using var driver = new ChromeDriver(options);
        driver.Navigate().GoToUrl("https://en.wikipedia.org");

        Assert.That(driver.Title, Does.Contain("Wikipedia"));
    }
}