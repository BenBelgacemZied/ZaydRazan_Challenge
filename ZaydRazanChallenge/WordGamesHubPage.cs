namespace ZaydRazanChallenge;

public sealed class WordGamesHubPage : ContentPage
{
    public WordGamesHubPage()
    {
        Title = "Woordenatelier";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        var games = new VerticalStackLayout { Spacing = 12 };
        AddGame(games, "🔗", "Woorden verbinden", "Verbind vijf Nederlandse woorden met hun Franse vertaling.", "#2563EB", WordGameMode.Connect);
        AddGame(games, "🎯", "Drie keuzes", "Lees het Nederlandse woord en kies de juiste Franse vertaling.", "#E11D48", WordGameMode.ThreeChoices);
        AddGame(games, "🔤", "Bouw het woord", "Zet de gemengde letters in de juiste volgorde.", "#059669", WordGameMode.BuildWord);
        AddGame(games, "🔊", "Luisteruitdaging", "Luister naar het Franse woord en kies de Nederlandse betekenis.", "#7C3AED", WordGameMode.Listening);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 18,
                Spacing = 16,
                Children =
                {
                    new Label { Text = "🎒 Le carnet des mots", FontSize = 28, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#17324D") },
                    new Label { Text = $"Oefen {VocabularyCatalog.Items.Count} woorden uit de hele reis met vier verschillende spellen.", FontSize = 17, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#475569") },
                    games
                }
            }
        };
    }

    private void AddGame(Layout layout, string icon, string title, string subtitle, string color, WordGameMode mode)
    {
        var button = new Button
        {
            Text = $"{icon}  {title}\n{subtitle}",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb(color),
            TextColor = Colors.White,
            CornerRadius = 18,
            HeightRequest = 88,
            Padding = new Thickness(14, 8)
        };
        button.Clicked += async (_, _) => await Navigation.PushAsync(new WordGamePage(mode));
        layout.Add(button);
    }
}
