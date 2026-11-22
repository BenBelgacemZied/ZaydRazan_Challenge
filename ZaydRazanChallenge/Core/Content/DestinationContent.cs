namespace ZaydRazanChallenge.Core.Content;

public sealed record LearningLanguagePair(string PromptLanguage, string AnswerLanguage);

public sealed record DestinationProfile(
    string Id,
    string DisplayName,
    LearningLanguagePair LanguagePair);

public sealed record DestinationClue(
    string DutchSentence,
    string Highlight,
    string Word,
    string[] Choices);

public sealed record DestinationQuest(
    string Title,
    string Target,
    string Photo,
    string Story,
    DestinationClue[] Clues);

/// <summary>
/// Destination-owned learning content consumed by the shared game flows.
/// UI pages should depend on this contract instead of a specific city catalog.
/// </summary>
public interface IDestinationContent
{
    DestinationProfile Profile { get; }
    int FirstStage { get; }
    int Count { get; }
    IReadOnlyList<DestinationQuest> Quests { get; }
    string[] PhotosFor(int index);
}

