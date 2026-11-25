namespace ZRLingoTrip.Domain.Games;

public sealed record VocabularyWord(string SourceText, string TargetText, string Category);

public enum WordGameMode { Connect, ThreeChoices, BuildWord, Listening }
