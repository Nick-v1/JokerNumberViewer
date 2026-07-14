namespace Backend.Models;

public record JokerDraw(
    string DrawNumber,
    string Date,
    int[] Numbers,
    int JokerNumber,
    string TotalColumns,
    List<PrizeTier> Prizes
);