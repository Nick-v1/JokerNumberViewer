using Backend.Models;
using ClosedXML.Excel;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/joker-years", () =>
{
    var resourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources");
    var years = Directory.GetFiles(resourcesPath, "Joker_*.xlsx")
        .Select(f => Path.GetFileNameWithoutExtension(f).Replace("Joker_", ""))
        .Where(y => int.TryParse(y, out _))
        .OrderByDescending(y => y)
        .ToList();

    return Results.Ok(years);
});

app.MapGet("/api/joker-data/{year}", (string year) =>
{
    if (!int.TryParse(year, out _))
        return Results.BadRequest("Invalid year.");

    var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", $"Joker_{year}.xlsx");

    if (!File.Exists(filePath))
        return Results.NotFound($"No data found for year {year}.");

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

    return Results.Ok(results);
});

app.MapFallbackToFile("index.html");

app.Run();