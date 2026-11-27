using ZRLingoTrip.Application;

namespace ZRLingoTrip.Infrastructure.Maui;

/// <summary>Maps language selection to the matching bundled splash image.</summary>
public sealed class MauiSplashImageProvider : ISplashImageProvider
{
    public string GetSplashImageAsset(string sourceLanguageCode) =>
        string.Equals(sourceLanguageCode, "en", StringComparison.OrdinalIgnoreCase)
            ? "splash_en.png"
            : "splash_nl.png";
}
