using Microsoft.Extensions.Configuration;

namespace Core;

public static class TestConfig
{
    private static readonly IConfigurationRoot Root = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    public static string FootballDataBaseUrl =>
        Root["FootballData:BaseUrl"] ?? "https://api.football-data.org/v4";

    public static string FootballDataApiKey =>
        Root["FootballData:ApiKey"]
        ?? throw new InvalidOperationException(
            "FootballData:ApiKey is not set. Add it to appsettings.json or set the FootballData__ApiKey environment variable.");
}