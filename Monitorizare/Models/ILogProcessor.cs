using System.Collections.Immutable;

namespace Monitorizare.Models;

public interface ILogProcessor
{
    Task<ImmutableList<TransportType>> TryParseLogAsync(string filePath);
}