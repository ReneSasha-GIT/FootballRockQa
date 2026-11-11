# FootballRockQa — Automated Testing Framework

[![Test Automation CI](https://github.com/Renesasha-GIT/FootballRockQa/actions/workflows/ci.yml/badge.svg)](https://github.com/Renesasha-GIT/FootballRockQa/actions/workflows/ci.yml)
[![Allure Report](https://img.shields.io/badge/Allure_Report-Live_on_Pages-29c6f7?style=flat&logo=allure)](https://renesasha-git.github.io/FootballRockQa/)
![.NET 10](https://img.shields.io/badge/.NET-10.0-512bd4?logo=dotnet)
![Selenium](https://img.shields.io/badge/Selenium-WebDriver-43B02A?logo=selenium)
![NUnit](https://img.shields.io/badge/NUnit-3-1383C6?logo=nunit)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

Multi-tier automated test solution covering **API contracts & business logic**, **UI web automation (Selenium)**, and **End-to-End Cross-Check validation (UI vs API)**, integrated with GitHub Actions CI and interactive Allure reports.

---

## 📊 Interactive Test Report

Allure generates a consolidated dashboard across both API and UI test runs on every commit:
👉 **[View Live Allure Report on GitHub Pages](https://renesasha-git.github.io/FootballRockQa/)**

* 76 test cases with 100% pass rate
* History and trend tracking across builds
* Automated screenshot capture on UI failure

---

## 🏗️ Architecture & Solution Layout

The solution follows clean testing architecture principles with separation of concerns:

```text
FootballRockQa/
├── .github/
│   └── workflows/
│       └── ci.yml                 # GitHub Actions pipeline (build, test, Allure publish)
├── src/
│   ├── Core/                      # Shared business domain & infrastructure
│   │   ├── Clients/               # FootballApiClient (RestSharp HTTP client)
│   │   ├── Helpers/               # FixtureHelper, TeamNameNormalizer
│   │   ├── Models/                # Strongly-typed DTOs for API and UI tables
│   │   └── TestConfig.cs          # Configuration provider (appsettings + env vars)
│   ├── Api.Tests/                 # API testing layer
│   │   ├── Fixtures/              # Cached responses (offline rate-limit protection)
│   │   ├── StandingsApiBusinessRulesTests.cs # Mathematical & standings integrity checks
│   │   └── StandingsApiLiveTests.cs          # Live HTTP contract & negative tests
│   └── Ui.Tests/                  # UI automation & cross-check layer
│       ├── Helpers/               # TableHelper (HTML parser for wikitable syntax)
│       ├── PageObjects/           # WikipediaStandingsPage (Page Object Model)
│       ├── BaseUiTest.cs          # Selenium driver lifecycle, headless config, screenshots
│       ├── WikipediaStandingsTests.cs        # UI Smoke & contract checks
│       └── StandingsCrossValidationTests.cs  # Cross-layer reconciliation (UI vs API)
└── README.md


## 🧪 Test Coverage Breakdown

### 1. API Testing (`Api.Tests`)
* **Contract & HTTP Verifications:**
  * Status code validations (`200 OK`, `400 Bad Request` for unknown competitions, `403 Forbidden` for unauthorized requests).
  * Payload structural integrity checks (presence of required fields, non-empty arrays, ID ranges).
* **Data-Driven Business Logic (`TestCaseSource`):**
  * **Points Calculation:** Points strictly match $(Won \times 3) + Draw$.
  * **Match Balance:** Played games count equals $Won + Draw + Lost$.
  * **Goal Difference:** Goal difference equals $GoalsFor - GoalsAgainst$.
  * **Standings Integrity:** Positions are sequential ($1 \dots 20$) and sorted strictly by points in descending order.
* **Resilience & Rate-Limiting Strategy:**
  * Uses cached JSON fixtures (`standings_pl.json`) for offline execution, preserving 3rd-party API rate quotas during CI runs.

### 2. UI Automation (`Ui.Tests`)
* **Page Object Model (POM):**
  * Encapsulates selectors, state transitions, and wait mechanisms (`WebDriverWait`) for the Wikipedia season table.
* **Resilient Table Parsing (`TableHelper`):**
  * Parses complex dynamic wikitables into typed C# DTOs.
  * Cleans Wikipedia citation footnotes (e.g., `[a]`, `[b]`) using regex.
  * Normalizes Unicode minus characters (`−` vs `-`) for robust numeric parsing.
* **Diagnostics & CI Reliability:**
  * Automatic screenshot capture on test failure integrated directly into Allure test attachments.
  * Headless Chrome configuration tuned for containerized Linux runners (`--headless=new`, `--no-sandbox`, `--disable-dev-shm-usage`).

### 3. Cross-Layer Data Verification (UI vs API)
* **Cross-Source Consistency:**
  * Reconciles data scraped live from Wikipedia against the official Football-Data.org API.
* **Entity Normalization (`TeamNameNormalizer`):**
  * Resolves naming discrepancies between data providers (e.g., `Brighton & Hove Albion FC` vs `Brighton & Hove Albion`, `Tottenham Hotspur` vs `Tottenham`).
* **Assertion Diffing:**
  * Validates that teams, points, played games, and goal differences correlate 1:1 across layers, providing clear mismatch logs on failure.
