using Backend.Models;
using ClosedXML.Excel;

namespace Backend.Utils;

public static class Parser
{
    public static List<JokerDraw> ParseJokerFile(string year)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", $"Joker_{year}.xlsx");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"No data found for year {year}.");

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);
        var prizeCategories = new[] { "5 + 1", "5", "4 + 1", "4", "3 + 1", "3", "2 + 1", "1 + 1", "2" };
        var results = new List<JokerDraw>();

        var lastRow = worksheet.LastRowUsed()!.RowNumber();
        for (int r = 4; r <= lastRow; r++)
        {
            var row = worksheet.Row(r);
            var drawNumber = row.Cell(1).GetString();
            if (string.IsNullOrWhiteSpace(drawNumber)) continue;

            var numbers = new[]
            {
                row.Cell(3).GetString(), row.Cell(4).GetString(), row.Cell(5).GetString(),
                row.Cell(6).GetString(), row.Cell(7).GetString()
            }.Select(int.Parse).ToArray();

            var prizes = new List<PrizeTier>();
            int col = 10;
            foreach (var category in prizeCategories)
            {
                prizes.Add(new PrizeTier(category, row.Cell(col).GetString(), row.Cell(col + 1).GetString()));
                col += 2;
            }

            results.Add(new JokerDraw(
                drawNumber, row.Cell(2).GetString(), numbers,
                int.Parse(row.Cell(8).GetString()), row.Cell(9).GetString(), prizes
            ));
        }

        return results;
    }
}
