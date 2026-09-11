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
        string Color,
        double MapX,
        double MapY);

    private static readonly Stage[] Stages =
    [
        new("🎒", "Thuis · À la maison",
            "Zayd en Razan controleren zelf hun rugzakken, tickets en paspoorten. Hun avontuur kan beginnen!",
            "Nos sacs sont prêts !",
            "Wat betekent « un sac »?", "een tas",
            ["een tas", "een trein", "een kaart"], "#FEF3C7", .59, .86),

        new("🧭", "Op weg · En route",
            "Zayd leest de kaart en Razan kiest het pad naar het station. Samen vinden ze de juiste richting.",
            "Où est la gare ?",
            "Welk Frans woord betekent « waar »?", "où",
            ["où", "quand", "merci"], "#FFEDD5", .63, .69),

        new("🚉", "Het station · La gare",
            "Razan vindt het juiste perron op het vertrekbord. Zayd bewaart de tickets voor de controle.",
            "Le train part du quai trois.",
            "Wat betekent « le quai »?", "het perron",
            ["het perron", "de koffer", "de toren"], "#DCFCE7", .69, .57),

        new("🚄", "In de trein · Dans le train",
            "Ze stappen zelfstandig in, zoeken hun zitplaatsen en zien het landschap snel voorbijgaan.",
            "Nous voyageons en train.",
            "Hoe zeg je « wij reizen » in het Frans?", "nous voyageons",
            ["nous voyageons", "nous mangeons", "nous dormons"], "#DBEAFE", .49, .46),

        new("🗺️", "Aankomst · Arrivée à Paris",
            "Zayd en Razan komen aan in Parijs. Ze openen hun stadskaart en kiezen samen de volgende halte.",
            "Bonjour Paris, nous sommes arrivés !",
            "Wat betekent « arrivés »?", "aangekomen",
            ["verdwaald", "aangekomen", "vertrokken"], "#EDE9FE", .43, .36),

        new("🗼", "La tour Eiffel",
            "Ze volgen de route langs de Seine. Razan ontdekt als eerste de top van de Eiffeltoren.",
            "La tour Eiffel est très haute.",
            "Welk Frans woord betekent « hoog »?", "haut",
            ["bas", "haut", "petit"], "#FCE7F3", .36, .25),

        new("🖼️", "Le musée du Louvre",
            "Zayd kiest de museumroute. Samen zoeken ze tussen de kunstwerken naar de beroemde Joconde.",
            "Où est la Joconde ?",
            "Waar vinden ze de Mona Lisa?", "au Louvre",
            ["à la gare", "au Louvre", "dans le train"], "#FFEDD5", .63, .35),

        new("🏛️", "L’Arc de Triomphe",
            "De wolken verdwijnen: Zayd en Razan bereiken zelfstandig de laatste halte van hun Parijse avontuur!",
            "Merci et au revoir, Paris !",
            "Wat betekent « au revoir »?", "tot ziens",
            ["dank je", "tot ziens", "goedemorgen"], "#CCFBF1", .72, .18)
    ];

    public static int StageCount => Stages.Length;

    private readonly Label _stars = new()
    {
        FontSize = 20,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White
    };
    private readonly Label _energy = new()
    {
        Text = "⚡ 10",
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White
    };
    private readonly Label _step = new()
    {
        FontSize = 14,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#2563EB")
    };
    private readonly Label _place = new()
    {
        FontSize = 25,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly Label _story = new() { FontSize = 16, LineHeight = 1.25 };
    private readonly Label _phrase = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        TextColor = Color.FromArgb("#7C3AED")
    };
    private readonly Label _question = new()
    {
        FontSize = 20,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly Label _feedback = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly ProgressBar _progress = new()
    {
        ProgressColor = Color.FromArgb("#F59E0B"),
        BackgroundColor = Color.FromArgb("#DBEAFE"),
        HeightRequest = 9
    };
    private readonly VerticalStackLayout _answers = new() { Spacing = 9 };
    private readonly AbsoluteLayout _mapLayer = new();
    private readonly Border _missionCard;
    private readonly ScrollView _scroll;
    private Label? _heroes;
    private readonly double _mapWidth;
    private readonly double _mapHeight;
    private int _stageIndex;
    private bool _answered;

    public AdventurePage()
    {
        Title = "Avontuur naar Parijs";
        BackgroundColor = Color.FromArgb("#E0F2FE");
        GameUi.AddHomeButton(this);
        _stageIndex = Math.Min(Preferences.Default.Get("adventure_stage", 0), Stages.Length - 1);

        var display = DeviceDisplay.Current.MainDisplayInfo;
        _mapWidth = Math.Min(430d, Math.Max(300d, display.Width / display.Density));
        _mapHeight = _mapWidth * 1.5;

        var listenButton = new Button
        {
            Text = "🔊 Luister",
            BackgroundColor = Color.FromArgb("#7C3AED"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold
        };
        listenButton.Clicked += async (_, _) => await SpeakFrench();

        _missionCard = new Border
        {
            IsVisible = false,
            Margin = new Thickness(14, -22, 14, 18),
            Stroke = Colors.White,
            StrokeThickness = 3,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 },
            Padding = 18,
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Opacity = .22f,
                Radius = 12,
                Offset = new Point(0, 5)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 11,
                Children =
                {
                    _step, _progress, _place, _story, _phrase,
                    listenButton, _question, _answers, _feedback
                }
            }
        };

        var hud = new Border
        {
            Margin = 10,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            Padding = new Thickness(14, 9),
            StrokeThickness = 0,
            BackgroundColor = Color.FromArgb("#CC17324D"),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto)
                },
                ColumnSpacing = 18,
                Children =
                {
                    new Label
                    {
                        Text = "🗺️ Zayd & Razan",
                        TextColor = Colors.White,
                        FontSize = 19,
                        FontAttributes = FontAttributes.Bold
                    },
                    _energy,
                    _stars
                }
            }
        };
        Grid.SetColumn(_energy, 1);
        Grid.SetColumn(_stars, 2);

        var mapContainer = new Grid
        {
            WidthRequest = _mapWidth,
            HeightRequest = _mapHeight,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Image
                {
                    Source = "adventure_map.jpg",
                    Aspect = Aspect.AspectFill,
                    WidthRequest = _mapWidth,
                    HeightRequest = _mapHeight
                },
                _mapLayer,
                hud
            }
        };

        _scroll = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children = { mapContainer, _missionCard }
            }
        };
        Content = _scroll;
        ShowStage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var savedStage = Math.Min(
            Preferences.Default.Get("adventure_stage", 0),
            Stages.Length - 1);
        if (savedStage != _stageIndex)
        {
            _stageIndex = savedStage;
            ShowStage();
        }
    }

    private void RenderMap()
    {
        _mapLayer.Clear();

        for (var i = 0; i < Stages.Length; i++)
        {
            var stageNumber = i;
            var unlocked = i <= _stageIndex;
            var completed = i < _stageIndex;
            var current = i == _stageIndex;
            var marker = new Button
            {
                Text = (i + 1).ToString(),
                FontSize = current ? 25 : 20,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 30,
                WidthRequest = current ? 62 : 52,
                HeightRequest = current ? 62 : 52,
                Padding = 0,
                BackgroundColor = completed
                    ? Color.FromArgb("#22C55E")
                    : current ? Color.FromArgb("#F59E0B") : Color.FromArgb("#94A3B8"),
                TextColor = Colors.White,
                BorderColor = Colors.White,
                BorderWidth = 4,
                IsEnabled = unlocked
            };
            marker.Clicked += async (_, _) =>
            {
                if (stageNumber == _stageIndex)
                {
                    if (stageNumber == 0)
                        await Navigation.PushAsync(new AdventureMissionHubPage());
                    else if (stageNumber == 2)
                        await Navigation.PushAsync(new StationMissionHubPage());
                    else if (stageNumber < 4)
                        await Navigation.PushAsync(new AdventureScenePage(stageNumber));
                    else
                    {
                        _missionCard.IsVisible = true;
                        await _missionCard.FadeTo(1, 180);
                        await _scroll.ScrollToAsync(_missionCard, ScrollToPosition.Start, true);
                    }
                }
                else if (stageNumber < _stageIndex)
                    await DisplayAlert($"Etappe {stageNumber + 1} · {Stages[stageNumber].Place}",
                        "Deze etappe is al voltooid ✓", "Verder");
            };

            var size = current ? 62d : 52d;
            AbsoluteLayout.SetLayoutBounds(marker, new Rect(
                Stages[i].MapX * _mapWidth - size / 2,
                Stages[i].MapY * _mapHeight - size / 2,
                size, size));
            _mapLayer.Add(marker);
        }

        var active = Stages[_stageIndex];
        _heroes = new Label
        {
            Text = "👦🏽👧🏽",
            FontSize = 31,
            HorizontalTextAlignment = TextAlignment.Center,
            WidthRequest = 78,
            HeightRequest = 46
        };
        AbsoluteLayout.SetLayoutBounds(_heroes, new Rect(
            active.MapX * _mapWidth - 39,
            active.MapY * _mapHeight - 64,
            78, 46));
        _mapLayer.Add(_heroes);
    }

    private void ShowStage()
    {
        _answered = false;
        _missionCard.IsVisible = false;
        _missionCard.Opacity = 0;
        var stage = Stages[_stageIndex];
        _stars.Text = $"⭐ {Preferences.Default.Get("stars", 0)}";
        _step.Text = $"MISSIE {_stageIndex + 1} / {Stages.Length}";
        _progress.Progress = (double)(_stageIndex + 1) / Stages.Length;
        _place.Text = $"{stage.Emoji} {stage.Place}";
        _story.Text = stage.Narration;
        _phrase.Text = $"« {stage.FrenchPhrase} »";
        _question.Text = stage.Question;
        _feedback.Text = "";
        _missionCard.BackgroundColor = Color.FromArgb(stage.Color);
        _answers.Clear();
        RenderMap();

        foreach (var answer in stage.Answers.OrderBy(_ => Random.Shared.Next()))
        {
            var button = new Button
            {
                Text = answer,
                BackgroundColor = Color.FromArgb("#2563EB"),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold
            };
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
        _energy.Text = correct ? "⚡ 10" : "⚡ 9";

        selected.BackgroundColor = correct
            ? Color.FromArgb("#16A34A")
            : Color.FromArgb("#DC2626");
        await (correct ? GameFeedback.SuccessAsync() : GameFeedback.FailureAsync());
        _feedback.TextColor = selected.BackgroundColor;
        _feedback.Text = correct
            ? "Bravo! +1 ⭐ De wolken verdwijnen..."
            : "Bijna! −1 ⭐ Zayd en Razan leren van hun fout.";

        await _missionCard.ScaleTo(1.025, 160, Easing.CubicOut);
        await _missionCard.ScaleTo(1, 160, Easing.CubicIn);

        var nextStage = _stageIndex + 1;
        if (nextStage < Stages.Length && _heroes is not null)
        {
            var current = Stages[_stageIndex];
            var next = Stages[nextStage];
            await _scroll.ScrollToAsync(0, 0, true);
            await _heroes.TranslateTo(
                (next.MapX - current.MapX) * _mapWidth,
                (next.MapY - current.MapY) * _mapHeight,
                900,
                Easing.CubicInOut);
            await Task.Delay(350);
        }

        Preferences.Default.Set("adventure_stage", Math.Min(nextStage, Stages.Length));

        if (nextStage >= Stages.Length)
        {
            var replay = await DisplayAlert(
                "🏆 Parijs ontdekt!",
                $"Zayd en Razan hebben hun reis helemaal zelf voltooid met {stars} sterren.",
                "Opnieuw spelen",
                "Naar start");
            if (replay)
            {
                Preferences.Default.Set("adventure_stage", 0);
                _stageIndex = 0;
                ShowStage();
                await _scroll.ScrollToAsync(0, 0, true);
            }
            else
                await Navigation.PopAsync();
            return;
        }

        _stageIndex = nextStage;
        ShowStage();
        await _scroll.ScrollToAsync(0, 0, true);
    }

    private async Task SpeakFrench()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var french = locales.FirstOrDefault(x =>
            x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
        await TextToSpeech.Default.SpeakAsync(
            Stages[_stageIndex].FrenchPhrase,
            new SpeechOptions { Locale = french });
    }
}
