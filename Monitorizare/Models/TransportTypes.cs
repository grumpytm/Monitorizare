namespace Monitorizare.Models;

public record struct Incarcare(double Data, int Siloz, int Greutate) : ITransport;
public record struct Descarcare(double Data, int Siloz, int Greutate, int Hala, int Buncar) : ITransport;
