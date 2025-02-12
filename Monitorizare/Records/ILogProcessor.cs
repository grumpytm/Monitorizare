namespace Monitorizare.Records;

public interface ILogProcessor
{
    Task<IEnumerable<Queries>> ProcessLogAsync(string filePath);
}
