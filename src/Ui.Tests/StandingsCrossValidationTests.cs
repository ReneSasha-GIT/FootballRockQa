using Allure.NUnit;
using Core.Helpers;
using Core.Models.Football;
using Core.Models.Ui;
using NUnit.Framework;
using Ui.Tests.PageObjects;

namespace Ui.Tests;

[TestFixture]
[AllureNUnit]
[Category("Ui")]
[Category("CrossCheck")]
public class StandingsCrossValidationTests : BaseUiTest
{
    private WikipediaStandingsPage _wikiPage = null!;
    private List<TableItem> _apiTable = null!;
    private List<UiTableRow> _uiTable = null!;

    [OneTimeSetUp]
    public void LoadApiData()
    {
        var apiStandings = FixtureHelper.LoadStandings(Path.Combine("Fixtures", "standings_pl.json"));
        _apiTable = apiStandings.Standings
            .First(s => s.Type == "TOTAL")
            .Table;
    }

    [SetUp]
    public void LoadUiData()
    {
        _wikiPage = new WikipediaStandingsPage(Driver, Wait);
        _wikiPage.Open();
        _uiTable = _wikiPage.GetStandings();
    }

    [Test]
    [Description("Validates that Wikipedia standings table has the exact same clubs as the API source.")]
    public void Standings_ClubsList_MatchesBetweenUiAndApi()
    {
        var uiClubs = _uiTable
            .Select(r => TeamNameNormalizer.Normalize(r.TeamName))
            .OrderBy(n => n)
            .ToList();

        var apiClubs = _apiTable
            .Select(r => TeamNameNormalizer.Normalize(r.Team.Name))
            .OrderBy(n => n)
            .ToList();

        Assert.That(uiClubs, Is.EqualTo(apiClubs), 
            "The list of teams in Wikipedia does not match the teams returned by the API.");
    }

    [Test]
    [Description("Compares key metrics (Points, Played, GoalDifference) for each team between UI and API.")]
    public void Standings_MetricsPerTeam_MatchBetweenUiAndApi()
    {
        var apiMap = _apiTable.ToDictionary(
            t => TeamNameNormalizer.Normalize(t.Team.Name),
            t => t,
            StringComparer.OrdinalIgnoreCase
        );

        Assert.Multiple(() =>
        {
            foreach (var uiRow in _uiTable)
            {
                var normalizedUiName = TeamNameNormalizer.Normalize(uiRow.TeamName);

                Assert.That(apiMap.ContainsKey(normalizedUiName), Is.True, 
                    $"Team '{uiRow.TeamName}' from UI was not found in API data.");

                if (!apiMap.TryGetValue(normalizedUiName, out var apiRow))
                {
                    continue;
                }

                Assert.That(uiRow.Points, Is.EqualTo(apiRow.Points),
                    $"Points mismatch for {uiRow.TeamName} (UI: {uiRow.Points}, API: {apiRow.Points})");

                Assert.That(uiRow.Played, Is.EqualTo(apiRow.PlayedGames),
                    $"Played games mismatch for {uiRow.TeamName} (UI: {uiRow.Played}, API: {apiRow.PlayedGames})");

                Assert.That(uiRow.GoalDifference, Is.EqualTo(apiRow.GoalDifference),
                    $"Goal difference mismatch for {uiRow.TeamName} (UI: {uiRow.GoalDifference}, API: {apiRow.GoalDifference})");
            }
        });
    }
}