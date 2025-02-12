using System.Data.Common;
using Monitorizare.Records;

namespace Monitorizare.Data;

public abstract class DatabaseService
{
    protected abstract DbConnection CreateConnection();
    public abstract Task IntegrityCheckAsync();
    public abstract Task<(int, int)> SaveRecordsAsync(IEnumerable<Queries> queryList);
    protected abstract Task<int> ExecuteQueriesAsync(IEnumerable<Queries> queryList, DbConnection connection, DbTransaction transaction);
    public abstract Task LogMessageAsync(string message);
}