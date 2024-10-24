using Monitorizare.Records.Services;

namespace Monitorizare.Records;

public class ProcessDescarcareLogs : IProcessTransportLogsFactory
{
    public IParseService CreateParseService() => new DescarcareParseService();

    public IQueryService CreateQueryService() => new DescarcareQueryService();
}