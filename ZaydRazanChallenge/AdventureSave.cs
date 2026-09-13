namespace ZaydRazanChallenge;

// Adventure test progress lives only in memory. Game progress stays in Preferences.
internal static class AdventureSave
{
    private static Dictionary<string, object>? _testValues;

    public static bool IsTestMode => _testValues is not null;

    public static void BeginTest() => _testValues = new Dictionary<string, object>();

    public static void ResetTest()
    {
        if (IsTestMode) _testValues = new Dictionary<string, object>();
    }

    public static void EndTest() => _testValues = null;

    public static bool Get(string key, bool fallback) =>
        _testValues is null
            ? Preferences.Default.Get(key, fallback)
            : _testValues.TryGetValue(key, out var value) && value is bool result ? result : fallback;

    public static int Get(string key, int fallback) =>
        _testValues is null
            ? Preferences.Default.Get(key, fallback)
            : _testValues.TryGetValue(key, out var value) && value is int result ? result : fallback;

    public static void Set(string key, bool value)
    {
        if (_testValues is null) Preferences.Default.Set(key, value);
        else _testValues[key] = value;
    }

    public static void Set(string key, int value)
    {
        if (_testValues is null) Preferences.Default.Set(key, value);
        else _testValues[key] = value;
    }

    public static void Remove(string key)
    {
        if (_testValues is null) Preferences.Default.Remove(key);
        else _testValues.Remove(key);
    }
}
