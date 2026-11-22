using Microsoft.Maui.Media;
using ZRLingoTrip.Application;

namespace ZaydRazanChallenge.Infrastructure.Maui;

/// <summary>MAUI text-to-speech adapter.</summary>
public sealed class MauiSpeechService : ISpeechService
{
    public async Task SpeakAsync(string text, string languageTag)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var locale = locales.FirstOrDefault(item => item.Language.StartsWith(languageTag, StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale });
        }
        catch
        {
            // Speech is an optional device capability; a missing locale should not stop gameplay.
        }
    }
}
