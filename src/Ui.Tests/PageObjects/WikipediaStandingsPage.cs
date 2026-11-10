using Core.Models.Ui;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Ui.Tests.Helpers;

namespace Ui.Tests.PageObjects;

public class WikipediaStandingsPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    // Wiki page of current 2026-2027 Premier League season
    public const string PageUrl = "https://en.wikipedia.org/wiki/2026%E2%80%9327_Premier_League";

    private readonly By _pageTitle = By.Id("firstHeading");
    private readonly By _standingsTable = By.XPath("//table[contains(@class, 'wikitable')][.//th/abbr[contains(text(), 'Pos') ]]");
    private readonly By _tableRows = By.XPath(".//tbody/tr[td]");

    public WikipediaStandingsPage(IWebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public WikipediaStandingsPage Open(string url = PageUrl)
    {
        _driver.Navigate().GoToUrl(url);
        _wait.Until(driver => driver.FindElement(_pageTitle).Displayed);
        return this;
    }

    public string GetPageTitle()
    {
        return _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_pageTitle);
                return element.Displayed ? element.Text : null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        });
    }

    public bool IsStandingsTableDisplayed()
    {
        try
        {
            return _wait.Until(driver =>
            {
                try
                {
                    return driver.FindElement(_standingsTable).Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public IWebElement GetTableElement()
    {
        return _wait.Until(driver =>
        {
            try
            {
                var element = driver.FindElement(_standingsTable);
                return element.Displayed ? element : null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        });
    }

    public List<UiTableRow> GetStandings()
    {
        var table = GetTableElement();
        return TableHelper.ParseStandingsTable(table);
    }
}