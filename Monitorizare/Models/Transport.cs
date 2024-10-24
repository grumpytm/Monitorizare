namespace Monitorizare.Models;

public abstract record Transport(double Data, int Siloz, int Greutate, int Hala, int Buncar);

public record Incarcare(double Data, int Siloz, int Greutate) : Transport(Data, Siloz, Greutate, 0, 0);

public record Descarcare(double Data, int Siloz, int Greutate, int Hala, int Buncar) : Transport(Data, Siloz, Greutate, Hala, Buncar);