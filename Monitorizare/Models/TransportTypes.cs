namespace Monitorizare.Models;

public abstract record TransportType;
public sealed record Incarcare(long Timestamp, int Siloz, int Greutate) : TransportType;
public sealed record Descarcare(long Timestamp, int Siloz, int Greutate, int Hala, int Buncar) : TransportType;