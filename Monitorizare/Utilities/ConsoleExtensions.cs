namespace Monitorizare.Utilities;

static class ConsoleExtensions
{
    public static void WriteLines(this IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine($"debug output: {line}");
        }
    }

    public static void PrintOut(params object[] args)
    {
        foreach (var ele in args)
        {
            Console.WriteLine($"{ele}");
        }
    }
}