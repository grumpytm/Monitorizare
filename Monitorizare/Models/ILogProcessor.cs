namespace Monitorizare.Models;

public interface ILogProcessor
{
    Task<IEnumerable<ITransport>> TryParseFileAsync(string filePath);
}