using Backend.Models;
using Backend.Utils;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
    if (!int.TryParse(year, out _)) return Results.BadRequest("Invalid year.");
    try
    {
        return Results.Ok(Parser.ParseJokerFile(year));
    }
    catch (FileNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.MapGet("/api/joker-data/{year}/pdf", (string year) =>
{
    if (!int.TryParse(year, out _)) return Results.BadRequest("Invalid year.");

    List<JokerDraw> draws;
    try
    {
        draws = Parser.ParseJokerFile(year);
    }
    catch (FileNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }

    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9));

            page.Header()
                .Text($"Joker {year} Results")
                .FontSize(18).Bold();

            page.Content().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();  // Draw #
                    columns.RelativeColumn();  // Date
                    columns.RelativeColumn(2); // Numbers
                    columns.RelativeColumn();  // Joker
                    columns.RelativeColumn();  // 5+1 Matches
                    columns.RelativeColumn();  // 5+1 Winnings
                });

                table.Header(header =>
                {
                    header.Cell().Text("Draw #").Bold();
                    header.Cell().Text("Date").Bold();
                    header.Cell().Text("Numbers").Bold();
                    header.Cell().Text("Joker").Bold();
                    header.Cell().Text("5+1 Matches").Bold();
                    header.Cell().Text("5+1 Winnings").Bold();
                });

                foreach (var draw in draws)
                {
                    var topPrize = draw.Prizes.FirstOrDefault(p => p.Category == "5 + 1");

                    table.Cell().Text(draw.DrawNumber);
                    table.Cell().Text(draw.Date);
                    table.Cell().Text(string.Join(", ", draw.Numbers));
                    table.Cell().Text(draw.JokerNumber.ToString());
                    table.Cell().Text(topPrize?.Matches ?? "-");
                    table.Cell().Text(topPrize?.Winnings ?? "-");
                }
            });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
        });
    });

    var pdfBytes = document.GeneratePdf();
    return Results.File(pdfBytes, "application/pdf", $"Joker_{year}_Results.pdf");
});

app.MapFallbackToFile("index.html");

app.Run();