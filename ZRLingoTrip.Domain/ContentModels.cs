namespace ZRLingoTrip.Domain.Content;

public sealed record LanguageCode(string Value);

/// <summary>The learner's language and the language learned at a destination.</summary>
public sealed record LearningLanguagePair(LanguageCode SourceLanguage, LanguageCode TargetLanguage);

/// <summary>A destination owns the language children learn there, independent of their source language.</summary>
public sealed record DestinationProfile(string Id, string DisplayName, LanguageCode TargetLanguage);

public sealed record DestinationClue(
    string SourceSentence,
    string HighlightedSourceText,
    string TargetWord,
    string[] TargetChoices);

public sealed record DestinationQuest(string Title, string TargetName, string Photo, string Story, DestinationClue[] Clues);
