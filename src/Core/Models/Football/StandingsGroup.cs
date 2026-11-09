using System.Text.Json.Serialization;

namespace Core.Models.Football;

public class StandingsGroup
{
    [JsonPropertyName("stage")]
    public string Stage { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("table")]
    public List<TableItem> Table { get; set; } = new();
}