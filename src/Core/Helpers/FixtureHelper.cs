using System.Text.Json;
using Core.Models.Football;

namespace Core.Helpers;

public static class FixtureHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    /// <summary>
    /// Loads and deserializes a JSON fixture file containing standings data.
    /// </summary>
    /// <param name="filePath">Relative or absolute path to the fixture file.</param>
    /// <returns>Deserialized StandingsResponse object.</returns>
    public static StandingsResponse LoadStandings(string filePath)
    {
        var fullPath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(AppContext.BaseDirectory, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Fixture file not found at path: {fullPath}");
        }

        var json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<StandingsResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize fixture from: {fullPath}");
    }

    /// <summary>
    /// Saves the received object or raw JSON to a file fixture.
    /// </summary>
    public static void SaveStandingsFixture(string filePath, StandingsResponse data)
    {
        var fullPath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(AppContext.BaseDirectory, filePath);

        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(fullPath, json);
    }
}