using System.Text.Json.Serialization;

namespace Core.Models.Football;

public class StandingsResponse
{
    [JsonPropertyName("standings")]
    public List<StandingsGroup> Standings { get; set; } = new();
}