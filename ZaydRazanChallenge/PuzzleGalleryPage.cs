namespace ZaydRazanChallenge;

public sealed class PuzzleGalleryPage : ContentPage
{
    private readonly Label _progress = new()
    {
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };

    public PuzzleGalleryPage()
    {
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
            await Navigation.PushAsync(new PuzzlePage(PuzzleCatalog.GetRandom()));

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
                        Text = "De verrassingspuzzel van Frankrijk",
                        FontSize = 29,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "De afbeelding, het raster en het maximale aantal zetten worden willekeurig gekozen. Kijk goed, denk na en ontdek daarna een leuk weetje!",
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
                        Text = "La Joconde · le croissant · la baguette · Versailles · Parijse monumenten en nog veel meer",
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
        var discovered = PuzzleCatalog.Items.Count(p =>
            Preferences.Default.Get($"puzzle_completed_{p.Key}", false));
        _progress.Text =
            $"⭐ {Preferences.Default.Get("stars", 0)}   •   {discovered}/{PuzzleCatalog.Items.Count} ontdekkingen";
    }
}
