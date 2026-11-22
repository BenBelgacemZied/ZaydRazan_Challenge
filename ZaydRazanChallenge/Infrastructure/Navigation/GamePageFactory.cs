using Microsoft.Extensions.DependencyInjection;

namespace ZaydRazanChallenge.Infrastructure.Navigation;

public interface IGamePageFactory
{
    MainPage CreateHomePage();
    AdventurePage CreateAdventurePage();
    AdventureTestMenuPage CreateAdventureTestMenuPage();
    ParisChapterPage CreateParisChapterPage();
    ParisTreasurePage CreateParisTreasurePage(int stage);
    PuzzleGalleryPage CreatePuzzleGalleryPage();
    PuzzlePage CreatePuzzlePage(PuzzleDefinition puzzle);
    PuzzleDiscoveryPage CreatePuzzleDiscoveryPage(PuzzleDefinition puzzle, int moves, int maxMoves, int reward);
    WordGamesHubPage CreateWordGamesHubPage();
    WordGamePage CreateWordGamePage(WordGameMode mode);
}

/// <summary>
/// Resolves constructor-injected pages and supplies runtime route arguments.
/// This keeps MAUI container access at the presentation composition boundary.
/// </summary>
internal sealed class GamePageFactory(IServiceProvider services) : IGamePageFactory
{
    public MainPage CreateHomePage() => services.GetRequiredService<MainPage>();
    public AdventurePage CreateAdventurePage() => services.GetRequiredService<AdventurePage>();
    public AdventureTestMenuPage CreateAdventureTestMenuPage() => services.GetRequiredService<AdventureTestMenuPage>();
    public ParisChapterPage CreateParisChapterPage() => services.GetRequiredService<ParisChapterPage>();
    public ParisTreasurePage CreateParisTreasurePage(int stage) =>
        ActivatorUtilities.CreateInstance<ParisTreasurePage>(services, stage);
    public PuzzleGalleryPage CreatePuzzleGalleryPage() => services.GetRequiredService<PuzzleGalleryPage>();
    public PuzzlePage CreatePuzzlePage(PuzzleDefinition puzzle) =>
        ActivatorUtilities.CreateInstance<PuzzlePage>(services, puzzle);
    public PuzzleDiscoveryPage CreatePuzzleDiscoveryPage(PuzzleDefinition puzzle, int moves, int maxMoves, int reward) =>
        ActivatorUtilities.CreateInstance<PuzzleDiscoveryPage>(services, puzzle, moves, maxMoves, reward);
    public WordGamesHubPage CreateWordGamesHubPage() => services.GetRequiredService<WordGamesHubPage>();
    public WordGamePage CreateWordGamePage(WordGameMode mode) =>
        ActivatorUtilities.CreateInstance<WordGamePage>(services, mode);
}
