using Monitorizare.Models;

namespace Monitorizare.Records.Services;

public class IncarcareQueryService : IQueryService
{
    public List<string> CreateQueryListFrom(IEnumerable<Transport> records)
    {
        List<string> queryList = new List<string> { };

        foreach (var record in records)
        {
            string query = record switch
            {
                Incarcare => $"INSERT OR IGNORE INTO incarcare (data, siloz, greutate) VALUES ('{record.Data}', '{record.Siloz}', '{record.Greutate}')",
                _ => throw new InvalidOperationException($"Unknown record type: {record.GetType().Name}")
            };

            queryList.Add(query);
        }
        return queryList;
    }
}