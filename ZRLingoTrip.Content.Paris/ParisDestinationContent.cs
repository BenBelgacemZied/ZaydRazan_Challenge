using ZRLingoTrip.Application;
using ZRLingoTrip.Application.Content;
using ZRLingoTrip.Domain.Content;
using ZRLingoTrip.Domain.Games;

namespace ZRLingoTrip.Content.Paris;

public sealed class ParisDestinationContent : IDestinationContent
{
    private static readonly string[] AdventureEmojis =
    [
        "🗺️", "🗼", "🔺", "🏛️", "🥖", "⛪", "🤍", "🎭", "🏛️", "✨",
        "🖼️", "🥐", "🎫", "🌊", "🎨", "🌳", "⛲", "🏘️", "🕰️", "🌈",
        "🎨", "🌉", "🖌️", "🚇", "🚤", "🍪", "🧢", "🥞", "🛠️", "🔬"
    ];

    private static readonly VocabularyWord[] EnglishJourneyWords =
    [
        new("hello", "bonjour", "Greetings"), new("thank you", "merci", "Greetings"),
        new("goodbye", "au revoir", "Greetings"), new("a bag", "un sac", "Travel"),
        new("a backpack", "un sac à dos", "Travel"), new("a suitcase", "une valise", "Travel"),
        new("a T-shirt", "un tee-shirt", "Clothing"), new("trousers", "un pantalon", "Clothing"),
        new("blue trousers", "un pantalon bleu", "Clothing"), new("a pink T-shirt", "un tee-shirt rose", "Clothing"),
        new("a hat", "un chapeau", "Clothing"), new("socks", "des chaussettes", "Clothing"),
        new("a book", "un livre", "Packing"), new("a toothbrush", "une brosse à dents", "Packing"),
        new("a water bottle", "une gourde", "Packing"), new("a camera", "un appareil photo", "Packing"),
        new("the passports", "les passeports", "Documents"), new("the tickets", "les billets", "Documents"),
        new("the map of Paris", "le plan de Paris", "Documents"), new("the travel journal", "le carnet de voyage", "Documents"),
        new("to pay", "payer", "At the station"), new("to travel", "voyager", "At the station"),
        new("the station", "la gare", "At the station"), new("the platform", "le quai", "At the station"),
        new("the train", "le train", "On the train"), new("the door", "la porte", "On the train"),
        new("the seat", "le siège", "On the train"), new("the luggage rack", "le porte-bagages", "On the train"),
        new("the window", "la fenêtre", "On the train"), new("the table", "la table", "On the train")
    ];

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

    private readonly ILearningLanguageSettings _languageSettings;

    public ParisDestinationContent(ILearningLanguageSettings languageSettings) => _languageSettings = languageSettings;

    public DestinationProfile Profile => new("paris", _languageSettings.SourceLanguageCode == "en" ? "Paris" : "Parijs", new("fr"));
    public LearningLanguagePair LanguagePair => new(new(_languageSettings.SourceLanguageCode), Profile.TargetLanguage);
    public int FirstStage => 5;
    public int Count => Quests.Count;

    private static DestinationClue C(string sentence, string highlight, string word, string other1, string other2) =>
        new(sentence, highlight, word, [word, other1, other2]);

    private static readonly DestinationQuest[] DutchQuests =
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

    private static readonly string[] EnglishStories = [
        "Zayd and Razan use the map to find their way around Paris. Together, they choose their next stop.",
        "The Eiffel Tower is made of iron. From the top, you can look out over Paris.",
        "Zayd and Razan see lots of art in the Louvre. The Mona Lisa is here too.",
        "The Arc de Triomphe is a large arch in a square in Paris.",
        "A baguette is a long, crispy loaf of bread. Zayd and Razan find one at the bakery.",
        "Notre-Dame is a large cathedral on an island in the River Seine.",
        "The white Sacré-Cœur Basilica stands high on the hill of Montmartre.",
        "Shows take place at the Opéra Garnier. Zayd admires the magnificent building.",
        "The Panthéon has a large dome. Famous people are remembered here.",
        "At Les Invalides, Zayd and Razan see a large golden dome.",
        "La Joconde is the French name for the Mona Lisa. You can see the painting in the Louvre.",
        "A croissant is a light, flaky pastry. Razan finds one in a bakery.",
        "Zayd and Razan travel to Paris with their train tickets. Keep your ticket safe.",
        "The Seine is the river that flows through Paris. Zayd and Razan walk beside the water.",
        "Montmartre is a neighbourhood on a hill. It has steep streets and the Sacré-Cœur.",
        "In the Luxembourg Gardens, Zayd and Razan see flowers, trees and a pond.",
        "The Place de la Concorde has a tall obelisk and fountains.",
        "The Place des Vosges is a square with arches and a green garden.",
        "The Musée d’Orsay used to be a railway station. Now Zayd and Razan see paintings there.",
        "At the Sainte-Chapelle, light shines through tall, colourful stained-glass windows.",
        "The Centre Pompidou displays modern art. You can see colourful pipes on the outside.",
        "The Pont Alexandre III is a decorated bridge over the Seine.",
        "Artists paint pictures in the Place du Tertre in Montmartre.",
        "The metro takes people quickly around Paris. Zayd and Razan look for the entrance.",
        "From a river boat, Zayd and Razan see the bridges of Paris.",
        "Macarons are small, colourful cookies. They are displayed in the bakery window.",
        "Zayd finds a beret, a soft, round hat. In French, it is called le béret.",
        "A crêpe is a thin pancake. Zayd and Razan choose one from a street stall.",
        "Gustave Eiffel was an engineer. His name is linked to the famous Eiffel Tower.",
        "Marie Curie was a scientist. She worked in Paris and carried out important research."
    ];

    private static readonly string[] EnglishClueSentences = [
        "Zayd is looking for the map.",
        "The street leads to the tower.",
        "Razan sees the city.",
        "The tower is made of iron.",
        "The tower is very tall.",
        "You can see the top of the tower.",
        "You can see art in the museum.",
        "The Mona Lisa hangs here.",
        "Zayd is looking for a painting.",
        "The large arch stands in a square.",
        "The arch is a monument.",
        "You can see a large arch.",
        "The baker makes fresh bread.",
        "You can buy bread at the bakery.",
        "A baguette is a long loaf of bread.",
        "A cathedral is a large church.",
        "Zayd sees a large window.",
        "The cathedral stands on an island.",
        "The basilica is white.",
        "Razan walks up the stairs.",
        "The building stands on a hill.",
        "Music can be heard in the hall.",
        "You can see a beautiful building.",
        "Razan watches a show.",
        "Zayd sees a large dome.",
        "This building is in Paris.",
        "We remember famous people.",
        "The dome is golden.",
        "Razan sees a museum.",
        "A soldier wears a uniform.",
        "This is a famous painting.",
        "The painting shows a woman.",
        "Zayd looks at a smile.",
        "The bakery sells pastries.",
        "The pastry is shaped like an arch.",
        "Zayd eats breakfast.",
        "You need a ticket to travel by train.",
        "The ticket is in a bag.",
        "Zayd sees a train.",
        "The water flows through the city.",
        "A river flows under the bridge.",
        "Zayd sees a bridge.",
        "Razan walks through a neighbourhood.",
        "The street is steep.",
        "There is a hill above.",
        "There are trees in the park.",
        "Razan smells a flower.",
        "There is a chair beside the water.",
        "A large square is in front of us.",
        "An obelisk stands in the middle.",
        "The fountain sprays water.",
        "You can see arches beneath the buildings.",
        "There is a garden in the middle of the square.",
        "The bricks are red.",
        "There used to be a station here.",
        "Now paintings hang here.",
        "You can see a clock on the building.",
        "Light shines through the window.",
        "The glass has many colours.",
        "Razan looks at the light.",
        "You can see pipes on the outside.",
        "Inside, you can see modern art.",
        "The building has many colours.",
        "The bridge crosses the river.",
        "You can see golden statues on top.",
        "Water flows under the bridge.",
        "An artist makes a drawing.",
        "There are easels in the square.",
        "Zayd looks at a painting.",
        "We are going to the entrance.",
        "A train runs beneath the city.",
        "A station has a platform.",
        "The boat travels along the river.",
        "We see a bridge on the way.",
        "The water glitters in the sun.",
        "The cookies have many colours.",
        "Razan chooses a small cookie.",
        "There are pastries in the shop.",
        "The hat is lying on the table.",
        "This fabric is soft.",
        "Razan sees a round shape.",
        "The pancake is thin.",
        "You can put fruit on it.",
        "Zayd smells a sweet crêpe.",
        "An engineer designs buildings.",
        "The tower is made of iron.",
        "Zayd reads a man's name.",
        "A scientist carries out research.",
        "A woman is working in the laboratory.",
        "She learns a lot about science."
    ];

    private static readonly IReadOnlyDictionary<string, string> EnglishClueWords = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["la carte"] = "the map",
            ["la rue"] = "the street",
            ["la ville"] = "the city",
            ["le fer"] = "iron",
            ["haute"] = "tall",
            ["le sommet"] = "the top",
            ["le musée"] = "the museum",
            ["la Joconde"] = "the Mona Lisa",
            ["le tableau"] = "a painting",
            ["la place"] = "a square",
            ["le monument"] = "a monument",
            ["un arc"] = "an arch",
            ["le boulanger"] = "the baker",
            ["la boulangerie"] = "the bakery",
            ["le pain"] = "bread",
            ["une église"] = "a church",
            ["une fenêtre"] = "a window",
            ["une île"] = "an island",
            ["blanc"] = "white",
            ["les escaliers"] = "the stairs",
            ["une colline"] = "a hill",
            ["la musique"] = "music",
            ["un bâtiment"] = "a building",
            ["un spectacle"] = "a show",
            ["un dôme"] = "a dome",
            ["des personnes"] = "people",
            ["doré"] = "golden",
            ["un musée"] = "a museum",
            ["un soldat"] = "a soldier",
            ["une femme"] = "a woman",
            ["un sourire"] = "a smile",
            ["une pâtisserie"] = "pastry",
            ["le petit-déjeuner"] = "breakfast",
            ["un billet"] = "a ticket",
            ["un sac"] = "a bag",
            ["un train"] = "a train",
            ["l’eau"] = "water",
            ["une rivière"] = "a river",
            ["un pont"] = "a bridge",
            ["un quartier"] = "a neighbourhood",
            ["des arbres"] = "trees",
            ["une fleur"] = "a flower",
            ["une chaise"] = "a chair",
            ["un obélisque"] = "an obelisk",
            ["une fontaine"] = "a fountain",
            ["des arcades"] = "arches",
            ["un jardin"] = "a garden",
            ["rouge"] = "red",
            ["une gare"] = "a station",
            ["des tableaux"] = "paintings",
            ["une horloge"] = "a clock",
            ["des couleurs"] = "colours",
            ["la lumière"] = "light",
            ["des tuyaux"] = "pipes",
            ["l’art"] = "art",
            ["des statues"] = "statues",
            ["un artiste"] = "an artist",
            ["une entrée"] = "an entrance",
            ["un quai"] = "a platform",
            ["un bateau"] = "a boat",
            ["le soleil"] = "the sun",
            ["un biscuit"] = "a cookie",
            ["une boutique"] = "a shop",
            ["un chapeau"] = "a hat",
            ["doux"] = "soft",
            ["rond"] = "round",
            ["fine"] = "thin",
            ["un fruit"] = "fruit",
            ["sucrée"] = "sweet",
            ["un ingénieur"] = "an engineer",
            ["un nom"] = "a name",
            ["la recherche"] = "research",
            ["la science"] = "science",
            ["un tableau"] = "a painting", ["une rue"] = "a street", ["une place"] = "a square"
        };

    private static readonly DestinationQuest[] EnglishQuests = BuildEnglishQuests();

    public IReadOnlyList<DestinationQuest> Quests =>
        _languageSettings.SourceLanguageCode == "en" ? EnglishQuests : DutchQuests;

    private static DestinationQuest[] BuildEnglishQuests()
    {
        var clueIndex = 0;
        return DutchQuests.Select((quest, index) =>
        {
            var translatedClues = quest.Clues.Select(clue => clue with
            {
                SourceSentence = EnglishClueSentences[clueIndex++],
                HighlightedSourceText = EnglishClueWords.GetValueOrDefault(clue.TargetWord, clue.TargetWord)
            }).ToArray();
            return quest with
            {
                Title = index == 0 ? "The city map" : quest.Title,
                Story = EnglishStories[index],
                Clues = translatedClues
            };
        }).ToArray();
    }

    public IReadOnlyList<PuzzleDefinition> Puzzles => Quests.Select((quest, index) =>
        new PuzzleDefinition(KeyFor(quest.Photo), quest.Title, quest.TargetName, AdventureEmojis[index], quest.Story, quest.Photo))
        .Append(new PuzzleDefinition("versailles",
            _languageSettings.SourceLanguageCode == "en" ? "the Palace of Versailles" : "het paleis van Versailles",
            "Le château de Versailles", "👑",
            _languageSettings.SourceLanguageCode == "en"
                ? "The Palace of Versailles was a royal residence. It is famous for its Hall of Mirrors and vast French gardens."
                : "Le château de Versailles was een koninklijk paleis. Het is bekend om zijn Spiegelzaal en grote Franse tuinen."))
        .ToArray();

    public IReadOnlyList<VocabularyWord> VocabularyWords => (_languageSettings.SourceLanguageCode == "en"
        ? EnglishJourneyWords
        : JourneyWords.Concat(Quests.SelectMany(quest => quest.Clues)
            .Select(clue => new VocabularyWord(clue.HighlightedSourceText, clue.TargetWord, Profile.DisplayName))))
        .GroupBy(word => word.TargetText.Trim().ToLowerInvariant())
        .Select(group => group.First())
        .OrderBy(word => word.Category)
        .ThenBy(word => word.SourceText)
        .ToArray();

    private static string KeyFor(string photo) =>
        Path.GetFileNameWithoutExtension(photo)
            .Replace("paris_", "", StringComparison.Ordinal)
            .Replace("_choice", "", StringComparison.Ordinal)
            .Replace("_puzzle", "", StringComparison.Ordinal);

    // Spread the correct answer across left, middle and right without showing the name early.
    public string[] PhotosFor(int index)
    {
        var first = Quests[(index + 7) % Count].Photo;
        var second = Quests[(index + 13) % Count].Photo;
        var correct = Quests[index].Photo;
        return (index % 3) switch
        {
            0 => new[] { correct, first, second },
            1 => new[] { first, correct, second },
            _ => new[] { first, second, correct }
        };
    }
}
