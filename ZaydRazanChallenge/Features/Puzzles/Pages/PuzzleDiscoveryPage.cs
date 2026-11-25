namespace ZaydRazanChallenge.Features.Puzzles.Pages;

public sealed class PuzzleDiscoveryPage : ContentPage
{
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private readonly PuzzleDefinition _puzzle;
    private readonly IPuzzleCatalog _puzzles;
    private readonly IGamePageFactory _pages;
    private readonly IDestinationContent _destination;

    public PuzzleDiscoveryPage(PuzzleDefinition puzzle, int moves, int maxMoves, int reward, IPuzzleCatalog puzzles, IGamePageFactory pages, IGameProgressStore progress, ISpeechService speech, IDestinationContent destination)
    {
        _speechService = speech;
        _gameProgress = progress;
        _puzzle = puzzle;
        _puzzles = puzzles;
        _pages = pages;
        _destination = destination;
        Title = "Ontdekking";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        var listen = new Button
        {
            Text = "🔊 Luister naar de uitleg",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#7C3AED"),
            TextColor = Colors.White,
            HeightRequest = 58
        };
        listen.Clicked += async (_, _) => await SpeakDiscovery();

        var next = new Button
        {
            Text = "🎲 Nieuwe verrassingspuzzel",
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#E11D48"),
            TextColor = Colors.White
        };
        next.Clicked += async (_, _) =>
            await Navigation.PushAsync(_pages.CreatePuzzlePage(_puzzles.GetRandom(_puzzle.Key)));

        var home = new Button
        {
            Text = "⌂ Terug naar start",
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
                        Text = "🏆 Puzzel opgelost!",
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.FromArgb("#15803D")
                    },
                    new Image
                    {
                        Source = puzzle.ImageSource,
                        HeightRequest = 250,
                        Aspect = Aspect.AspectFit
                    },
                    new Label
                    {
                        Text = $"{puzzle.Emoji} {puzzle.TargetName}",
                        FontSize = 27,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = $"{moves}/{maxMoves} zetten · {new string('⭐', reward)}",
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
                                    Text = "🎓 Wist je dat?",
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
        await _speechService.SpeakAsync(_puzzle.TargetName, _destination.LanguagePair.TargetLanguage.Value);
        await _speechService.SpeakAsync(_puzzle.Description, _destination.LanguagePair.SourceLanguage.Value);
    }
}
