using Microsoft.Maui.Storage;
using ZRLingoTrip.Application;

namespace ZaydRazanChallenge.Infrastructure.Maui;

public sealed class MauiLearningLanguageSettings : ILearningLanguageSettings
{
    internal const string PreferenceKey = "source_language_code";

    public string SourceLanguageCode
    {
        get => Preferences.Default.Get(PreferenceKey, "nl") == "en" ? "en" : "nl";
        set
        {
            if (value is not ("nl" or "en"))
                throw new ArgumentOutOfRangeException(nameof(value), "Supported source languages are nl and en.");

            Preferences.Default.Set(PreferenceKey, value);
        }
    }
}
