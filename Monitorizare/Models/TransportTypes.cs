namespace Monitorizare.Models;

public abstract record TransportLog;
public record Incarcare(double Data, int Siloz, int Greutate) : TransportLog;
public record Descarcare(double Data, int Siloz, int Greutate, int Hala, int Buncar) : TransportLog;