namespace Monitorizare.Models;

public abstract record Transport();

sealed record Incarcare(double data, int siloz, int greutate) : Transport
{
    public static Incarcare Create(double data, int siloz, int greutate) =>
        new(data, siloz, greutate);
}

sealed record Descarcare(double data, int siloz, int greutate, int hala, int buncar) : Transport
{
    public static Descarcare Create(double data, int siloz, int greutate, int hala, int buncar) =>
        new(data, siloz, greutate, hala, buncar);
}