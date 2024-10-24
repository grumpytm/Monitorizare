using Monitorizare.Models;

namespace Monitorizare.Database;

public interface IRecordSaver
{
    Task<(int, int)> SaveRecordsAsync(List<string> queryList);
}