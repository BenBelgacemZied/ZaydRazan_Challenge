using Microsoft.Extensions.DependencyInjection;
using ZRLingoTrip.Application;

namespace ZaydRazanChallenge;

/// <summary>The only place that binds application ports to their production adapters.</summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton<IDestinationContent, ParisDestinationContent>();
        builder.Services.AddSingleton<IPuzzleCatalog, PuzzleCatalog>();
        builder.Services.AddSingleton<IVocabularyCatalog, VocabularyCatalog>();
        builder.Services.AddSingleton<IGameProgressStore, MauiGameProgressStore>();
        builder.Services.AddSingleton<ISpeechService, MauiSpeechService>();
        builder.Services.AddSingleton<IGamePageFactory, GamePageFactory>();
        builder.Services.AddTransient<SplashPage>();

        return builder.Build();
    }
}
