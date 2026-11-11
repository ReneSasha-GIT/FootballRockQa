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
