namespace ZaydRazanChallenge;

public enum GameMode { Matching, Listening, Monuments }

public sealed class GamePage : ContentPage
{
    private readonly GameMode _mode;
    private readonly Label _question = new() { FontSize = 27, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _feedback = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly VerticalStackLayout _answers = new() { Spacing = 12 };
    private readonly Button _sound = new() { Text = "🔊 Écouter le mot", BackgroundColor = Color.FromArgb("#7C3AED"), IsVisible = false };
    private int _index;
    private int _score;

    private static readonly (string Fr, string Nl)[] Words =
    [
        ("bonjour", "goedendag"), ("un pantalon bleu", "een blauwe broek"),
        ("une valise", "een koffer"), ("un train", "een trein"),
        ("une gare", "een station"), ("merci", "dank je")
    ];

    private static readonly (string Question, string Answer, string[] Choices)[] Monuments =
    [
        ("Quel monument ressemble à une grande tour de fer ?", "La tour Eiffel", ["Le Louvre", "La tour Eiffel", "Notre-Dame"]),
        ("Waar vind je de Mona Lisa?", "Le Louvre", ["Le Louvre", "Sacré-Cœur", "Arc de Triomphe"]),
        ("Quel monument est une célèbre cathédrale ?", "Notre-Dame", ["Notre-Dame", "La tour Eiffel", "Le Louvre"]),
        ("Welk monument staat op de Champs-Élysées?", "Arc de Triomphe", ["Sacré-Cœur", "Notre-Dame", "Arc de Triomphe"]),
        ("Quel monument blanc domine Montmartre ?", "Sacré-Cœur", ["Le Louvre", "Sacré-Cœur", "Arc de Triomphe"])
    ];

    public GamePage(GameMode mode)
    {
        _mode = mode;
        Title = mode switch { GameMode.Matching => "Woorden verbinden", GameMode.Listening => "Luisteren", _ => "Monumentenquiz" };
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);
        _sound.Clicked += async (_, _) => await TextToSpeech.Default.SpeakAsync(
            Words[_index % Words.Length].Fr,
            new SpeechOptions { Locale = await FindFrenchLocale() });
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 22, Spacing = 22,
                Children =
                {
                    new Label { Text = "Op reis met Zayd en Razan", FontSize = 17, TextColor = Color.FromArgb("#3B82F6"), HorizontalTextAlignment = TextAlignment.Center },
                    _question, _sound, _answers, _feedback
                }
            }
        };
        ShowQuestion();
    }

    private void ShowQuestion()
    {
        _feedback.Text = "";
        _answers.Clear();
        if (_mode == GameMode.Monuments)
        {
            var item = Monuments[_index % Monuments.Length];
            _question.Text = item.Question;
            AddChoices(item.Choices, item.Answer);
            return;
        }
        var word = Words[_index % Words.Length];
        _sound.IsVisible = _mode == GameMode.Listening;
        _question.Text = _mode == GameMode.Matching
            ? $"Que signifie « {word.Fr} » ?"
            : "Luister en kies het juiste Nederlandse woord";
        var choices = Words.Select(x => x.Nl).Where(x => x != word.Nl)
            .OrderBy(_ => Random.Shared.Next()).Take(2).Append(word.Nl)
            .OrderBy(_ => Random.Shared.Next()).ToArray();
        AddChoices(choices, word.Nl);
    }

    private void AddChoices(IEnumerable<string> choices, string answer)
    {
        foreach (var choice in choices)
        {
            var button = new Button { Text = choice };
            button.Clicked += (_, _) => Answer(choice == answer, button);
            _answers.Add(button);
        }
    }

    private async void Answer(bool correct, Button selected)
    {
        foreach (var view in _answers.Children)
            if (view is Button button) button.IsEnabled = false;
        selected.BackgroundColor = correct ? Color.FromArgb("#16A34A") : Color.FromArgb("#DC2626");
        await (correct ? GameFeedback.SuccessAsync() : GameFeedback.FailureAsync());
        _feedback.Text = correct ? "Bravo! Goed gedaan! ⭐" : "Presque ! Bijna goed.";
        if (correct)
        {
            _score++;
            Preferences.Default.Set("stars", Preferences.Default.Get("stars", 0) + 1);
        }
        await Task.Delay(900);
        _index++;
        if (_index >= (_mode == GameMode.Monuments ? Monuments.Length : Words.Length))
        {
            await DisplayAlert("Challenge terminé", $"Tu as gagné {_score} étoile(s) !", "Terug");
            await Navigation.PopToRootAsync();
            return;
        }
        ShowQuestion();
    }

    private static async Task<Locale?> FindFrenchLocale() =>
        (await TextToSpeech.Default.GetLocalesAsync())
        .FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
}
