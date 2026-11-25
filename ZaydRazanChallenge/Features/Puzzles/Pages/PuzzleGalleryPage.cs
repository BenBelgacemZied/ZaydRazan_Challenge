namespace ZaydRazanChallenge.Features.Puzzles.Pages;

public sealed class PuzzleGalleryPage : ContentPage
{
    private readonly IGameProgressStore _gameProgress;
    private readonly IDestinationContent _destination;
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";
    private readonly IPuzzleCatalog _puzzles;
    private readonly IGamePageFactory _pages;
    private readonly Label _progress = new()
    {
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };

    public PuzzleGalleryPage(IPuzzleCatalog puzzles, IGamePageFactory pages, IGameProgressStore progress, IDestinationContent destination)
    {
        _gameProgress = progress;
        _destination = destination;
        _puzzles = puzzles;
        _pages = pages;
        Title = IsEnglish ? "Paris puzzles" : "Frankrijk-puzzels";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        var startButton = new Button
        {
            Text = IsEnglish ? "🎲 Start a surprise puzzle" : "🎲 Start een verrassingspuzzel",
            FontSize = 19,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#E11D48"),
            TextColor = Colors.White,
            HeightRequest = 62
        };
        startButton.Clicked += async (_, _) =>
            await Navigation.PushAsync(_pages.CreatePuzzlePage(_puzzles.GetRandom()));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 22,
                Spacing = 18,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label
                    {
                        Text = "🇫🇷🧩",
                        FontSize = 76,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = IsEnglish ? "Every discovery becomes a puzzle" : "Alle ontdekkingen worden puzzels",
                        FontSize = 29,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = IsEnglish ? "All 30 pictures from the adventure are included. The grid and move limit are chosen at random." : "Alle 30 afbeeldingen uit het avontuur zitten nu in de verrassingspuzzel. Het raster en het maximale aantal zetten worden willekeurig gekozen.",
                        FontSize = 17,
                        LineHeight = 1.25,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Border
                    {
                        BackgroundColor = Color.FromArgb("#DBEAFE"),
                        StrokeThickness = 0,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
                        Padding = 16,
                        Content = new VerticalStackLayout
                        {
                            Spacing = 8,
                            Children =
                            {
                                new Label
                                {
                                    Text = IsEnglish ? "🎯 Rules" : "🎯 Spelregels",
                                    FontSize = 20,
                                    FontAttributes = FontAttributes.Bold
                                },
                                new Label { Text = IsEnglish ? "• Random limit: 7 to 20 moves" : "• Willekeurige limiet: 7 tot 20 zetten" },
                                new Label { Text = IsEnglish ? "• Random grid: 3×3 to 6×6" : "• Willekeurig raster: 3×3 tot 6×6" },
                                new Label { Text = IsEnglish ? "• Within 50% of the limit: ⭐⭐⭐" : "• Tot 50% van de limiet: ⭐⭐⭐" },
                                new Label { Text = IsEnglish ? "• Within 75% of the limit: ⭐⭐" : "• Tot 75% van de limiet: ⭐⭐" },
                                new Label { Text = IsEnglish ? "• Within the limit: ⭐" : "• Binnen de limiet: ⭐" }
                            }
                        }
                    },
                    _progress,
                    startButton,
                    new Label
                    {
                        Text = IsEnglish ? "30 discoveries in Paris · plus Versailles" : "30 avontuurlijke ontdekkingen uit Parijs · Versailles als bonus",
                        FontSize = 14,
                        TextColor = Color.FromArgb("#64748B"),
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var discovered = _puzzles.Items.Count(p =>
            _gameProgress.Get($"puzzle_completed_{p.Key}", false));
        _progress.Text = IsEnglish
            ? $"⭐ {_gameProgress.Get("stars", 0)}   •   {discovered}/{_puzzles.Items.Count} discoveries"
            : $"⭐ {_gameProgress.Get("stars", 0)}   •   {discovered}/{_puzzles.Items.Count} ontdekkingen";
    }
}
