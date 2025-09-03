namespace Monitorizare.Models;

public abstract record TransportType;
public record Incarcare(double Data, int Siloz, int Greutate) : TransportType;
public record Descarcare(double Data, int Siloz, int Greutate, int Hala, int Buncar) : TransportType;