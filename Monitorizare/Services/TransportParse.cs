using System.Globalization;

namespace Monitorizare.Services;

public class TransportParse
{
    private string[] _items;
    public int Count => _items.Length;

    public TransportParse(string line) =>
        _items = string.IsNullOrWhiteSpace(line) ? Array.Empty<string>() : line.Split(',');

    public bool IsValid() =>
        Count == 4 || Count == 6;

    public record ParseResult(double Timestamp, int Siloz, int Greutate, int Hala, int Buncar, bool IsValid = true)
    {
        public static ParseResult Invalid() =>
            new ParseResult(0, 0, 0, 0, 0, false);
    }

    public ParseResult Parse() =>
        !IsValid() ? ParseResult.Invalid()
        : new ParseResult(
            Timestamp: Date.ToDateTime(Time).ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            Siloz: GetFrom(2),
            Greutate: GetFrom(3),
            Hala: Count == 6 ? GetFrom(4) : 0,
            Buncar: Count == 6 ? GetFrom(5) : 0
        );

    private DateOnly Date =>
            DateOnly.ParseExact(_items[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);

    private TimeOnly Time =>
        TimeOnly.ParseExact(_items[1], "H:mm", CultureInfo.InvariantCulture);

    private int GetFrom(int position) =>
        int.TryParse(_items[position], out var result) ? result : 0;
}
