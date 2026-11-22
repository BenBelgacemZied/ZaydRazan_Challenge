using Microsoft.Extensions.DependencyInjection;

namespace ZaydRazanChallenge;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton<IDestinationContent, ParisDestinationContent>();
        builder.Services.AddSingleton<IPuzzleCatalog, PuzzleCatalog>();
        builder.Services.AddSingleton<IVocabularyCatalog, VocabularyCatalog>();
        builder.Services.AddSingleton<IGamePageFactory, GamePageFactory>();

        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AdventurePage>();
        builder.Services.AddTransient<AdventureTestMenuPage>();
        builder.Services.AddTransient<ParisChapterPage>();
        builder.Services.AddTransient<ParisTreasurePage>();
        builder.Services.AddTransient<PuzzleGalleryPage>();
        builder.Services.AddTransient<PuzzlePage>();
        builder.Services.AddTransient<PuzzleDiscoveryPage>();
        builder.Services.AddTransient<WordGamesHubPage>();
        builder.Services.AddTransient<WordGamePage>();

        return builder.Build();
    }
}
