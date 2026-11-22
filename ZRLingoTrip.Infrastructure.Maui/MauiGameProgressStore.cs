using Microsoft.Maui.Storage;
using ZRLingoTrip.Application;

namespace ZaydRazanChallenge.Infrastructure.Maui;

/// <summary>MAUI Preferences adapter. Temporary test values never reach persistent storage.</summary>
public sealed class MauiGameProgressStore : IGameProgressStore
{
    private Dictionary<string, object>? _testValues;

    public bool IsTestMode => _testValues is not null;

    public bool Get(string key, bool fallback) => _testValues is null
        ? Preferences.Default.Get(key, fallback)
        : _testValues.TryGetValue(key, out var value) && value is bool result ? result : fallback;

    public int Get(string key, int fallback) => _testValues is null
        ? Preferences.Default.Get(key, fallback)
        : _testValues.TryGetValue(key, out var value) && value is int result ? result : fallback;

    public void Set(string key, bool value)
    {
        if (_testValues is null) Preferences.Default.Set(key, value);
        else _testValues[key] = value;
    }

    public void Set(string key, int value)
    {
        if (_testValues is null) Preferences.Default.Set(key, value);
        else _testValues[key] = value;
    }

    public void Remove(string key)
    {
        if (_testValues is null) Preferences.Default.Remove(key);
        else _testValues.Remove(key);
    }

    public void BeginTest() => _testValues = new Dictionary<string, object>();
    public void ResetTest() { if (IsTestMode) _testValues = new Dictionary<string, object>(); }
    public void EndTest() => _testValues = null;
    public void Clear() => Preferences.Default.Clear();
}
