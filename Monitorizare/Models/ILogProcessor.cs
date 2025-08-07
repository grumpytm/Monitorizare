namespace Monitorizare.Models;

public interface ILogProcessor
{
    Task<IEnumerable<TransportLog>> TryParseLoadingAsync(string filePath);
    Task<IEnumerable<TransportLog>> TryParseUnloadingAsync(string filePath);
}