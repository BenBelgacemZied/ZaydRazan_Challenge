using ZRLingoTrip.Domain.Games;

namespace ZRLingoTrip.Application.Games;

public interface IPuzzleCatalog
{
    IReadOnlyList<PuzzleDefinition> Items { get; }
    PuzzleDefinition GetRandom(string? excludedKey = null);
}

public interface IVocabularyCatalog
{
    IReadOnlyList<VocabularyWord> Items { get; }
    VocabularyWord[] RandomWords(int count, Func<VocabularyWord, bool>? filter = null);
}
