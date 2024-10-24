using Monitorizare.Records.Services;

namespace Monitorizare.Records;

public class ProcessIncarcareLogs : IProcessTransportLogsFactory
{
    public IParseService CreateParseService() => new IncarcareParseService();

    public IQueryService CreateQueryService() => new IncarcareQueryService();
}