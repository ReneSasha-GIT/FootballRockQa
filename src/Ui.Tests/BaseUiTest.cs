using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

[TestFixture]
[Category("Ui")]
public abstract class BaseUiTest
{
    protected IWebDriver Driver { get; private set; } = null!;
    protected WebDriverWait Wait { get; private set; } = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--window-size=1920,1080");

        Driver = new ChromeDriver(options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            var fileName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Screenshots", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            screenshot.SaveAsFile(filePath);
            TestContext.AddTestAttachment(filePath, "Failure Screenshot");
            System.Console.WriteLine(Path.GetFullPath(fileName));
        }
        Driver.Quit();
        Driver.Dispose();
    }
}