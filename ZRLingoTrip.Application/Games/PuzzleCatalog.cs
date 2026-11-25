using ZRLingoTrip.Application.Content;
using ZRLingoTrip.Domain.Games;

namespace ZRLingoTrip.Application.Games;

/// <summary>Shared puzzle selection behavior over the active destination's puzzle content.</summary>
public sealed class PuzzleCatalog : IPuzzleCatalog
{
    private readonly IDestinationContent _destination;

    public IReadOnlyList<PuzzleDefinition> Items => _destination.Puzzles;

    public PuzzleCatalog(IDestinationContent destination) => _destination = destination;

    public PuzzleDefinition GetRandom(string? excludedKey = null)
    {
        var choices = Items.Where(item => item.Key != excludedKey).ToArray();
        return choices[Random.Shared.Next(choices.Length)];
    }
}
