namespace ZaydRazanChallenge;

internal sealed record VocabularyWord(string Dutch, string French, string Category);

internal static class VocabularyCatalog
{
    private static readonly VocabularyWord[] JourneyWords =
    [
        new("goedendag", "bonjour", "Begroetingen"),
        new("dank je", "merci", "Begroetingen"),
        new("tot ziens", "au revoir", "Begroetingen"),
        new("een tas", "un sac", "Op reis"),
        new("een rugzak", "un sac à dos", "Op reis"),
        new("een koffer", "une valise", "Op reis"),
        new("een T-shirt", "un tee-shirt", "Kleding"),
        new("een broek", "un pantalon", "Kleding"),
        new("een blauwe broek", "un pantalon bleu", "Kleding"),
        new("een roze T-shirt", "un tee-shirt rose", "Kleding"),
        new("een hoed", "un chapeau", "Kleding"),
        new("sokken", "des chaussettes", "Kleding"),
        new("een boek", "un livre", "In de koffer"),
        new("een tandenborstel", "une brosse à dents", "In de koffer"),
        new("een drinkfles", "une gourde", "In de koffer"),
        new("een fototoestel", "un appareil photo", "In de koffer"),
        new("de paspoorten", "les passeports", "Documenten"),
        new("de tickets", "les billets", "Documenten"),
        new("de kaart van Parijs", "le plan de Paris", "Documenten"),
        new("het reisboekje", "le carnet de voyage", "Documenten"),
        new("betalen", "payer", "Het station"),
        new("reizen", "voyager", "Het station"),
        new("het station", "la gare", "Het station"),
        new("het perron", "le quai", "Het station"),
        new("de trein", "le train", "De trein"),
        new("de deur", "la porte", "De trein"),
        new("de stoel", "le siège", "De trein"),
        new("het bagagerek", "le porte-bagages", "De trein"),
        new("het raam", "la fenêtre", "De trein"),
        new("de tafel", "la table", "De trein")
    ];

    public static IReadOnlyList<VocabularyWord> Items { get; } = JourneyWords
        .Concat(ParisTreasureCatalog.Quests.SelectMany(quest => quest.Clues)
            .Select(clue => new VocabularyWord(clue.Highlight, clue.Word, "Parijs")))
        .GroupBy(word => word.French.Trim().ToLowerInvariant())
        .Select(group => group.First())
        .OrderBy(word => word.Category)
        .ThenBy(word => word.Dutch)
        .ToArray();

    public static VocabularyWord[] RandomWords(int count, Func<VocabularyWord, bool>? filter = null)
    {
        var source = filter is null ? Items : Items.Where(filter);
        return source.OrderBy(_ => Random.Shared.Next()).Take(count).ToArray();
    }
}
