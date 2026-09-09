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
        Title = "Défis de la France";
        BackgroundColor = Color.FromArgb("#FFF8E7");

        var startButton = new Button
        {
            Text = "🎲 Lancer un défi surprise",
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
                        Text = "Le défi surprise de la France",
                        FontSize = 29,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "L’image, la grille et la limite de déplacements sont choisies au hasard. Observe, réfléchis et découvre ensuite un fait culturel!",
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
                                    Text = "🎯 Règles du challenge",
                                    FontSize = 20,
                                    FontAttributes = FontAttributes.Bold
                                },
                                new Label { Text = "• Limite aléatoire: 7 à 20 déplacements" },
                                new Label { Text = "• Grille aléatoire: 3×3 à 6×6" },
                                new Label { Text = "• 50% de la limite: ⭐⭐⭐" },
                                new Label { Text = "• 75% de la limite: ⭐⭐" },
                                new Label { Text = "• Jusqu’à la limite: ⭐" }
                            }
                        }
                    },
                    _progress,
                    startButton,
                    new Label
                    {
                        Text = "La Joconde · croissant · baguette · Versailles · monuments de Paris et bien plus",
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
            $"⭐ {Preferences.Default.Get("stars", 0)}   •   {discovered}/{PuzzleCatalog.Items.Count} découvertes";
    }
}
