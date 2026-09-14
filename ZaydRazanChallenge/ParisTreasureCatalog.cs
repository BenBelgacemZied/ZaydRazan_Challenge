namespace ZaydRazanChallenge;

internal sealed record ParisClue(string DutchSentence, string Highlight, string Word, string[] Choices);
internal sealed record ParisQuest(string Title, string Target, string Photo, string Story, ParisClue[] Clues);

internal static class ParisTreasureCatalog
{
    public const int FirstStage = 4;
    public static int Count => Quests.Length;
    private static ParisClue C(string sentence, string highlight, string word, string other1, string other2) =>
        new(sentence, highlight, word, [word, other1, other2]);

    public static readonly ParisQuest[] Quests =
    [
        new("De stadskaart", "la carte", "paris_map_choice.jpg", "Op de kaart vinden Zayd en Razan de weg door Parijs. Samen kiezen ze een nieuwe halte.",
            [
                C("Zayd zoekt de kaart.", "kaart", "la carte", "le pain", "la porte"),
                C("De straat leidt naar de toren.", "straat", "la rue", "la gare", "la table"),
                C("Razan ziet de stad.", "stad", "la ville", "la valise", "la porte")
            ]),
        new("La tour Eiffel", "la tour Eiffel", "eiffel_puzzle.jpg", "De Eiffeltoren is van ijzer. Vanaf de top kijk je over Parijs.",
            [
                C("De toren is van ijzer.", "ijzer", "le fer", "le bois", "le pain"),
                C("De toren is heel hoog.", "hoog", "haute", "petite", "rouge"),
                C("Je ziet de top van de toren.", "top", "le sommet", "le quai", "le parc")
            ]),
        new("Le musée du Louvre", "le Louvre", "louvre_puzzle.jpg", "In het Louvre zien Zayd en Razan veel kunst. Ook de Mona Lisa hangt hier.",
            [
                C("In het museum zie je kunst.", "museum", "le musée", "le train", "le parc"),
                C("Hier hangt de Mona Lisa.", "Mona Lisa", "la Joconde", "la fenêtre", "la valise"),
                C("Zayd zoekt een schilderij.", "schilderij", "le tableau", "le billet", "le train")
            ]),
        new("L'Arc de Triomphe", "l'Arc de Triomphe", "arc_puzzle.jpg", "De Arc de Triomphe is een grote boog op een plein in Parijs.",
            [
                C("De grote boog staat aan een plein.", "plein", "la place", "la table", "la porte"),
                C("De boog is een monument.", "monument", "le monument", "le musée", "le train"),
                C("Je ziet een grote boog.", "boog", "un arc", "un quai", "un pain")
            ]),
        new("La baguette", "la baguette", "baguette_puzzle.jpg", "De baguette is een lang, knapperig brood. Zayd en Razan vinden het in de bakkerij.",
            [
                C("De bakker maakt vers brood.", "bakker", "le boulanger", "le musée", "le quai"),
                C("In de bakkerij koop je brood.", "bakkerij", "la boulangerie", "la gare", "la tour"),
                C("Een baguette is lang brood.", "brood", "le pain", "le train", "le siège")
            ]),
        new("Notre-Dame de Paris", "Notre-Dame", "notre_dame_puzzle.jpg", "Notre-Dame is een grote kathedraal op een eiland in de Seine.",
            [
                C("Een kathedraal is een grote kerk.", "kerk", "une église", "une gare", "une table"),
                C("Zayd ziet een groot raam.", "raam", "une fenêtre", "un billet", "une valise"),
                C("De kathedraal staat op een eiland.", "eiland", "une île", "une rue", "une porte")
            ]),
        new("Le Sacré-Cœur", "le Sacré-Cœur", "sacre_coeur_puzzle.jpg", "De witte basiliek Sacré-Cœur staat hoog op de heuvel van Montmartre.",
            [
                C("De basiliek is wit.", "wit", "blanc", "rouge", "vert"),
                C("Razan loopt de trappen op.", "trappen", "les escaliers", "les billets", "les livres"),
                C("Het gebouw staat op een heuvel.", "heuvel", "une colline", "une rivière", "une gare")
            ]),
        new("L'Opéra Garnier", "l'Opéra Garnier", "opera_puzzle.jpg", "In de Opéra Garnier worden voorstellingen gegeven. Zayd bewondert het grote gebouw.",
            [
                C("In de zaal klinkt muziek.", "muziek", "la musique", "la rivière", "la table"),
                C("Je ziet een mooi gebouw.", "gebouw", "un bâtiment", "un bateau", "un pain"),
                C("Razan ziet een voorstelling.", "voorstelling", "un spectacle", "un billet", "un jardin")
            ]),
        new("Le Panthéon", "le Panthéon", "pantheon_puzzle.jpg", "Het Panthéon heeft een grote koepel. Hier worden bekende mensen herdacht.",
            [
                C("Zayd ziet een grote koepel.", "koepel", "un dôme", "un pont", "un train"),
                C("Dit gebouw staat in Parijs.", "gebouw", "un bâtiment", "un bateau", "un sac"),
                C("We denken aan bekende mensen.", "mensen", "des personnes", "des pommes", "des tables")
            ]),
        new("Les Invalides", "les Invalides", "invalides_puzzle.jpg", "Bij Les Invalides zien Zayd en Razan een grote gouden koepel.",
            [
                C("De koepel is goudkleurig.", "goud", "doré", "bleu", "vert"),
                C("Razan ziet een museum.", "museum", "un musée", "un café", "un train"),
                C("Een soldaat draagt een uniform.", "soldaat", "un soldat", "un boulanger", "un peintre")
            ]),
        new("La Joconde", "la Joconde", "joconde_puzzle.jpg", "La Joconde is de Franse naam van de Mona Lisa. Je kunt het schilderij in het Louvre zien.",
            [
                C("Dit is een beroemd schilderij.", "schilderij", "un tableau", "un train", "un jardin"),
                C("Het schilderij toont een vrouw.", "vrouw", "une femme", "un homme", "un enfant"),
                C("Zayd kijkt naar een glimlach.", "glimlach", "un sourire", "un pont", "un quai")
            ]),
        new("Le croissant", "le croissant", "croissant_puzzle.jpg", "Een croissant is een luchtig gebakje. Razan vindt het in een bakkerij.",
            [
                C("De bakkerij verkoopt gebak.", "gebak", "une pâtisserie", "une gare", "une rivière"),
                C("Het broodje heeft een boogvorm.", "boog", "un arc", "un siège", "un train"),
                C("Zayd eet zijn ontbijt.", "ontbijt", "le petit-déjeuner", "le dîner", "le bateau")
            ]),
        new("Le billet de train", "le billet", "paris_ticket_choice.jpg", "Met hun treintickets reizen Zayd en Razan naar Parijs. Bewaar het ticket goed.",
            [
                C("Voor de trein heb je een ticket nodig.", "ticket", "un billet", "un jardin", "un pont"),
                C("Het ticket ligt in een tas.", "tas", "un sac", "un parc", "un pain"),
                C("Zayd ziet een trein.", "trein", "un train", "une tour", "une table")
            ]),
        new("La Seine", "la Seine", "paris_seine.jpg", "De Seine is de rivier die door Parijs stroomt. Zayd en Razan wandelen langs het water.",
            [
                C("Het water stroomt door de stad.", "water", "l’eau", "le pain", "la rue"),
                C("Een rivier gaat onder de brug.", "rivier", "une rivière", "une gare", "une école"),
                C("Zayd ziet een brug.", "brug", "un pont", "un livre", "un sac")
            ]),
        new("Montmartre", "Montmartre", "paris_montmartre.jpg", "Montmartre is een wijk op een heuvel. Je vindt er steile straten en het Sacré-Cœur.",
            [
                C("Razan loopt door een wijk.", "wijk", "un quartier", "un billet", "un tableau"),
                C("De straat is steil.", "straat", "une rue", "une gare", "une porte"),
                C("Boven ligt een heuvel.", "heuvel", "une colline", "une rivière", "une table")
            ]),
        new("Le jardin du Luxembourg", "le jardin du Luxembourg", "paris_luxembourg.jpg", "In de Jardin du Luxembourg zien Zayd en Razan bloemen, bomen en een vijver.",
            [
                C("In het park staan bomen.", "bomen", "des arbres", "des trains", "des billets"),
                C("Razan ruikt een bloem.", "bloem", "une fleur", "une tour", "une gare"),
                C("Bij het water staat een stoel.", "stoel", "une chaise", "un pont", "un train")
            ]),
        new("La place de la Concorde", "la place de la Concorde", "paris_concorde.jpg", "Op de Place de la Concorde staan een hoge obelisk en fonteinen.",
            [
                C("Een groot plein ligt voor ons.", "plein", "une place", "une table", "une rue"),
                C("In het midden staat een obelisk.", "obelisk", "un obélisque", "un bateau", "un jardin"),
                C("De fontein geeft water.", "fontein", "une fontaine", "une fenêtre", "une valise")
            ]),
        new("La place des Vosges", "la place des Vosges", "paris_vosges.jpg", "De Place des Vosges is een plein met bogen en een groene tuin.",
            [
                C("Onder de gebouwen zie je bogen.", "bogen", "des arcades", "des bateaux", "des billets"),
                C("Midden op het plein ligt een tuin.", "tuin", "un jardin", "un train", "un sac"),
                C("De bakstenen zijn rood.", "rood", "rouge", "bleu", "jaune")
            ]),
        new("Le musée d'Orsay", "le musée d'Orsay", "paris_orsay.jpg", "Het Musée d'Orsay was vroeger een station. Nu zien Zayd en Razan er schilderijen.",
            [
                C("Vroeger stond hier een station.", "station", "une gare", "une école", "une tour"),
                C("Nu hangen er schilderijen.", "schilderijen", "des tableaux", "des billets", "des trains"),
                C("Op het gebouw zie je een klok.", "klok", "une horloge", "une valise", "une rue")
            ]),
        new("La Sainte-Chapelle", "la Sainte-Chapelle", "paris_chapelle.jpg", "In de Sainte-Chapelle schijnt licht door hoge, gekleurde glasramen.",
            [
                C("Het licht komt door het raam.", "raam", "une fenêtre", "un jardin", "un bateau"),
                C("Het glas heeft veel kleuren.", "kleuren", "des couleurs", "des tickets", "des tables"),
                C("Razan kijkt naar het licht.", "licht", "la lumière", "la gare", "la rue")
            ]),
        new("Le Centre Pompidou", "le Centre Pompidou", "paris_pompidou.jpg", "Het Centre Pompidou toont moderne kunst. Aan de buitenkant zie je gekleurde buizen.",
            [
                C("Aan de buitenkant zie je buizen.", "buizen", "des tuyaux", "des fleurs", "des trains"),
                C("Binnen zie je moderne kunst.", "kunst", "l’art", "l’eau", "le pain"),
                C("Het gebouw heeft veel kleuren.", "kleuren", "des couleurs", "des billets", "des pommes")
            ]),
        new("Le pont Alexandre III", "le pont Alexandre III", "paris_pont.jpg", "De Pont Alexandre III is een versierde brug over de Seine.",
            [
                C("De brug gaat over de rivier.", "brug", "un pont", "un train", "un livre"),
                C("Bovenop zie je gouden beelden.", "beelden", "des statues", "des valises", "des arbres"),
                C("Onder de brug stroomt water.", "water", "l’eau", "le pain", "le lait")
            ]),
        new("La place du Tertre", "la place du Tertre", "paris_tertre.jpg", "Op de Place du Tertre in Montmartre maken kunstenaars schilderijen.",
            [
                C("Een kunstenaar maakt een tekening.", "kunstenaar", "un artiste", "un soldat", "un marin"),
                C("Op het plein staan schildersezels.", "plein", "une place", "une gare", "une porte"),
                C("Zayd kijkt naar een schilderij.", "schilderij", "un tableau", "un bateau", "un train")
            ]),
        new("Le métro parisien", "le métro", "paris_metro.jpg", "De metro brengt mensen snel door Parijs. Zayd en Razan zoeken de ingang.",
            [
                C("We gaan naar de ingang.", "ingang", "une entrée", "une fenêtre", "une gare"),
                C("Onder de stad rijdt een trein.", "trein", "un train", "un pont", "un jardin"),
                C("Een halte heeft een perron.", "perron", "un quai", "un pain", "un livre")
            ]),
        new("Le bateau-mouche", "le bateau-mouche", "paris_bateau.jpg", "Vanuit een rondvaartboot zien Zayd en Razan de bruggen van Parijs.",
            [
                C("De boot vaart op de rivier.", "boot", "un bateau", "un vélo", "un bus"),
                C("Onderweg zien we een brug.", "brug", "un pont", "un train", "un livre"),
                C("Het water glinstert in de zon.", "zon", "le soleil", "la lune", "la gare")
            ]),
        new("Les macarons", "les macarons", "paris_macarons.jpg", "Macarons zijn kleine kleurrijke koekjes. Ze liggen in de vitrine van de bakkerij.",
            [
                C("De koekjes hebben veel kleuren.", "kleuren", "des couleurs", "des trains", "des rues"),
                C("Razan kiest een klein koekje.", "koekje", "un biscuit", "un billet", "un bateau"),
                C("In de winkel staan gebakjes.", "winkel", "une boutique", "une rivière", "une gare")
            ]),
        new("Le béret", "le béret", "paris_beret.jpg", "Zayd vindt een baret, een zachte ronde muts. In het Frans heet die le béret.",
            [
                C("De muts ligt op de tafel.", "muts", "un chapeau", "un pont", "un billet"),
                C("Deze stof is zacht.", "zacht", "doux", "grand", "rouge"),
                C("Razan ziet een ronde vorm.", "rond", "rond", "haut", "long")
            ]),
        new("La crêpe", "la crêpe", "paris_crepe.jpg", "Een crêpe is een dunne pannenkoek. Zayd en Razan kiezen er een bij een kraampje.",
            [
                C("De pannenkoek is dun.", "dun", "fine", "haute", "rouge"),
                C("Je kunt er fruit op leggen.", "fruit", "un fruit", "un train", "un livre"),
                C("Zayd ruikt een zoete crêpe.", "zoet", "sucrée", "salée", "froide")
            ]),
        new("Gustave Eiffel", "Gustave Eiffel", "paris_eiffel_person.jpg", "Gustave Eiffel was een ingenieur. Zijn naam hoort bij de beroemde Eiffeltoren.",
            [
                C("Een ingenieur ontwerpt gebouwen.", "ingenieur", "un ingénieur", "un boulanger", "un marin"),
                C("De toren is van ijzer.", "ijzer", "le fer", "le bois", "le pain"),
                C("Zayd leest de naam van een man.", "naam", "un nom", "un pont", "un train")
            ]),
        new("Marie Curie", "Marie Curie", "paris_curie.jpg", "Marie Curie was een wetenschapster. Zij werkte in Parijs en deed belangrijk onderzoek.",
            [
                C("Een wetenschapster doet onderzoek.", "onderzoek", "la recherche", "la musique", "la cuisine"),
                C("In het laboratorium werkt een vrouw.", "vrouw", "une femme", "un homme", "un enfant"),
                C("Zij leert veel over wetenschap.", "wetenschap", "la science", "la peinture", "la danse")
            ]),
    ];

    // Spread the correct answer across left, middle and right without showing the name early.
    public static string[] PhotosFor(int index)
    {
        var first = Quests[(index + 7) % Count].Photo;
        var second = Quests[(index + 13) % Count].Photo;
        var correct = Quests[index].Photo;
        return index % 3 switch
        {
            0 => [correct, first, second],
            1 => [first, correct, second],
            _ => [first, second, correct]
        };
    }
}
