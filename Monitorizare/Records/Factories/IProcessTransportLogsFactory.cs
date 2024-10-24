namespace Monitorizare.Records;
public interface IProcessTransportLogsFactory
{
    IParseService CreateParseService();
    IQueryService CreateQueryService();
}