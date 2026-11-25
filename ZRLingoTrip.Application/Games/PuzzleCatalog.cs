using ZRLingoTrip.Application.Content;
using ZRLingoTrip.Domain.Games;

namespace ZRLingoTrip.Application.Games;

/// <summary>Shared puzzle selection behavior over the active destination's puzzle content.</summary>
public sealed class PuzzleCatalog : IPuzzleCatalog
{
    public IReadOnlyList<PuzzleDefinition> Items { get; }

    public PuzzleCatalog(IDestinationContent destination) => Items = destination.Puzzles;

    public PuzzleDefinition GetRandom(string? excludedKey = null)
    {
        var choices = Items.Where(item => item.Key != excludedKey).ToArray();
        return choices[Random.Shared.Next(choices.Length)];
    }
}
