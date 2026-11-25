namespace ZaydRazanChallenge.Features.WordGames.Pages;

public sealed class WordGamesHubPage : ContentPage
{
    private readonly IGamePageFactory _pages;
    private readonly IDestinationContent _destination;
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";

    public WordGamesHubPage(IVocabularyCatalog vocabulary, IGamePageFactory pages, IDestinationContent destination)
    {
        _pages = pages;
        _destination = destination;
        Title = IsEnglish ? "Word workshop" : "Woordenatelier";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        var games = new VerticalStackLayout { Spacing = 12 };
        AddGame(games, "🔗", IsEnglish ? "Match the words" : "Woorden verbinden", IsEnglish ? "Match five English words with their French translations." : "Verbind vijf Nederlandse woorden met hun Franse vertaling.", "#2563EB", WordGameMode.Connect);
        AddGame(games, "🎯", IsEnglish ? "Three choices" : "Drie keuzes", IsEnglish ? "Read the English word and choose the correct French translation." : "Lees het Nederlandse woord en kies de juiste Franse vertaling.", "#E11D48", WordGameMode.ThreeChoices);
        AddGame(games, "🔤", IsEnglish ? "Build the word" : "Bouw het woord", IsEnglish ? "Put the mixed-up letters in the right order." : "Zet de gemengde letters in de juiste volgorde.", "#059669", WordGameMode.BuildWord);
        AddGame(games, "🔊", IsEnglish ? "Listening challenge" : "Luisteruitdaging", IsEnglish ? "Listen to the French word and choose its English meaning." : "Luister naar het Franse woord en kies de Nederlandse betekenis.", "#7C3AED", WordGameMode.Listening);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 18,
                Spacing = 16,
                Children =
                {
                    new Label { Text = IsEnglish ? "🎒 Word book" : "🎒 Le carnet des mots", FontSize = 28, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#17324D") },
                    new Label { Text = IsEnglish ? $"Practise {vocabulary.Items.Count} words from the journey with four different games." : $"Oefen {vocabulary.Items.Count} woorden uit de hele reis met vier verschillende spellen.", FontSize = 17, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#475569") },
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
        button.Clicked += async (_, _) => await Navigation.PushAsync(_pages.CreateWordGamePage(mode));
        layout.Add(button);
    }
}
