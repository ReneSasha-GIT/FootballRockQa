using System.Text.RegularExpressions;
using Core.Models.Ui;
using OpenQA.Selenium;

namespace Ui.Tests.Helpers;

public static class TableHelper
{
    public static List<UiTableRow> ParseStandingsTable(IWebElement tableElement)
    {
        var result = new List<UiTableRow>();
        
        var rows = tableElement.FindElements(By.XPath(".//tbody/tr[td]"));

        foreach (var row in rows)
        {
            var cells = row.FindElements(By.XPath("./td | ./th"));
            
            if (cells.Count < 10)
            {
                continue;
            }

            int.TryParse(CleanCellText(cells[0].Text), out var position);

            var teamCell = cells[1];
            var teamLink = teamCell.FindElements(By.TagName("a")).FirstOrDefault();
            var teamName = CleanCellText(teamCell.Text);
            var teamUrl = teamLink?.GetAttribute("href");

            int.TryParse(CleanCellText(cells[2].Text), out var played);
            int.TryParse(CleanCellText(cells[3].Text), out var won);
            int.TryParse(CleanCellText(cells[4].Text), out var drawn);
            int.TryParse(CleanCellText(cells[5].Text), out var lost);
            int.TryParse(CleanCellText(cells[6].Text), out var goalsFor);
            int.TryParse(CleanCellText(cells[7].Text), out var goalsAgainst);

            var gdText = CleanCellText(cells[8].Text)
                .Replace("−", "-")
                .Replace("+", "");
            int.TryParse(gdText, out var goalDifference);

            int.TryParse(CleanCellText(cells[9].Text), out var points);

            result.Add(new UiTableRow
            {
                Position = position,
                TeamName = teamName,
                Played = played,
                Won = won,
                Drawn = drawn,
                Lost = lost,
                GoalsFor = goalsFor,
                GoalsAgainst = goalsAgainst,
                GoalDifference = goalDifference,
                Points = points,
                TeamUrl = teamUrl
            });
        }

        return result;
    }

    private static string CleanCellText(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        return Regex.Replace(raw, @"\[.*?\]", "").Trim();
    }
}