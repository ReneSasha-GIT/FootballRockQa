using Core.Models.Football;
using RestSharp;

namespace Core.Clients;

public class FootballApiClient
{
    private readonly RestClient _client;

    public FootballApiClient()
    {
        var options = new RestClientOptions(TestConfig.FootballDataBaseUrl);
        _client = new RestClient(options);

        var token = TestConfig.FootballDataApiKey;
        _client.AddDefaultHeader("X-Auth-Token", token);
    }

    /// <summary>
    /// Retrieves the competition standings by league code (e.g., "PL", "BL1", "SA")
    /// and deserializes the response into a StandingsResponse model.
    /// </summary>
    public async Task<RestResponse<StandingsResponse>> GetStandingsAsync(string competitionCode)
    {
        var request = new RestRequest($"competitions/{competitionCode}/standings");
        return await _client.ExecuteAsync<StandingsResponse>(request);
    }

    /// <summary>
    /// Executes a raw HTTP request without deserialization.
    /// Useful for negative tests (401, 404, etc.) where the response body is either different or not needed.
    /// </summary>
    public async Task<RestResponse> ExecuteRawAsync(RestRequest request)
    {
        return await _client.ExecuteAsync(request);
    }
}