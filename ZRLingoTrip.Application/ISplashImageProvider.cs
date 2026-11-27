namespace ZRLingoTrip.Application;

/// <summary>Provides the splash artwork for the selected learner support language.</summary>
public interface ISplashImageProvider
{
    string GetSplashImageAsset(string sourceLanguageCode);
}
