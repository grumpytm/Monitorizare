namespace Monitorizare.Models;

public interface ITransport
{
    public static IEnumerable<string> TableList =>
        new[] { "incarcare", "descarcare" }; // figure out something better?
}
