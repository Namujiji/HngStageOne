namespace HngStageOne.Application.Utilities;

public static class AgeRanges
{
    private static readonly Dictionary<string, (int Min, int Max)> Ranges = new()
    {
        { "Child", (1, 12) },
        { "Teenager", (13, 19) },
        { "Adult", (20, 59) },
        { "Senior", (60, int.MaxValue) }
    };

    public static string? GetKey(int value)
    {
        foreach (var kvp in Ranges)
        {
            if (value >= kvp.Value.Min && value <= kvp.Value.Max)
            {
                return kvp.Key;
            }
        }

        return "unknown";
    }

    public static string? GetValue(string key)
    {
        foreach (var kvp in Ranges)
        {
            if (kvp.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase))
            {
                return kvp.Key;
            }
        }

        return null;
    }
}
