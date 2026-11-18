# FootballRockQa Architecture

## 1. Project Architecture

FootballRockQa is a .NET 10 NUnit solution organized as three projects:

```text
FootballRockQa.slnx
|
|-- src/Core/Core.csproj
|   |-- Clients/FootballApiClient.cs
|   |-- Helpers/FixtureHelper.cs
|   |-- Helpers/TeamNameNormalizer.cs
|   |-- Models/Football/
|   |-- Models/Ui/
|   `-- TestConfig.cs
|
|-- src/Api.Tests/Api.Tests.csproj
|   |-- SmokeTests.cs
|   |-- StandingsApiLiveTests.cs
|   |-- StandingsApiBusinessRulesTests.cs
|   `-- Fixtures/standings_pl.json
|
`-- src/Ui.Tests/Ui.Tests.csproj
    |-- BaseUiTest.cs
    |-- BrowserSmokeTests.cs
    |-- WikipediaStandingsTests.cs
    |-- StandingsCrossValidationTests.cs
    |-- PageObjects/WikipediaStandingsPage.cs
    `-- Helpers/TableHelper.cs
```

The dependency direction is:

```text
Api.Tests ----> Core
Ui.Tests -----> Core
Api.Tests and Ui.Tests do not reference each other
```

Both test projects reference `src/Core/Core.csproj` from their project files. Core does not reference either test project.

## 2. Project Responsibilities

### Core

Core contains shared configuration, HTTP infrastructure, models, and helpers.

- `TestConfig` loads `FootballData:BaseUrl` and `FootballData:ApiKey` from JSON configuration and environment variables.
- `FootballApiClient` creates a RestSharp client, adds the API token header, and executes standings requests.
- `FixtureHelper` resolves fixture paths, reads JSON, and deserializes it into `StandingsResponse`.
- `TeamNameNormalizer` normalizes team names used in API/UI comparisons.
- Football models represent API standings payloads.
- `UiTableRow` represents a parsed row from the Wikipedia standings table.

Core is a library project targeting `net10.0`. It depends on RestSharp and Microsoft.Extensions.Configuration packages.

### Api.Tests

Api.Tests validates Football-Data.org and the standings data rules.

It contains:

- `StandingsApiLiveTests` for live HTTP status and payload contract checks.
- `StandingsApiBusinessRulesTests` for offline standings calculations and integrity checks.
- `SmokeTests` for basic runtime and configuration smoke checks.
- A cached `Fixtures/standings_pl.json` response used by offline tests and cross-validation.

The project depends on Core and uses NUnit, Allure.NUnit, and the .NET test SDK.

### Ui.Tests

Ui.Tests uses Selenium WebDriver to automate Wikipedia and compare its standings table with cached API standings.

It contains:

- `BaseUiTest` for Chrome setup, waits, teardown, and failure screenshots.
- `WikipediaStandingsPage`, a Page Object Model for the Wikipedia page and standings table.
- `TableHelper`, which parses Selenium table elements into `UiTableRow` objects.
- `WikipediaStandingsTests` for page-level UI checks.
- `StandingsCrossValidationTests` for API/UI club and metric reconciliation.
- `BrowserSmokeTests` for a direct Selenium browser smoke check.

The project depends on Core and uses Selenium, NUnit, Allure.NUnit, and the .NET test SDK.

## 3. Main Dependencies

### Project dependencies

- `Api.Tests -> Core`
- `Ui.Tests -> Core`
- Both projects target `net10.0`.

### Shared Core classes

| Class | Main consumers |
|---|---|
| `TestConfig` | `FootballApiClient` and API smoke tests |
| `FootballApiClient` | `StandingsApiLiveTests` |
| `FixtureHelper` | Business-rule tests and UI/API cross-validation |
| `StandingsResponse` | Live API deserialization, fixture loading, and cross-validation |
| `StandingsGroup` | Cached standings payload structure |
| `TableItem` | API business-rule tests and cross-validation |
| `UiTableRow` | Selenium table parsing and cross-validation |
| `TeamNameNormalizer` | UI/API club-name reconciliation |

### Shared UI classes

| Class | Main consumers |
|---|---|
| `BaseUiTest` | `WikipediaStandingsTests` and `StandingsCrossValidationTests` |
| `WikipediaStandingsPage` | `WikipediaStandingsTests` and `StandingsCrossValidationTests` |
| `TableHelper` | `WikipediaStandingsPage` |

The biggest modification blast radius is concentrated in the football standings models and UI lifecycle classes:

- Changes to `StandingsResponse`, `StandingsGroup`, `TableItem`, or `Team` can affect deserialization, live tests, offline tests, and cross-validation.
- Changes to `BaseUiTest` affect all tests that inherit it.
- Changes to `WikipediaStandingsPage` can affect both Wikipedia UI test classes.

## 4. API Testing Architecture

### Live API tests

`StandingsApiLiveTests` creates `FootballApiClient` in its NUnit `[SetUp]`.

The client:

1. Reads the base URL and API key through `TestConfig`.
2. Creates a RestSharp `RestClient`.
3. Adds `X-Auth-Token`.
4. Requests `competitions/{code}/standings`.

The live tests verify:

- Premier League requests return `200 OK`.
- `StandingsResponse.Data` is populated.
- A `TOTAL` standings group contains table rows.
- The first team has an ID and name.
- Unknown competition codes return `400 Bad Request`.
- Requests without an API token return `403 Forbidden`.

The live test class has `Api` and `Live` NUnit categories.

### Offline business-rule tests

`StandingsApiBusinessRulesTests` loads `Fixtures/standings_pl.json` through `FixtureHelper` in `[OneTimeSetUp]`.

It validates:

- `points = (won * 3) + draw`
- `playedGames = won + draw + lost`
- `goalDifference = goalsFor - goalsAgainst`
- Positions start at 1 and are sequential.
- Table rows are sorted by points in descending order.
- The table contains 20 teams.

The row-level tests use `TestCaseSource` and `TableItem` instances loaded from the cached fixture.

The class has `Api` and `Offline` NUnit categories.

### Cached fixtures

The offline tests read a copied fixture from `Fixtures/standings_pl.json`. The path is resolved from the test application's base directory by `FixtureHelper`.

The fixture is copied to output by `Api.Tests.csproj`; it is also copied by `Ui.Tests.csproj` because `Ui.Tests` loads the same fixture for cross-validation.

## 5. UI Testing Architecture

### Browser lifecycle

`BaseUiTest` is an abstract NUnit fixture with the `Ui` category.

Its `[SetUp]` creates headless Chrome with:

- `--headless=new`
- `--no-sandbox`
- `--disable-dev-shm-usage`
- `--window-size=1920,1080`

Its `[TearDown]` creates a screenshot in `Screenshots/` for failed tests, adds the screenshot as an Allure attachment, and quits/disposes the driver.

### Page Object Model

`WikipediaStandingsPage` owns:

- The Wikipedia standings page URL.
- The page title selector.
- The standings-table XPath.
- Wait and stale-element handling.
- Calls to `TableHelper.ParseStandingsTable`.

The page object returns `List<UiTableRow>` rather than exposing Selenium elements to test classes.

### Table parsing

`TableHelper.ParseStandingsTable` converts table rows into `UiTableRow` values. It uses `TableHelper.CleanCellText` for parsed text. `UiTableRow` fields represent position, team name, URL, and standings metrics.

### Cross-layer validation

`StandingsCrossValidationTests` loads:

- API standings through `FixtureHelper.LoadStandings`.
- UI standings through `WikipediaStandingsPage.GetStandings`.

It then:

- Compares normalized club names.
- Matches names between `TableItem.Team.Name` and `UiTableRow.TeamName`.
- Compares points, played games, and goal difference.

The class has `Ui` and `CrossCheck` categories and inherits `BaseUiTest`.

## 6. Test Execution Flow

### Local execution

Run all discovered tests from the solution root:

```bash
dotnet test
```

Run a category:

```bash
dotnet test --filter "TestCategory=Api"
dotnet test --filter "TestCategory=Ui"
dotnet test --filter "TestCategory=CrossCheck"
```

Live API tests require `FootballData__ApiKey` or a local API key configuration.

### CI execution

GitHub Actions in `.github/workflows/ci.yml` runs API and UI jobs in separate Ubuntu runners.

API job:

1. Checkout.
2. Setup .NET 10.
3. Restore.
4. Build Release.
5. Run `dotnet test --filter "TestCategory=Api"`.
6. Upload Allure results.

UI job:

1. Checkout.
2. Setup .NET 10.
3. Restore.
4. Build Release.
5. Run `dotnet test --filter "TestCategory=Ui"`.
6. Upload Allure results and screenshots.

Because the filter selects NUnit categories, a test must belong to the requested category or be inherited from a categorized base fixture. Cross-validation belongs to `Ui` and `CrossCheck`; API live and offline fixtures belong to `Api`.

A report job collects Allure artifacts, consolidates results, retains history, and publishes the report to GitHub Pages.

## 7. How to Add a New API Test

Choose the appropriate test style.

### Add a live API test

1. Open `src/Api.Tests/StandingsApiLiveTests.cs` or add a new fixture under `src/Api.Tests`.
2. Use `_apiClient` when extending the existing live fixture.
3. Add a public async NUnit `[Test]` method.
4. Call `FootballApiClient.GetStandingsAsync` or `ExecuteRawAsync`.
5. Assert status codes and payload fields with NUnit.
6. Use `[Category("Api")]` and, when appropriate, `[Category("Live")]`.
7. Reuse `Core.Models.Football` types instead of defining duplicate response DTOs.

Example pattern:

```csharp
[Test]
public async Task GetStandings_ReturnsContract()
{
    var response = await _apiClient.GetStandingsAsync("PL");

    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    Assert.That(response.Data, Is.Not.Null);
}
```

New live fixtures can create `FootballApiClient` in `[SetUp]`, following the existing fixture pattern.

### Add an offline API business-rule test

1. Open `src/Api.Tests/StandingsApiBusinessRulesTests.cs` or add a fixture under `src/Api.Tests`.
2. Load the fixture with `FixtureHelper.LoadStandings`.
3. Select `Standings.First(s => s.Type == "TOTAL").Table`.
4. Use `TableItem` in assertions.
5. Use a `TestCaseSource` for checks that apply to every table row.
6. Use `[Category("Api")]` and `[Category("Offline")]`.

Adding a new field or changing fixture JSON may require updating `TableItem` or other Core football models.

## 8. How to Add a New UI Test

1. Add the test class under `src/Ui.Tests`.
2. Inherit `BaseUiTest` when the test needs Chrome, WebDriverWait, screenshots, or teardown.
3. Add `[TestFixture]` and `[Category("Ui")]`.
4. Create or reuse a Page Object in `src/Ui.Tests/PageObjects`.
5. Expose page behavior as methods that return strings, booleans, or typed `UiTableRow` values.
6. Put Selenium element parsing into `TableHelper` or a new helper only when it is reused or keeps test logic clear.
7. Add NUnit `[Test]` methods and use `Assert` or `Assert.Multiple`.

The existing Page Object setup pattern is:

```csharp
[SetUp]
public void PageInit()
{
    _page = new WikipediaStandingsPage(Driver, Wait);
    _page.Open();
}

[Test]
public void Page_Header_IsDisplayed()
{
    Assert.That(_page.GetPageTitle(), Is.Not.Empty);
}
```

For cross-layer UI/API checks:

1. Load API data through `FixtureHelper`.
2. Load UI rows through `WikipediaStandingsPage.GetStandings`.
3. Convert names through `TeamNameNormalizer.Normalize` when team-name spelling differs.
4. Compare typed values in `StandingsCrossValidationTests` or a clearly related test class.
