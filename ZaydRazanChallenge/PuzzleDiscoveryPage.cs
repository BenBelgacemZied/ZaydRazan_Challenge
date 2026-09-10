namespace ZaydRazanChallenge;

public sealed class PuzzleDiscoveryPage : ContentPage
{
    private readonly PuzzleDefinition _puzzle;

    public PuzzleDiscoveryPage(PuzzleDefinition puzzle, int moves, int maxMoves, int reward)
    {
        _puzzle = puzzle;
        Title = "Découverte";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        var listen = new Button
        {
            Text = "🔊 Écouter l’information",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#7C3AED"),
            TextColor = Colors.White,
            HeightRequest = 58
        };
        listen.Clicked += async (_, _) => await SpeakDiscovery();

        var next = new Button
        {
            Text = "🎲 Nouveau défi surprise",
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#E11D48"),
            TextColor = Colors.White
        };
        next.Clicked += async (_, _) =>
            await Navigation.PushAsync(new PuzzlePage(PuzzleCatalog.GetRandom(_puzzle.Key)));

        var home = new Button
        {
            Text = "⌂ Retour à l’accueil",
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#183153"),
            TextColor = Colors.White
        };
        home.Clicked += async (_, _) => await Navigation.PopToRootAsync();

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 16,
                Children =
                {
                    new Label
                    {
                        Text = "🏆 Puzzle réussi !",
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.FromArgb("#15803D")
                    },
                    new Image
                    {
                        Source = $"{puzzle.Key}_puzzle.jpg",
                        HeightRequest = 250,
                        Aspect = Aspect.AspectFit
                    },
                    new Label
                    {
                        Text = $"{puzzle.Emoji} {puzzle.FrenchName}",
                        FontSize = 27,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = $"{moves}/{maxMoves} déplacements · {new string('⭐', reward)}",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.FromArgb("#D97706")
                    },
                    new Border
                    {
                        BackgroundColor = Color.FromArgb("#DBEAFE"),
                        Stroke = Color.FromArgb("#2563EB"),
                        StrokeThickness = 2,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                        Padding = 18,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 10,
                            Children =
                            {
                                new Label
                                {
                                    Text = "🎓 Le savais-tu ?",
                                    FontSize = 22,
                                    FontAttributes = FontAttributes.Bold,
                                    TextColor = Color.FromArgb("#1D4ED8")
                                },
                                new Label
                                {
                                    Text = puzzle.Description,
                                    FontSize = 18,
                                    LineHeight = 1.3,
                                    TextColor = Color.FromArgb("#1E293B")
                                }
                            }
                        }
                    },
                    listen,
                    next,
                    home
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(350);
        await SpeakDiscovery();
    }

    private async Task SpeakDiscovery()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        var french = locales.FirstOrDefault(x =>
            x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
        var dutch = locales.FirstOrDefault(x =>
            x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));

        await TextToSpeech.Default.SpeakAsync(_puzzle.FrenchName,
            new SpeechOptions { Locale = french });
        await TextToSpeech.Default.SpeakAsync(_puzzle.Description,
            new SpeechOptions { Locale = dutch ?? french });
    }
}
