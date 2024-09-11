// ----- Infrastructure
static class RepositoryExtensions // services
{
    public static IEnumerable<Usage> GetTimeWindow(
        this Repository repository, DateTime cutoff, DateTime before) =>
        repository.GetUsageFromLatest(before)
            .TakeWhile(sample => sample.Started >= cutoff);
}

class Repository
{
    public IEnumerable<Usage> GetUsageFromLatest(DateTime at)
    {
        Random rnd = new(Guid.NewGuid().GetHashCode());
        int stepSeconds = 240;
        int durationSeconds = 620;
        DateTime time = at;

        while (true)
        {
            time -= TimeSpan.FromSeconds(rnd.NextDouble() * stepSeconds);
            DateTime lastSeen = time + TimeSpan.FromSeconds(rnd.NextDouble() * durationSeconds);
            yield return new(time, lastSeen);
        }
    }
}

static class ConsoleExtensions
{
    public static void WriteLines(this IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}

// ---- Program?
static class Demo
{
    public static void Something()
    {
        DateTime now = DateTime.UtcNow.Date.AddHours(10);
        int maxAgeHours = 48;
        DateTime cutoff = now.AddHours(-maxAgeHours);

        // ----- Endpoint/controller/etc.
        new Repository()
            .GetTimeWindow(cutoff, now)
            .GetHourlyHistogram()
            .Show(cutoff, maxAgeHours);
    }
}

// ----- Domain
public record Usage(DateTime Started, DateTime LastSeen);

static class Histogram
{
    public static IDictionary<DateTime, int> GetHourlyHistogram(
        this IEnumerable<Usage> samples) =>
        samples.Select(sample => sample.Started)
        .GroupBy(time => time.Date.AddHours(time.Hour))
        .ToDictionary(group => group.Key, group => group.Count());

    public static int GetMinValue(this IDictionary<DateTime, int> histogram) =>
        histogram.Values.Min();

    public static int GetMaxValue(this IDictionary<DateTime, int> histogram) =>
        histogram.Values.Max();

    public static int GetHeight(this IDictionary<DateTime, int> histogram, DateTime key) =>
        histogram.TryGetValue(key, out int known) ? known : 0;
}

// ----- Presentation/UI
static class HistogramUIExtension
{
    public static void Show(
        this IDictionary<DateTime, int> histogram, DateTime cutoff, int barsCount)
    {
        (int height, int low) = (histogram.GetMaxValue(), histogram.GetMinValue());
        for (int h = height; h >= height; h--)
        {
            IEnumerable<string> pixels = Enumerable.Range(0, barsCount)
                .Select(i => histogram.GetHeight(cutoff.AddHours(i)))
                .Select(barHeight => barHeight >= h ? "#" : " ");
            Console.WriteLine(string.Join(" ", pixels).TrimEnd());
        }
        int width = 2 * barsCount - 1;
        Console.WriteLine(new string('-', width));
        Console.WriteLine("-48" + "Now".PadLeft(width - "-48".Length));
    }
}