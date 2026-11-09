using System.Text.Json.Serialization;

namespace Core.Models.Football;

public class TableItem
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("team")]
    public Team Team { get; set; } = new();

    [JsonPropertyName("playedGames")]
    public int PlayedGames { get; set; }

    [JsonPropertyName("won")]
    public int Won { get; set; }

    [JsonPropertyName("draw")]
    public int Draw { get; set; }

    [JsonPropertyName("lost")]
    public int Lost { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("goalsFor")]
    public int GoalsFor { get; set; }

    [JsonPropertyName("goalsAgainst")]
    public int GoalsAgainst { get; set; }

    [JsonPropertyName("goalDifference")]
    public int GoalDifference { get; set; }
}