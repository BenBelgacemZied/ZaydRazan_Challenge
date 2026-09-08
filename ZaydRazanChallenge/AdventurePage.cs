namespace ZaydRazanChallenge;

public sealed class AdventurePage : ContentPage
{
    private sealed record Stage(
        string Emoji,
        string Place,
        string Narration,
        string FrenchPhrase,
        string Question,
        string CorrectAnswer,
        string[] Answers,
        string Color);

    private static readonly Stage[] Stages =
    [
        new("🏠", "Thuis · À la maison",
            "Zayd en Razan maken hun koffers klaar. Mama vraagt of alles klaar is.",
            "La valise est prête !",
            "Wat betekent « une valise » ?", "een koffer",
            ["een koffer", "een jas", "een boek"], "#DBEAFE"),

        new("🚉", "Het station · La gare",
            "Papa brengt hen naar het station. Op het bord staat het vertrek naar Parijs.",
            "Nous allons à la gare.",
            "Quel mot français signifie « het station » ?", "la gare",
            ["le train", "la gare", "la maison"], "#DCFCE7"),

        new("🚄", "In de trein · Dans le train",
            "De trein vertrekt. Razan kijkt door het raam en ziet de velden voorbijgaan.",
            "Le train est rapide.",
            "Wat betekent « le train » ?", "de trein",
            ["de trein", "de straat", "de toren"], "#EDE9FE"),

        new("🗺️", "Aankomst · Arrivée à Paris",
            "Zayd en Razan komen aan in Parijs. Ze openen de kaart en kiezen hun eerste bezoek.",
            "Bonjour Paris !",
            "Comment dit-on « goedendag » en français ?", "bonjour",
            ["merci", "bonjour", "au revoir"], "#FCE7F3"),

        new("🗼", "La tour Eiffel",
            "De grote ijzeren toren verschijnt voor hen. Zayd kijkt helemaal naar boven.",
            "La tour Eiffel est très haute.",
            "Quel mot signifie « hoog » ?", "haut",
            ["bas", "haut", "petit"], "#FEF3C7"),

        new("🖼️", "Le musée du Louvre",
            "In het Louvre zoeken ze samen naar het beroemde schilderij van Mona Lisa.",
            "Où est la Joconde ?",
            "Waar vind je de Mona Lisa ?", "au Louvre",
            ["à la gare", "au Louvre", "dans le train"], "#FFEDD5"),

        new("🥐", "La boulangerie",
            "Na het museum ruikt Razan vers brood. Ze bestellen beleefd bij de bakker.",
            "Un croissant, s'il vous plaît.",
            "Wat zeg je om beleefd iets te vragen ?", "s'il vous plaît",
            ["bonjour", "s'il vous plaît", "bonne nuit"], "#FEE2E2"),

        new("🏛️", "L’Arc de Triomphe",
            "De laatste halte is het Arc de Triomphe. Ze hebben Parijs stap voor stap ontdekt!",
            "Merci et au revoir, Paris !",
            "Que signifie « au revoir » ?", "tot ziens",
            ["dank je", "tot ziens", "goedemorgen"], "#CCFBF1")
    ];

    public static int StageCount => Stages.Length;

    private readonly Label _stars = new() { FontSize = 20, FontAttributes = FontAttributes.Bold };
    private readonly Label _step = new() { FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#2563EB") };
    private readonly Label _emoji = new() { FontSize = 68, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _place = new() { FontSize = 27, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _story = new() { FontSize = 17, LineHeight = 1.25 };
    private readonly Label _phrase = new() { FontSize = 22, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#7C3AED") };
    private readonly Label _question = new() { FontSize = 21, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _feedback = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly ProgressBar _progress = new() { ProgressColor = Color.FromArgb("#F59E0B"), HeightRequest = 9 };
    private readonly VerticalStackLayout _answers = new() { Spacing = 11 };
    private readonly Border _sceneCard;
    private int _stageIndex;
    private bool _answered;

    public AdventurePage()
    {
        Title = "Avontuur naar Parijs";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        _stageIndex = Math.Min(Preferences.Default.Get("adventure_stage", 0), Stages.Length - 1);

        var listenButton = new Button { Text = "🔊  Luister naar het Frans", BackgroundColor = Color.FromArgb("#7C3AED") };
        listenButton.Clicked += async (_, _) => await SpeakFrench();

        _sceneCard = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 28 },
            Padding = 20,
            Content = new VerticalStackLayout
            {
                Spacing = 13,
                Children = { _step, _emoji, _place, _story, _phrase, listenButton }
            }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 18,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                        Children =
                        {
                            new Label { Text = "🗺️ Route naar Parijs", FontSize = 20, FontAttributes = FontAttributes.Bold },
                            _stars
                        }
                    },
                    _progress, _sceneCard, _question, _answers, _feedback
                }
            }
        };

        Grid.SetColumn(_stars, 1);
        ShowStage();
    }

    private void ShowStage()
    {
        _answered = false;
        var stage = Stages[_stageIndex];
        _stars.Text = $"⭐ {Preferences.Default.Get("stars", 0)}";
        _step.Text = $"ETAPPE {_stageIndex + 1} / {Stages.Length}";
        _progress.Progress = (double)(_stageIndex + 1) / Stages.Length;
        _emoji.Text = stage.Emoji;
        _place.Text = stage.Place;
        _story.Text = stage.Narration;
        _phrase.Text = $"« {stage.FrenchPhrase} »";
        _question.Text = stage.Question;
        _feedback.Text = "";
        _sceneCard.BackgroundColor = Color.FromArgb(stage.Color);
        _answers.Clear();

        foreach (var answer in stage.Answers.OrderBy(_ => Random.Shared.Next()))
        {
            var button = new Button { Text = answer, BackgroundColor = Color.FromArgb("#2563EB") };
            button.Clicked += (_, _) => CheckAnswer(answer, button);
            _answers.Add(button);
        }
    }

    private async void CheckAnswer(string answer, Button selected)
    {
        if (_answered) return;
        _answered = true;

        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = false;

        var correct = answer == Stages[_stageIndex].CorrectAnswer;
        var stars = Preferences.Default.Get("stars", 0);
        stars = correct ? stars + 1 : Math.Max(0, stars - 1);
        Preferences.Default.Set("stars", stars);
        _stars.Text = $"⭐ {stars}";

        selected.BackgroundColor = correct ? Color.FromArgb("#16A34A") : Color.FromArgb("#DC2626");
        _feedback.TextColor = selected.BackgroundColor;
        _feedback.Text = correct
            ? "Bravo! +1 ⭐  Nieuwe etappe ontgrendeld!"
            : "Bijna! −1 ⭐  De volgende etappe wordt toch geopend.";

        await _sceneCard.ScaleTo(1.03, 180, Easing.CubicOut);
        await _sceneCard.ScaleTo(1, 180, Easing.CubicIn);
        await Task.Delay(1300);

        var nextStage = _stageIndex + 1;
        Preferences.Default.Set("adventure_stage", Math.Min(nextStage, Stages.Length));

        if (nextStage >= Stages.Length)
        {
            var replay = await DisplayAlert("🏆 Avontuur voltooid!",
                $"Zayd en Razan hebben Parijs bereikt met {stars} sterren.",
                "Opnieuw spelen", "Naar start");
            if (replay)
            {
                Preferences.Default.Set("adventure_stage", 0);
                _stageIndex = 0;
                ShowStage();
            }
            else
            {
                await Navigation.PopAsync();
            }
            return;
        }

        _stageIndex = nextStage;
        ShowStage();
        await ScrollToTop();
    }

    private async Task SpeakFrench()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var french = locales.FirstOrDefault(x =>
            x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
        await TextToSpeech.Default.SpeakAsync(Stages[_stageIndex].FrenchPhrase,
            new SpeechOptions { Locale = french });
    }

    private async Task ScrollToTop()
    {
        if (Content is ScrollView scroll)
            await scroll.ScrollToAsync(0, 0, true);
    }
}
