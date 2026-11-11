using System.Text.RegularExpressions;

namespace Core.Helpers;

public static class TeamNameNormalizer
{
    private static readonly Dictionary<string, string> CustomAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Brighton & Hove Albion", "Brighton Hove" },
        { "Brighton & Hove Albion FC", "Brighton Hove" },
        { "Tottenham Hotspur", "Tottenham" },
        { "Tottenham Hotspur FC", "Tottenham" },
        { "Manchester United", "Man United" },
        { "Manchester United FC", "Man United" },
        { "Manchester City", "Man City" },
        { "Manchester City FC", "Man City" },
        { "AFC Bournemouth", "Bournemouth" },
        { "Nottingham Forest", "Nottingham" },
        { "Nottingham Forest FC", "Nottingham" },
        { "Wolverhampton Wanderers", "Wolves" },
        { "Wolverhampton Wanderers FC", "Wolves" }
    };

    public static string Normalize(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return string.Empty;
        }

        var cleaned = rawName.Trim();

        if (CustomAliases.TryGetValue(cleaned, out var alias))
        {
            return alias;
        }

        cleaned = Regex.Replace(cleaned, @"\b(FC|AFC)\b", "", RegexOptions.IgnoreCase).Trim();
        cleaned = Regex.Replace(cleaned, @"\s+", " ");

        return cleaned;
    }
}