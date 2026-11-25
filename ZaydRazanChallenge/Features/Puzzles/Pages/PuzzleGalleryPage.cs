namespace ZaydRazanChallenge.Features.Puzzles.Pages;

public sealed class PuzzleGalleryPage : ContentPage
{
    private readonly IGameProgressStore _gameProgress;
    private readonly IPuzzleCatalog _puzzles;
    private readonly IGamePageFactory _pages;
    private readonly Label _progress = new()
    {
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };

    public PuzzleGalleryPage(IPuzzleCatalog puzzles, IGamePageFactory pages, IGameProgressStore progress)
    {
        _gameProgress = progress;
        _puzzles = puzzles;
        _pages = pages;
        Title = "Frankrijk-puzzels";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        var startButton = new Button
        {
            Text = "🎲 Start een verrassingspuzzel",
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
                        Text = "Alle ontdekkingen worden puzzels",
                        FontSize = 29,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Alle 30 afbeeldingen uit het avontuur zitten nu in de verrassingspuzzel. Het raster en het maximale aantal zetten worden willekeurig gekozen.",
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
                                    Text = "🎯 Spelregels",
                                    FontSize = 20,
                                    FontAttributes = FontAttributes.Bold
                                },
                                new Label { Text = "• Willekeurige limiet: 7 tot 20 zetten" },
                                new Label { Text = "• Willekeurig raster: 3×3 tot 6×6" },
                                new Label { Text = "• Tot 50% van de limiet: ⭐⭐⭐" },
                                new Label { Text = "• Tot 75% van de limiet: ⭐⭐" },
                                new Label { Text = "• Binnen de limiet: ⭐" }
                            }
                        }
                    },
                    _progress,
                    startButton,
                    new Label
                    {
                        Text = "30 avontuurlijke ontdekkingen uit Parijs · Versailles als bonus",
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
        _progress.Text =
            $"⭐ {_gameProgress.Get("stars", 0)}   •   {discovered}/{_puzzles.Items.Count} ontdekkingen";
    }
}
