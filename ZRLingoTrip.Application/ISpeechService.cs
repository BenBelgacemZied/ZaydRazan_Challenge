namespace ZRLingoTrip.Application;

/// <summary>Application port for speaking text in an ISO language tag such as fr or nl.</summary>
public interface ISpeechService
{
    Task SpeakAsync(string text, string languageTag);
}
