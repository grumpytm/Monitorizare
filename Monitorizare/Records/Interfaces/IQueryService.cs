using Monitorizare.Models;

namespace Monitorizare.Records;

public interface IQueryService
{
    public List<string> CreateQueryListFrom(IEnumerable<Transport> records);
}