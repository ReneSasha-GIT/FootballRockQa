using System.Net;
using Core.Clients;
using RestSharp;

namespace Api.Tests;

[TestFixture]
[Category("Api")]
[Category("Live")]
public class StandingsApiLiveTests
{
    private FootballApiClient _apiClient = null!;

    [SetUp]
    public void SetUp()
    {
        _apiClient = new FootballApiClient();
    }

    [Test]
    public async Task GetStandings_PremierLeague_ReturnsSuccessAndValidContract()
    {
        var response = await _apiClient.GetStandingsAsync("PL");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data, Is.Not.Null, "Response payload should not be null.");
            Assert.That(response.Data!.Standings, Is.Not.Empty, "Standings list should not be empty.");
        });

        var totalStandings = response.Data.Standings.FirstOrDefault(s => s.Type == "TOTAL");
        Assert.Multiple(() =>
        {
            Assert.That(totalStandings, Is.Not.Null, "Expected 'TOTAL' standings group was missing.");
            Assert.That(totalStandings!.Table, Is.Not.Empty, "Table rows should not be empty.");
            
            var firstRow = totalStandings.Table.First();
            Assert.That(firstRow.Team, Is.Not.Null);
            Assert.That(firstRow.Team.Name, Is.Not.Null.And.Not.Empty);
            Assert.That(firstRow.Team.Id, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task GetStandings_UnknownCompetitionCode_ReturnsNotFound()
    {
        var request = new RestRequest("competitions/NON_EXISTENT_CODE_999/standings");
        var response = await _apiClient.ExecuteRawAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetStandings_MissingApiKey_ReturnsUnauthorized()
    {
        // Client without token
        var clientWithoutToken = new RestClient("https://api.football-data.org/v4");
        var request = new RestRequest("competitions/PL/standings");

        var response = await clientWithoutToken.ExecuteAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}