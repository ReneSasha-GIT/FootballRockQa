using NUnit.Framework;
using Ui.Tests.PageObjects;
using Allure.NUnit;

namespace Ui.Tests;

[TestFixture]
[AllureNUnit]
public class WikipediaStandingsTests : BaseUiTest
{
    private WikipediaStandingsPage _page = null!;

    [SetUp]
    public void PageInit()
    {
        _page = new WikipediaStandingsPage(Driver, Wait);
        _page.Open();
    }

    [Test]
    [Description("Verifies that the page header title is displayed and contains the league name.")]
    public void LeaguePage_Title_ContainsPremierLeague()
    {
        var title = _page.GetPageTitle();
        Assert.That(title, Does.Contain("Premier League"), "Page title should contain 'Premier League'.");
    }

    [Test]
    [Description("Verifies that the standings table is loaded and present on the page.")]
    public void StandingsTable_ShouldBeDisplayedOnPage()
    {
        var isDisplayed = _page.IsStandingsTableDisplayed();
        Assert.That(isDisplayed, Is.True, "Standings table should be displayed on the league page.");
    }

    [Test]
    [Description("Verifies that the parsed standings table contains exactly 20 teams.")]
    public void StandingsTable_ContainsTwentyTeams()
    {
        var tableData = _page.GetStandings();
        Assert.That(tableData, Has.Count.EqualTo(20), "The English Premier League table must contain 20 teams.");
    }

    [Test]
    [Description("Verifies that parsed numeric fields (position, points, matches played) fall within valid ranges.")]
    public void StandingsTable_PointsAndMatches_AreValidNumbers()
    {
        var tableData = _page.GetStandings();

        Assert.Multiple(() =>
        {
            foreach (var row in tableData)
            {
                Assert.That(row.Position, Is.InRange(1, 20), $"Position {row.Position} is out of expected range for {row.TeamName}.");
                Assert.That(row.Points, Is.GreaterThanOrEqualTo(0), $"Points for {row.TeamName} must not be negative.");
                Assert.That(row.Played, Is.GreaterThanOrEqualTo(0), $"Matches played for {row.TeamName} must not be negative.");
            }
        });
    }

    [Test]
    [Description("Verifies that each club row contains a valid, non-empty Wikipedia URL.")]
    public void StandingsTable_TeamLinks_LeadToWikipedia()
    {
        var tableData = _page.GetStandings();

        Assert.Multiple(() =>
        {
            foreach (var row in tableData)
            {
                Assert.That(row.TeamUrl, Is.Not.Null.And.Not.Empty, $"Missing team URL for {row.TeamName}.");
                Assert.That(row.TeamUrl, Does.StartWith("https://en.wikipedia.org/wiki/"), $"Team URL for {row.TeamName} must be a valid Wikipedia article link.");
            }
        });
    }
}