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
            "Hoe zeg je « een tas » in het Frans?", "un sac",
            ["un sac", "un train", "une carte"], "#FEF3C7", .59, .86),

        new("🧭", "Op weg · En route",
            "Zayd leest de kaart en Razan kiest het pad naar het station. Samen vinden ze de juiste richting.",
            "Où est la gare ?",
            "Welk Frans woord betekent « waar »?", "où",
            ["où", "quand", "merci"], "#FFEDD5", .63, .69),

        new("🚉", "Het station · La gare",
            "Razan vindt het juiste perron op het vertrekbord. Zayd bewaart de tickets voor de controle.",
            "Le train part du quai trois.",
            "Hoe zeg je « het perron » in het Frans?", "le quai",
            ["le quai", "la valise", "la tour"], "#DCFCE7", .69, .57),

        new("🚄", "In de trein · Dans le train",
            "Ze stappen zelfstandig in, zoeken hun zitplaatsen en zien het landschap snel voorbijgaan.",
            "Nous voyageons en train.",
            "Hoe zeg je « wij reizen » in het Frans?", "nous voyageons",
            ["nous voyageons", "nous mangeons", "nous dormons"], "#DBEAFE", .49, .46),

        new("🗺️", "Aankomst · Arrivée à Paris",
            "Zayd en Razan komen aan in Parijs. Ze openen hun stadskaart en kiezen samen de volgende halte.",
            "Bonjour Paris, nous sommes arrivés !",
            "Welk Frans woord betekent « aangekomen »?", "arrivés",
            ["perdus", "arrivés", "partis"], "#EDE9FE", .43, .36),

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
            "Welke Franse uitdrukking betekent « tot ziens »?", "au revoir",
            ["merci", "au revoir", "bonjour"], "#CCFBF1", .72, .18),

        new("🥖", "La boulangerie · La baguette",
            "Zayd en Razan ontdekken een warme bakkerij. Ze zoeken samen een knapperige baguette.",
            "Une baguette, s’il vous plaît.",
            "Hoe zeg je « stokbrood » in het Frans?", "la baguette",
            ["la baguette", "la carte", "le billet"], "#FEF3C7", .55, .11)
    ];

    public static int StageCount => ParisTreasureCatalog.FirstStage + ParisTreasureCatalog.Count;

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
    private readonly Image _mapImage = new() { Source = "paris_letter_scene.jpg", Aspect = Aspect.AspectFill };
    private readonly HorizontalStackLayout _chapterTabs = new() { Spacing = 8 };
    private readonly HorizontalStackLayout _routeStops = new() { Spacing = 8 };
    private readonly Label _routeTitle = new()
    {
        FontSize = 20,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#17324D")
    };
    private readonly Label _routeProgressText = new()
    {
        FontSize = 14,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#2563EB")
    };
    private readonly ProgressBar _journeyProgress = new()
    {
        ProgressColor = Color.FromArgb("#F59E0B"),
        BackgroundColor = Color.FromArgb("#DBEAFE"),
        HeightRequest = 8
    };
    private readonly Border _missionCard;
    private readonly ScrollView _scroll;
    private int _stageIndex;
    private int _selectedChapter;
    private readonly int? _testStage;
    private bool _answered;

    public AdventurePage(int? testStage = null)
    {
        _testStage = testStage;
        if (testStage is int stage && (stage < ParisTreasureCatalog.FirstStage || stage >= StageCount))
            throw new ArgumentOutOfRangeException(nameof(testStage));
        Title = "Avontuur naar Parijs";
        BackgroundColor = Color.FromArgb("#E0F2FE");
        GameUi.AddHomeButton(this);
        _stageIndex = testStage ?? CurrentSavedStage();

        var display = DeviceDisplay.Current.MainDisplayInfo;
        var logicalHeight = display.Height / display.Density;
        var heroHeight = Math.Max(360d, logicalHeight * .55d);

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

        var hero = new Grid
        {
            HeightRequest = heroHeight,
            Children =
            {
                _mapImage,
                hud
            }
        };

        var journeyPanel = new Border
        {
            Margin = new Thickness(10, -18, 10, 14),
            Padding = new Thickness(14, 16),
            BackgroundColor = Color.FromArgb("#F8FBFF"),
            Stroke = Color.FromArgb("#F59E0B"),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 },
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Opacity = .18f,
                Radius = 10,
                Offset = new Point(0, 4)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto)
                        },
                        Children = { _routeTitle, _routeProgressText }
                    },
                    _journeyProgress,
                    new ScrollView
                    {
                        Orientation = ScrollOrientation.Horizontal,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                        Content = _chapterTabs
                    },
                    new ScrollView
                    {
                        Orientation = ScrollOrientation.Horizontal,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                        Content = _routeStops
                    }
                }
            }
        };
        Grid.SetColumn(_routeProgressText, 1);

        _scroll = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children = { hero, journeyPanel, _missionCard }
            }
        };
        Content = _scroll;
        ShowStage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_testStage.HasValue)
        {
            Dispatcher.Dispatch(async () => await Navigation.PushAsync(new ParisTreasurePage(_testStage.Value)));
            return;
        }
        var savedStage = CurrentSavedStage();
        if (savedStage != _stageIndex)
        {
            _stageIndex = savedStage;
            ShowStage();
        }
    }

    private static int CurrentSavedStage()
    {
        var stage = AdventureSave.Get("adventure_stage", 0);
        // Older APKs routed completed home missions to a legacy ticket screen.
        // Restore the full station chapter without erasing any saved answers.
        if (stage == 1 &&
            AdventureSave.Get("home_pack_zayd", false) &&
            AdventureSave.Get("home_pack_razan", false) &&
            AdventureSave.Get("home_documents", false))
        {
            stage = 2;
            AdventureSave.Set("adventure_stage", stage);
        }
        return Math.Clamp(stage, 0, StageCount - 1);
    }

    private void RenderMap()
    {
        _chapterTabs.Clear();
        _routeStops.Clear();

        var parisCompleted = Math.Clamp(
            _stageIndex - ParisTreasureCatalog.FirstStage,
            0,
            ParisTreasureCatalog.Count);
        _journeyProgress.Progress = (double)Math.Min(_stageIndex, StageCount) / StageCount;
        _routeProgressText.Text = _stageIndex < ParisTreasureCatalog.FirstStage
            ? $"{_stageIndex}/4"
            : $"{parisCompleted}/30";

        if (_stageIndex < ParisTreasureCatalog.FirstStage)
        {
            _routeTitle.Text = "🧭 De reis naar Parijs";
            _chapterTabs.Add(MakeChapterChip("PROLOOG", true, false));
            var prologue = new[]
            {
                ("Thuis", "mission_pack_zayd.jpg"),
                ("Op weg", "adventure_map.jpg"),
                ("Station", "station_concourse.jpg"),
                ("Trein", "scene_train_interior.jpg"),
                ("Parijs", "paris_letter_scene.jpg")
            };

            for (var i = 0; i < prologue.Length; i++)
            {
                var stageNumber = i;
                var completed = i < _stageIndex;
                var current = i == _stageIndex;
                var unlocked = i <= _stageIndex;
                if (i > 0)
                    _routeStops.Add(MakeRouteConnector(i <= _stageIndex));
                _routeStops.Add(MakeRouteStop(
                    prologue[i].Item1,
                    prologue[i].Item2,
                    completed,
                    current,
                    unlocked,
                    async () =>
                    {
                        if (!unlocked) return;
                        if (stageNumber == 0)
                            await Navigation.PushAsync(new AdventureMissionHubPage());
                        else if (stageNumber == 2)
                            await Navigation.PushAsync(new StationMissionHubPage());
                        else if (stageNumber == 3)
                            await Navigation.PushAsync(new TrainMissionHubPage());
                        else if (stageNumber < ParisTreasureCatalog.FirstStage)
                            await Navigation.PushAsync(new AdventureScenePage(stageNumber));
                        else
                            await Navigation.PushAsync(new ParisChapterPage());
                    }));
            }
            return;
        }

        var chapterCount = 6;
        var currentChapter = Math.Clamp(parisCompleted / 5, 0, chapterCount - 1);
        if (_selectedChapter < 0 || _selectedChapter >= chapterCount || _selectedChapter > currentChapter)
            _selectedChapter = currentChapter;
        if (parisCompleted > 0 && parisCompleted % 5 == 0)
            _selectedChapter = Math.Min(parisCompleted / 5, chapterCount - 1);

        var chapterTitles = new[]
        {
            "Aankomst", "Monumenten", "Stad", "Kunst", "Buurten", "Finale"
        };
        var chapterIcons = new[] { "🗺️", "🏛️", "🔭", "🎨", "🥐", "⭐" };
        _routeTitle.Text = $"{chapterIcons[_selectedChapter]} {chapterTitles[_selectedChapter]}";

        for (var chapter = 0; chapter < chapterCount; chapter++)
        {
            var chapterIndex = chapter;
            var completedChapter = parisCompleted >= (chapter + 1) * 5;
            var unlockedChapter = chapter <= currentChapter;
            var chip = MakeChapterChip(
                $"{chapterIcons[chapter]} {chapter + 1}",
                chapter == _selectedChapter,
                completedChapter,
                unlockedChapter);
            if (unlockedChapter)
            {
                chip.Clicked += (_, _) =>
                {
                    _selectedChapter = chapterIndex;
                    RenderMap();
                };
            }
            _chapterTabs.Add(chip);
        }

        var firstLevel = _selectedChapter * 5;
        for (var offset = 0; offset < 5; offset++)
        {
            var questIndex = firstLevel + offset;
            var completed = questIndex < parisCompleted;
            var current = questIndex == parisCompleted;
            var unlocked = questIndex <= parisCompleted;
            var quest = ParisTreasureCatalog.Quests[questIndex];
            var stage = ParisTreasureCatalog.FirstStage + questIndex;
            if (offset > 0)
                _routeStops.Add(MakeRouteConnector(questIndex <= parisCompleted));
            _routeStops.Add(MakeRouteStop(
                ShortTitle(quest.Title),
                quest.Photo,
                completed,
                current,
                unlocked,
                async () =>
                {
                    if (unlocked)
                        await Navigation.PushAsync(new ParisTreasurePage(stage));
                }));
        }
    }

    private static Button MakeChapterChip(
        string text,
        bool selected,
        bool completed,
        bool enabled = true) => new()
        {
            Text = completed ? $"✓ {text}" : text,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            HeightRequest = 38,
            Padding = new Thickness(13, 0),
            CornerRadius = 18,
            TextColor = Colors.White,
            BackgroundColor = completed
                ? Color.FromArgb("#16A34A")
                : selected ? Color.FromArgb("#D97706") : Color.FromArgb("#64748B"),
            IsEnabled = enabled,
            Opacity = enabled ? 1 : .45
        };

    private static View MakeRouteStop(
        string title,
        string image,
        bool completed,
        bool current,
        bool unlocked,
        Func<Task> open)
    {
        var picture = new Image
        {
            Source = image,
            Aspect = Aspect.AspectFill,
            WidthRequest = current ? 60 : 54,
            HeightRequest = current ? 60 : 54,
            Opacity = unlocked ? 1 : .38
        };
        var pictureFrame = new Border
        {
            Content = picture,
            Padding = 0,
            WidthRequest = current ? 66 : 60,
            HeightRequest = current ? 66 : 60,
            StrokeThickness = current ? 4 : 3,
            Stroke = completed
                ? Color.FromArgb("#16A34A")
                : current ? Color.FromArgb("#F59E0B") : Color.FromArgb("#94A3B8"),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 33 }
        };
        var badge = new Label
        {
            Text = completed ? "✓" : current ? "▶" : "🔒",
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = completed
                ? Color.FromArgb("#16A34A")
                : current ? Color.FromArgb("#D97706") : Color.FromArgb("#64748B"),
            WidthRequest = 25,
            HeightRequest = 25,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End
        };
        var visual = new Grid { WidthRequest = 68, HeightRequest = 68 };
        visual.Add(pictureFrame);
        visual.Add(badge);
        var stop = new VerticalStackLayout
        {
            WidthRequest = 70,
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                visual,
                new Label
                {
                    Text = title,
                    FontSize = 11,
                    FontAttributes = current ? FontAttributes.Bold : FontAttributes.None,
                    TextColor = unlocked ? Color.FromArgb("#17324D") : Color.FromArgb("#94A3B8"),
                    HorizontalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaxLines = 2
                }
            }
        };
        if (unlocked)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await open();
            stop.GestureRecognizers.Add(tap);
        }
        return stop;
    }

    private static View MakeRouteConnector(bool travelled) => new Label
    {
        Text = "•••",
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb(travelled ? "#F59E0B" : "#CBD5E1"),
        WidthRequest = 25,
        HeightRequest = 68,
        HorizontalTextAlignment = TextAlignment.Center,
        VerticalTextAlignment = TextAlignment.Center
    };

    private static string ShortTitle(string title)
    {
        var clean = title.Replace("Le ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("La ", "", StringComparison.OrdinalIgnoreCase)
            .Replace("Les ", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
        return clean.Length <= 12 ? clean : $"{clean[..11]}…";
    }

    private void ShowStage()
    {
        _answered = false;
        _missionCard.IsVisible = false;
        _missionCard.Opacity = 0;
        var stage = Stages[Math.Min(_stageIndex, ParisTreasureCatalog.FirstStage)];
        _mapImage.Source = "paris_letter_scene.jpg";
        _stars.Text = $"⭐ {AdventureSave.Get("stars", 0)}";
        _step.Text = _stageIndex >= ParisTreasureCatalog.FirstStage
            ? $"PARIJS · {Math.Min(ParisTreasureCatalog.Count, _stageIndex - 3)} / {ParisTreasureCatalog.Count}"
            : $"MISSIE {_stageIndex + 1} / {StageCount}";
        _progress.Progress = (double)(_stageIndex + 1) / StageCount;
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
        var stars = AdventureSave.Get("stars", 0);
        stars = correct ? stars + 1 : Math.Max(0, stars - 1);
        AdventureSave.Set("stars", stars);
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

        if (_testStage.HasValue && AdventureSave.IsTestMode)
        {
            await DisplayAlert("Testmodus", "Vraag getest. Kies een andere scène in het testmenu.", "Verder");
            await Navigation.PopAsync();
            return;
        }

        var nextStage = _stageIndex + 1;

        AdventureSave.Set("adventure_stage", Math.Min(nextStage, Stages.Length));

        if (nextStage >= Stages.Length)
        {
            var replay = await DisplayAlert(
                "🏆 Parijs ontdekt!",
                $"Zayd en Razan hebben hun reis helemaal zelf voltooid met {stars} sterren.",
                "Opnieuw spelen",
                "Naar start");
            if (replay)
            {
                AdventureSave.Set("adventure_stage", 0);
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
