namespace ZRLingoTrip.Application;

/// <summary>Application port for saved game state and temporary test-play state.</summary>
public interface IGameProgressStore
{
    bool IsTestMode { get; }
    bool Get(string key, bool fallback);
    int Get(string key, int fallback);
    void Set(string key, bool value);
    void Set(string key, int value);
    void Remove(string key);
    void BeginTest();
    void ResetTest();
    void EndTest();
    void Clear();
}
