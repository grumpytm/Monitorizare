namespace Monitorizare.Models;

public record struct Transport(double Data, int Siloz, int Greutate, int? Hala = null, int? Buncar = null)
{
    public static Transport Incarcare(double Data, int Siloz, int Greutate) =>
        new(Data, Siloz, Greutate);

    public static Transport Descarcare(double Data, int Siloz, int Greutate, int Hala, int Buncar) =>
        new(Data, Siloz, Greutate, Hala, Buncar);
}