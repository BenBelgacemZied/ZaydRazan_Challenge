namespace ZaydRazanChallenge.Features.Shared;

public enum GameMode { Matching, Listening, Monuments }

public sealed class GamePage : ContentPage
{
    private readonly IDestinationContent _destination;
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private readonly GameMode _mode;
    private readonly Label _question = new() { FontSize = 27, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _feedback = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly VerticalStackLayout _answers = new() { Spacing = 12 };
    private readonly Button _sound = new() { Text = "🔊 Luister naar het woord", BackgroundColor = Color.FromArgb("#7C3AED"), IsVisible = false };
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";
    private int _index;
    private int _score;

    private static readonly (string Fr, string Nl, string En)[] Words =
    [
        ("bonjour", "goedendag", "hello"), ("un pantalon bleu", "een blauwe broek", "blue trousers"),
        ("une valise", "een koffer", "a suitcase"), ("un train", "een trein", "a train"),
        ("une gare", "een station", "a station"), ("merci", "dank je", "thank you")
    ];

    private static readonly (string Question, string Answer, string[] Choices)[] Monuments =
    [
        ("Welk monument lijkt op een grote ijzeren toren?", "La tour Eiffel", ["Le Louvre", "La tour Eiffel", "Notre-Dame"]),
        ("Waar vind je de Mona Lisa?", "Le Louvre", ["Le Louvre", "Sacré-Cœur", "Arc de Triomphe"]),
        ("Welk monument is een beroemde kathedraal?", "Notre-Dame", ["Notre-Dame", "La tour Eiffel", "Le Louvre"]),
        ("Welk monument staat op de Champs-Élysées?", "Arc de Triomphe", ["Sacré-Cœur", "Notre-Dame", "Arc de Triomphe"]),
        ("Welk wit monument staat boven op Montmartre?", "Sacré-Cœur", ["Le Louvre", "Sacré-Cœur", "Arc de Triomphe"])
    ];

    private static readonly (string Question, string Answer, string[] Choices)[] EnglishMonuments =
    [
        ("Which landmark looks like a tall iron tower?", "La tour Eiffel", ["Le Louvre", "La tour Eiffel", "Notre-Dame"]),
        ("Where can you see the Mona Lisa?", "Le Louvre", ["Le Louvre", "Sacré-Cœur", "Arc de Triomphe"]),
        ("Which landmark is a famous cathedral?", "Notre-Dame", ["Notre-Dame", "La tour Eiffel", "Le Louvre"]),
        ("Which landmark stands on the Champs-Élysées?", "Arc de Triomphe", ["Sacré-Cœur", "Notre-Dame", "Arc de Triomphe"]),
        ("Which white landmark stands on top of Montmartre?", "Sacré-Cœur", ["Le Louvre", "Sacré-Cœur", "Arc de Triomphe"])
    ];

    public GamePage(GameMode mode, IGameProgressStore progress, ISpeechService speech, IDestinationContent destination)
    {
        _destination = destination;
        _speechService = speech;
        _gameProgress = progress;
        _mode = mode;
        Title = IsEnglish
            ? mode switch { GameMode.Matching => "Match the words", GameMode.Listening => "Listening", _ => "Monument quiz" }
            : mode switch { GameMode.Matching => "Woorden verbinden", GameMode.Listening => "Luisteren", _ => "Monumentenquiz" };
        _sound.Text = IsEnglish ? "🔊 Listen to the word" : "🔊 Luister naar het woord";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);
        _sound.Clicked += async (_, _) => await _speechService.SpeakAsync(Words[_index % Words.Length].Fr, _destination.LanguagePair.TargetLanguage.Value);
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
            var questions = IsEnglish ? EnglishMonuments : Monuments;
            var item = questions[_index % questions.Length];
            _question.Text = item.Question;
            AddChoices(item.Choices, item.Answer);
            return;
        }
        var word = Words[_index % Words.Length];
        _sound.IsVisible = _mode == GameMode.Listening;
        var sourceWord = IsEnglish ? word.En : word.Nl;
        _question.Text = _mode == GameMode.Matching
            ? (IsEnglish ? $"What does « {word.Fr} » mean?" : $"Wat betekent « {word.Fr} »?")
            : (IsEnglish ? "Listen and choose the English meaning" : "Luister en kies het juiste Nederlandse woord");
        var choices = Words.Select(x => IsEnglish ? x.En : x.Nl).Where(x => x != sourceWord)
            .OrderBy(_ => Random.Shared.Next()).Take(2).Append(sourceWord)
            .OrderBy(_ => Random.Shared.Next()).ToArray();
        AddChoices(choices, sourceWord);
    }

    private void AddChoices(IEnumerable<string> choices, string answer)
    {
        foreach (var choice in choices)
        {
            var button = new Button { Text = choice, LineBreakMode = LineBreakMode.WordWrap, FontSize = 18, FontAttributes = FontAttributes.Bold, MinimumHeightRequest = 58 };
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
        _feedback.Text = correct
            ? (IsEnglish ? "Well done! ⭐" : "Goed gedaan! ⭐")
            : (IsEnglish ? "Almost! Try the next word." : "Bijna goed. Probeer het volgende woord.");
        if (correct)
        {
            _score++;
            _gameProgress.Set("stars", _gameProgress.Get("stars", 0) + 1);
        }
        await Task.Delay(900);
        _index++;
        if (_index >= (_mode == GameMode.Monuments ? Monuments.Length : Words.Length))
        {
            await DisplayAlert(
                IsEnglish ? "Challenge complete" : "Uitdaging voltooid",
                IsEnglish ? $"You earned {_score} star(s)!" : $"Je hebt {_score} ster(ren) verdiend!",
                IsEnglish ? "Back" : "Terug");
            await Navigation.PopToRootAsync();
            return;
        }
        ShowQuestion();
    }

}
