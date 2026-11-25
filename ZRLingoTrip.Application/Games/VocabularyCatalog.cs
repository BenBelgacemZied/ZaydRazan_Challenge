using ZRLingoTrip.Application.Content;
using ZRLingoTrip.Domain.Games;

namespace ZRLingoTrip.Application.Games;

/// <summary>Shared vocabulary selection behavior over the active destination's language pair content.</summary>
public sealed class VocabularyCatalog : IVocabularyCatalog
{
    public IReadOnlyList<VocabularyWord> Items { get; }

    public VocabularyCatalog(IDestinationContent destination) => Items = destination.VocabularyWords;

    public VocabularyWord[] RandomWords(int count, Func<VocabularyWord, bool>? filter = null)
    {
        var source = filter is null ? Items : Items.Where(filter);
        return source.OrderBy(_ => Random.Shared.Next()).Take(count).ToArray();
    }
}
