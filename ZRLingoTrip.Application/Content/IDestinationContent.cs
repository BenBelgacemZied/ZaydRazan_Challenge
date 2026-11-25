using ZRLingoTrip.Domain.Content;
using ZRLingoTrip.Domain.Games;

namespace ZRLingoTrip.Application.Content;

/// <summary>Port implemented by destination content packs and consumed by shared game flows.</summary>
public interface IDestinationContent
{
    DestinationProfile Profile { get; }
    LearningLanguagePair LanguagePair { get; }
    int FirstStage { get; }
    int Count { get; }
    IReadOnlyList<DestinationQuest> Quests { get; }
    IReadOnlyList<PuzzleDefinition> Puzzles { get; }
    IReadOnlyList<VocabularyWord> VocabularyWords { get; }
    string[] PhotosFor(int index);
}
