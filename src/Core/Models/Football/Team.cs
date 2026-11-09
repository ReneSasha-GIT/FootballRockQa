using System.Text.Json.Serialization;

namespace Core.Models.Football;

public class Team
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; } = string.Empty;

    [JsonPropertyName("tla")]
    public string Tla { get; set; } = string.Empty;
}