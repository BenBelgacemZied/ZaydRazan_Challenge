namespace ZaydRazanChallenge;

public static class BrusselsPuzzleCatalog
{
    public static IReadOnlyList<PuzzleDefinition> Items { get; } =
    [
        new(
            "brussels_atomium",
            "L’Atomium",
            "l’Atomium",
            "⚛️",
            "Het Atomium is een bekend monument in Brussel. Het werd gebouwd voor de Wereldtentoonstelling van 1958. Het gebouw lijkt op een ijzerkristal en heeft negen bollen.",
            "brussels_atomium.svg"),
        new(
            "brussels_grand_place",
            "La Grand-Place",
            "la Grand-Place",
            "🏛️",
            "De Grote Markt is een beroemd plein in het centrum van Brussel. Rond het plein staan mooie historische gebouwen.",
            "brussels_grand_place.svg"),
        new(
            "brussels_manneken_pis",
            "Manneken Pis",
            "Manneken Pis",
            "⛲",
            "Manneken Pis is een klein bronzen beeld en een bekend symbool van Brussel. Het beeld staat bij een fontein.",
            "brussels_manneken_pis.svg"),
        new(
            "brussels_waffle",
            "La gaufre belge",
            "la gaufre belge",
            "🧇",
            "De Belgische wafel is goudbruin en heeft kleine vierkantjes. In Brussel kun je verschillende soorten wafels proeven.",
            "brussels_waffle.svg"),
        new(
            "brussels_mini_europe",
            "Mini-Europe",
            "Mini-Europe",
            "🌍",
            "In Mini-Europe zie je kleine modellen van bekende gebouwen uit verschillende Europese landen.",
            "brussels_mini_europe.svg")
    ];
}
