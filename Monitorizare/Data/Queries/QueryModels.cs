namespace Monitorizare.Data.Queries;

public sealed record DateTimeBoundsDTO(DateTime Min, DateTime Max)
{
    public static implicit operator ValueTuple<DateTime, DateTime>(DateTimeBoundsDTO record) => (record.Min, record.Max);
}

public sealed record LongBoundsDTO(string Type, long Min, long Max)
{
    public static implicit operator ValueTuple<long, long>(LongBoundsDTO record) => (record.Min, record.Max);
}

public record struct LastDatesDTO(string Type, DateTime Last);
public record struct IncarcareDTO(DateOnly Date, TimeOnly Time, int Siloz, int Greutate);
public record struct DescarcareDTO(DateOnly Date, TimeOnly Time, int Siloz, int Greutate, int Hala, int Buncar);
public sealed record TransportContentDTO(int Id, DateOnly Date, TimeOnly Time, int Siloz, int Greutate, int Hala, int Buncar);
public sealed record LogsDTO(int Id, string Type, DateOnly Date, TimeOnly Time, string Message);
public sealed record MigrationsDTO(int Id, string File, DateOnly Date, TimeOnly Time);