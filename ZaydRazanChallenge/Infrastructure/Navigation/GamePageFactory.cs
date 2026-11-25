namespace ZaydRazanChallenge.Infrastructure.Navigation;

/// <summary>Creates presentation pages while forwarding their constructor-injected dependencies.</summary>
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
/// Presentation factory for pages with runtime route arguments. Dependencies are injected once here;
/// this type does not resolve services from the container.
/// </summary>
internal sealed class GamePageFactory(
    IDestinationContent destination,
    IPuzzleCatalog puzzles,
    IVocabularyCatalog vocabulary,
    IGameProgressStore progress,
    ISpeechService speech,
    ILearningLanguageSettings languageSettings) : IGamePageFactory
{
    public MainPage CreateHomePage() => new(destination, this, progress, speech, languageSettings);
    public AdventurePage CreateAdventurePage() => new(destination, this, progress, speech);
    public AdventureTestMenuPage CreateAdventureTestMenuPage() => new(destination, this, progress, speech);
    public ParisChapterPage CreateParisChapterPage() => new(destination, this, progress);
    public ParisTreasurePage CreateParisTreasurePage(int stage) => new(stage, destination, progress, speech);
    public PuzzleGalleryPage CreatePuzzleGalleryPage() => new(puzzles, this, progress);
    public PuzzlePage CreatePuzzlePage(PuzzleDefinition puzzle) => new(puzzle, puzzles, this, progress, speech, destination);
    public PuzzleDiscoveryPage CreatePuzzleDiscoveryPage(PuzzleDefinition puzzle, int moves, int maxMoves, int reward) =>
        new(puzzle, moves, maxMoves, reward, puzzles, this, progress, speech, destination);
    public WordGamesHubPage CreateWordGamesHubPage() => new(vocabulary, this, destination);
    public WordGamePage CreateWordGamePage(WordGameMode mode) => new(mode, vocabulary, progress, speech, destination);
}
