using Monitorizare.Models;

namespace Monitorizare.Records.Services;

public class DescarcareQueryService : IQueryService
{
   public List<string> CreateQueryListFrom(IEnumerable<Transport> records)
    {
        List<string> queryList = new List<string> { };

        foreach (var record in records)
        {
            string query = record switch
            {
                Descarcare => $"INSERT OR IGNORE INTO descarcare (data, siloz, greutate, hala, buncar) VALUES ('{record.Data}', '{record.Siloz}', '{record.Greutate}', '{record.Hala}', '{record.Buncar}')",
                _ => throw new InvalidOperationException($"Unknown record type: {record.GetType().Name}")
            };

            queryList.Add(query);
        }
        return queryList;
    }
}