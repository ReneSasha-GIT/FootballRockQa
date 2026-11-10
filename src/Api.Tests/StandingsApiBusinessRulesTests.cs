using Core.Helpers;
using Core.Models.Football;
using Allure.NUnit;

namespace Api.Tests;

[TestFixture]
[AllureNUnit]
[Category("Api")]
[Category("Offline")]
public class StandingsApiBusinessRulesTests
{
    private static StandingsResponse _cachedStandings = null!;
    private static List<TableItem> _tableRows = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _cachedStandings = FixtureHelper.LoadStandings(Path.Combine("Fixtures", "standings_pl.json"));
        _tableRows = _cachedStandings.Standings
            .First(s => s.Type == "TOTAL")
            .Table;
    }

    public static IEnumerable<TestCaseData> GetTableRows()
    {
        var standings = FixtureHelper.LoadStandings(Path.Combine("Fixtures", "standings_pl.json"));
        var rows = standings.Standings.First(s => s.Type == "TOTAL").Table;

        foreach (var row in rows)
        {
            yield return new TestCaseData(row)
                .SetName($"RowValidation_Pos_{row.Position}_{row.Team.ShortName.Replace(" ", "_")}");
        }
    }

    [Test, TestCaseSource(nameof(GetTableRows))]
    public void TeamRow_PointsCalculation_MatchesFormula(TableItem row)
    {
        // points = 3 * won + draw
        var expectedPoints = (row.Won * 3) + row.Draw;

        Assert.That(row.Points, Is.EqualTo(expectedPoints),
            $"Points calculation mismatch for {row.Team.Name}. Won: {row.Won}, Draw: {row.Draw}");
    }

    [Test, TestCaseSource(nameof(GetTableRows))]
    public void TeamRow_PlayedGames_MatchesSumOfOutcomes(TableItem row)
    {
        // played = won + draw + lost
        var expectedPlayed = row.Won + row.Draw + row.Lost;

        Assert.That(row.PlayedGames, Is.EqualTo(expectedPlayed),
            $"Played games mismatch for {row.Team.Name}");
    }

    [Test, TestCaseSource(nameof(GetTableRows))]
    public void TeamRow_GoalDifference_MatchesGoalsForMinusAgainst(TableItem row)
    {
        // goalDifference = goalsFor - goalsAgainst
        var expectedDifference = row.GoalsFor - row.GoalsAgainst;

        Assert.That(row.GoalDifference, Is.EqualTo(expectedDifference),
            $"Goal difference mismatch for {row.Team.Name}");
    }

    [Test]
    public void StandingsTable_Positions_AreSequentialStartingFromOne()
    {
        var positions = _tableRows.Select(r => r.Position).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(positions, Is.Not.Empty);
            Assert.That(positions.First(), Is.EqualTo(1));
            
            for (var i = 0; i < positions.Count; i++)
            {
                Assert.That(positions[i], Is.EqualTo(i + 1), $"Position at index {i} is out of sequence.");
            }
        });
    }

    [Test]
    public void StandingsTable_IsOrderedByPointsDescending()
    {
        var points = _tableRows.Select(r => r.Points).ToList();
        
        Assert.That(points, Is.Ordered.Descending, 
            "The standings table is not properly sorted by points.");
    }

    [Test]
    public void StandingsTable_ContainsExpectedNumberOfTeams()
    {
        // Premier League contains 20 teams
        Assert.That(_tableRows, Has.Count.EqualTo(20));
    }
}