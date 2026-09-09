namespace ZaydRazanChallenge;

public sealed class PuzzleGalleryPage : ContentPage
{
    private readonly VerticalStackLayout _cards = new() { Spacing = 14 };
    private readonly Label _progress = new()
    {
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };

    public PuzzleGalleryPage()
    {
        Title = "Puzzels van Parijs";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 18,
                Spacing = 14,
                Children =
                {
                    new Label
                    {
                        Text = "🧩 Puzzels van Parijs",
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Kies een monument, verplaats de 9 blokken en verzamel sterren!",
                        FontSize = 16,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    _progress,
                    _cards
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuildGallery();
    }

    private void BuildGallery()
    {
        _cards.Clear();
        var completed = PuzzleCatalog.Items.Count(p =>
            Preferences.Default.Get($"puzzle_completed_{p.Key}", false));
        _progress.Text = $"⭐ {Preferences.Default.Get("stars", 0)}   •   {completed}/{PuzzleCatalog.Items.Count} voltooid";

        foreach (var puzzle in PuzzleCatalog.Items)
        {
            var isCompleted = Preferences.Default.Get(
                $"puzzle_completed_{puzzle.Key}", false);
            var button = new Button
            {
                Text = isCompleted ? "↻ Opnieuw spelen" : "▶ Start puzzel",
                BackgroundColor = isCompleted
                    ? Color.FromArgb("#16A34A")
                    : Color.FromArgb("#0891B2")
            };
            button.Clicked += async (_, _) =>
                await Navigation.PushAsync(new PuzzlePage(puzzle));

            var details = new VerticalStackLayout
            {
                Spacing = 7,
                Children =
                {
                    new Label
                    {
                        Text = $"{puzzle.Emoji} {puzzle.Title}",
                        FontSize = 21,
                        FontAttributes = FontAttributes.Bold
                    },
                    new Label
                    {
                        Text = isCompleted ? "✅ Voltooid" : "🔒 Nog te bouwen",
                        FontSize = 14,
                        TextColor = isCompleted
                            ? Color.FromArgb("#15803D")
                            : Color.FromArgb("#64748B")
                    },
                    button
                }
            };

            var layout = new Grid
            {
                ColumnSpacing = 14,
                ColumnDefinitions =
                {
                    new ColumnDefinition(new GridLength(115)),
                    new ColumnDefinition(GridLength.Star)
                }
            };
            var thumbnail = new Image
            {
                Source = $"{puzzle.Key}_puzzle.jpg",
                HeightRequest = 125,
                WidthRequest = 115,
                Aspect = Aspect.AspectFill
            };
            layout.Add(thumbnail, 0, 0);
            layout.Add(details, 1, 0);

            _cards.Add(new Border
            {
                BackgroundColor = Colors.White,
                Stroke = isCompleted
                    ? Color.FromArgb("#22C55E")
                    : Color.FromArgb("#BAE6FD"),
                StrokeThickness = 2,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = 20
                },
                Padding = 12,
                Content = layout
            });
        }
    }
}
