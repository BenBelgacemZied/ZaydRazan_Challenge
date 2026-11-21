namespace ZaydRazanChallenge;

public sealed record BrusselsMission(
    string Icon,
    string Title,
    string Subtitle,
    string Story,
    string[] Questions,
    string[] CorrectAnswers,
    string[][] Choices);

public static class BrusselsCatalog
{
    public static IReadOnlyList<BrusselsMission> Missions { get; } =
    [
        new(
            "🚉",
            "Bruxelles-Midi",
            "Arrivée en Belgique",
            "Zayd et Razan arrivent à Bruxelles-Midi. Ils ouvrent leur carnet et commencent une nouvelle aventure.",
            ["Comment dit-on « la gare » en français ?", "Comment dit-on « le billet » en français ?", "Quelle phrase signifie « nous arrivons à Bruxelles » ?"],
            ["la gare", "le billet", "nous arrivons à Bruxelles"],
            [
                ["la gare", "la rue", "le parc"],
                ["le billet", "la carte", "le musée"],
                ["nous arrivons à Bruxelles", "nous visitons Paris", "nous rentrons à la maison"]
            ]),
        new(
            "💧",
            "Manneken Pis",
            "La petite statue célèbre",
            "Ils suivent les rues du centre et découvrent le Manneken Pis, une petite statue devenue un symbole de Bruxelles.",
            ["Où se trouve le Manneken Pis ?", "Comment dit-on « une statue » en français ?", "Que découvrent Zayd et Razan ?"],
            ["à Bruxelles", "une statue", "le Manneken Pis"],
            [
                ["à Bruxelles", "à Paris", "à Londres"],
                ["une statue", "une gare", "une montagne"],
                ["le Manneken Pis", "la tour Eiffel", "le Louvre"]
            ]),
        new(
            "🧇",
            "La gaufre belge",
            "Une pause gourmande",
            "Après la promenade, les enfants commandent une gaufre. Ils doivent parler français pour passer leur commande.",
            ["Comment demander une gaufre ?", "Comment dit-on « sucré » en français ?", "Que mangent-ils ?"],
            ["Une gaufre, s'il vous plaît", "sucré", "une gaufre"],
            [
                ["Une gaufre, s'il vous plaît", "Un train, s'il vous plaît", "Une carte, s'il vous plaît"],
                ["sucré", "fermé", "rapide"],
                ["une gaufre", "une statue", "un billet"]
            ]),
        new(
            "🌍",
            "Mini-Europe",
            "L'Europe en miniature",
            "À Mini-Europe, Zayd et Razan voyagent entre de petits monuments européens et observent les détails.",
            ["Comment dit-on « petit » en français ?", "Où visitent-ils de nombreux pays ?", "Comment dit-on « regarder » ?"],
            ["petit", "à Mini-Europe", "regarder"],
            [
                ["petit", "grand", "ancien"],
                ["à Mini-Europe", "à la gare", "au restaurant"],
                ["regarder", "dormir", "courir"]
            ]),
        new(
            "⚛️",
            "L'Atomium",
            "Les neuf sphères",
            "Ils lèvent les yeux vers l'Atomium. Ses grandes sphères brillent dans le ciel de Bruxelles.",
            ["Comment dit-on « grand » en français ?", "Combien de sphères voient-ils ?", "Que font-ils avec leurs yeux ?"],
            ["grand", "neuf", "ils regardent"],
            [
                ["grand", "petit", "sucré"],
                ["neuf", "deux", "trente"],
                ["ils regardent", "ils mangent", "ils dorment"]
            ]),
        new(
            "🏛️",
            "La Grand-Place",
            "Le cœur de Bruxelles",
            "La Grand-Place est la dernière étape. Zayd et Razan complètent leur carnet et célèbrent leur découverte.",
            ["Comment dit-on « la place » en français ?", "Quelle place célèbre visitent-ils ?", "Quelle phrase signifie « Bruxelles est magnifique » ?"],
            ["la place", "la Grand-Place", "Bruxelles est magnifique"],
            [
                ["la place", "la rue", "la gare"],
                ["la Grand-Place", "la place de la Concorde", "la place Rouge"],
                ["Bruxelles est magnifique", "Paris est loin", "Le train est fermé"]
            ])
    ];
}
