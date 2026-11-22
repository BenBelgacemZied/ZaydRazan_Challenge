namespace ZRLingoTrip.Domain.Games;

public sealed record PuzzleDefinition(
    string Key,
    string Title,
    string TargetName,
    string Emoji,
    string Description,
    string? Image = null)
{
    public string ImageSource => Image ?? $"{Key}_puzzle.jpg";
}
