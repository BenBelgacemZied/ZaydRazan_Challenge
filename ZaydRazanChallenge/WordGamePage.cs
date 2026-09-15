using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public enum WordGameMode { Connect, ThreeChoices, BuildWord, Listening }

public sealed class WordGamePage : ContentPage
{
    private readonly WordGameMode _mode;
    private readonly Label _progress = new() { FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#2563EB") };
    private readonly Label _question = new() { FontSize = 24, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#17324D") };
    private readonly Label _feedback = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly VerticalStackLayout _gameArea = new() { Spacing = 10 };
    private VocabularyWord[] _roundWords = [];
    private int _index;
    private int _score;
    private int _matched;
    private bool _busy;

    public WordGamePage(WordGameMode mode)
    {
        _mode = mode;
        Title = mode switch
        {
            WordGameMode.Connect => "Woorden verbinden",
            WordGameMode.ThreeChoices => "Drie keuzes",
            WordGameMode.BuildWord => "Bouw het woord",
            _ => "Luisteruitdaging"
        };
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Spacing = 14,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                        Children = { new Label { Text = "🇳🇱  →  🇫🇷", FontSize = 22, FontAttributes = FontAttributes.Bold }, _progress }
                    },
                    _question,
                    _gameArea,
                    _feedback
                }
            }
        };
        Grid.SetColumn(_progress, 1);
        StartGame();
    }

    private void StartGame()
    {
        _index = 0;
        _score = 0;
        _feedback.Text = "";
        if (_mode == WordGameMode.Connect)
            ShowConnectRound();
        else
        {
            var count = _mode == WordGameMode.BuildWord ? 6 : 10;
            _roundWords = VocabularyCatalog.RandomWords(count, _mode == WordGameMode.BuildWord ? IsSpellable : null);
            ShowSingleWordRound();
        }
    }

    private void ShowConnectRound()
    {
        _gameArea.Clear();
        _feedback.Text = "";
        _matched = 0;
        _roundWords = VocabularyCatalog.RandomWords(5);
        _progress.Text = "0/5";
        _question.Text = "Verbind de juiste woorden";

        var left = _roundWords.OrderBy(_ => Random.Shared.Next()).ToArray();
        var right = _roundWords.OrderBy(_ => Random.Shared.Next()).ToArray();
        var grid = new Grid { ColumnSpacing = 10, RowSpacing = 9, ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) } };
        for (var row = 0; row < 5; row++) grid.RowDefinitions.Add(new RowDefinition(new GridLength(58)));

        VocabularyWord? selectedWord = null;
        Button? selectedButton = null;
        var leftButtons = new List<Button>();
        for (var row = 0; row < 5; row++)
        {
            var dutchWord = left[row];
            var leftButton = MakeButton(dutchWord.Dutch, "#2563EB");
            leftButtons.Add(leftButton);
            leftButton.Clicked += (_, _) =>
            {
                if (_busy || !leftButton.IsEnabled) return;
                foreach (var button in leftButtons.Where(button => button.IsEnabled)) button.BackgroundColor = Color.FromArgb("#2563EB");
                selectedWord = dutchWord;
                selectedButton = leftButton;
                leftButton.BackgroundColor = Color.FromArgb("#F59E0B");
                _feedback.Text = "Kies nu het Franse woord.";
                _feedback.TextColor = Color.FromArgb("#475569");
            };
            grid.Add(leftButton, 0, row);

            var frenchWord = right[row];
            var rightButton = MakeButton(frenchWord.French, "#7C3AED");
            rightButton.Clicked += async (_, _) =>
            {
                if (_busy || !rightButton.IsEnabled || selectedWord is null || selectedButton is null) return;
                _busy = true;
                if (selectedWord.French == frenchWord.French)
                {
                    selectedButton.BackgroundColor = rightButton.BackgroundColor = Color.FromArgb("#16A34A");
                    selectedButton.IsEnabled = rightButton.IsEnabled = false;
                    _matched++;
                    _score++;
                    _progress.Text = $"{_matched}/5";
                    _feedback.Text = $"Goed zo! {selectedWord.Dutch} = {selectedWord.French}";
                    _feedback.TextColor = Color.FromArgb("#15803D");
                    await GameFeedback.SuccessAsync();
                    await SpeakFrenchAsync(selectedWord.French);
                    selectedWord = null;
                    selectedButton = null;
                    if (_matched == 5) await CompleteAsync();
                }
                else
                {
                    rightButton.BackgroundColor = Color.FromArgb("#DC2626");
                    _feedback.Text = "Die woorden horen niet bij elkaar. Probeer opnieuw.";
                    _feedback.TextColor = Color.FromArgb("#DC2626");
                    await GameFeedback.FailureAsync();
                    await Task.Delay(450);
                    rightButton.BackgroundColor = Color.FromArgb("#7C3AED");
                }
                _busy = false;
            };
            grid.Add(rightButton, 1, row);
        }
        _gameArea.Add(grid);
    }

    private void ShowSingleWordRound()
    {
        _busy = false;
        if (_index >= _roundWords.Length) { _ = CompleteAsync(); return; }
        _gameArea.Clear();
        _feedback.Text = "";
        _progress.Text = $"{_index + 1}/{_roundWords.Length}  ·  ⭐ {_score}";
        var word = _roundWords[_index];

        if (_mode == WordGameMode.BuildWord)
        {
            ShowBuildWord(word);
            return;
        }

        _question.Text = _mode == WordGameMode.Listening
            ? "Luister en kies de betekenis"
            : $"Hoe zeg je ‘{word.Dutch}’ in het Frans?";
        if (_mode == WordGameMode.Listening)
        {
            var listen = MakeButton("🔊  Luister naar het Franse woord", "#7C3AED");
            listen.Clicked += async (_, _) => await SpeakFrenchAsync(word.French);
            _gameArea.Add(listen);
            _ = SpeakFrenchAsync(word.French);
        }

        var answers = VocabularyCatalog.Items
            .Where(item => item.French != word.French)
            .OrderBy(_ => Random.Shared.Next()).Take(2).Append(word)
            .OrderBy(_ => Random.Shared.Next()).ToArray();
        foreach (var answer in answers)
        {
            var text = _mode == WordGameMode.Listening ? answer.Dutch : answer.French;
            var button = MakeButton(text, "#2563EB");
            button.Clicked += async (_, _) => await CheckChoiceAsync(answer.French == word.French, button, word);
            _gameArea.Add(button);
        }
    }

    private void ShowBuildWord(VocabularyWord word)
    {
        var target = Normalize(word.French);
        var current = "";
        _question.Text = $"Bouw het Franse woord voor ‘{word.Dutch}’";
        var answer = new Label { Text = "_ _ _", FontSize = 27, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#7C3AED") };
        var letters = new FlexLayout { Wrap = FlexWrap.Wrap, JustifyContent = FlexJustify.Center, AlignItems = FlexAlignItems.Center };
        var letterButtons = new List<Button>();

        void ResetLetters()
        {
            current = "";
            answer.Text = "_ _ _";
            foreach (var tile in letterButtons) tile.IsEnabled = true;
        }

        foreach (var letter in target.OrderBy(_ => Random.Shared.Next()))
        {
            var value = letter;
            var tile = new Button { Text = value.ToString().ToUpperInvariant(), FontSize = 20, FontAttributes = FontAttributes.Bold, WidthRequest = 51, HeightRequest = 51, CornerRadius = 14, Margin = 3, Padding = 0, BackgroundColor = Color.FromArgb("#DBEAFE"), TextColor = Color.FromArgb("#17324D") };
            letterButtons.Add(tile);
            tile.Clicked += async (_, _) =>
            {
                if (_busy || !tile.IsEnabled) return;
                tile.IsEnabled = false;
                current += value;
                answer.Text = string.Join(" ", current.ToUpperInvariant().ToCharArray());
                if (current.Length != target.Length) return;
                _busy = true;
                if (current == target)
                {
                    _score++;
                    _feedback.Text = $"Goed zo! {word.Dutch} = {word.French}";
                    _feedback.TextColor = Color.FromArgb("#15803D");
                    await GameFeedback.SuccessAsync();
                    await SpeakFrenchAsync(word.French);
                    await Task.Delay(650);
                    _index++;
                    ShowSingleWordRound();
                }
                else
                {
                    _feedback.Text = "De volgorde klopt nog niet. Probeer opnieuw.";
                    _feedback.TextColor = Color.FromArgb("#DC2626");
                    await GameFeedback.FailureAsync();
                    await Task.Delay(500);
                    ResetLetters();
                    _busy = false;
                }
            };
            letters.Add(tile);
        }
        var reset = MakeButton("↶  Opnieuw", "#64748B");
        reset.Clicked += (_, _) => ResetLetters();
        _gameArea.Add(answer);
        _gameArea.Add(new Label { Text = "Zonder spaties of leestekens", FontSize = 13, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#64748B") });
        _gameArea.Add(letters);
        _gameArea.Add(reset);
    }

    private async Task CheckChoiceAsync(bool correct, Button selected, VocabularyWord word)
    {
        if (_busy) return;
        _busy = true;
        foreach (var child in _gameArea.Children) if (child is Button button) button.IsEnabled = false;
        selected.BackgroundColor = correct ? Color.FromArgb("#16A34A") : Color.FromArgb("#DC2626");
        if (correct)
        {
            _score++;
            _feedback.Text = $"Goed zo! {word.Dutch} = {word.French}";
            _feedback.TextColor = Color.FromArgb("#15803D");
            await GameFeedback.SuccessAsync();
            await SpeakFrenchAsync(word.French);
        }
        else
        {
            _feedback.Text = $"Het juiste antwoord is {word.French}.";
            _feedback.TextColor = Color.FromArgb("#DC2626");
            await GameFeedback.FailureAsync();
            await SpeakFrenchAsync(word.French);
        }
        await Task.Delay(750);
        _index++;
        ShowSingleWordRound();
    }

    private async Task CompleteAsync()
    {
        if (_busy && _mode != WordGameMode.Connect) return;
        Preferences.Default.Set("stars", Preferences.Default.Get("stars", 0) + _score);
        await DisplayAlert("⭐ Woordspel voltooid", $"Je hebt {_score} van {_roundWords.Length} woorden gevonden en {_score} sterren verdiend!", "Verder");
        await Navigation.PopAsync();
    }

    private static Button MakeButton(string text, string color) => new()
    {
        Text = text,
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        BackgroundColor = Color.FromArgb(color),
        TextColor = Colors.White,
        CornerRadius = 15,
        MinimumHeightRequest = 54,
        Padding = new Thickness(8, 5)
    };

    private static bool IsSpellable(VocabularyWord word)
    {
        var length = Normalize(word.French).Length;
        return length is >= 4 and <= 11;
    }

    private static string Normalize(string text) =>
        new(text.Where(char.IsLetter).Select(char.ToLowerInvariant).ToArray());

    private static async Task SpeakFrenchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var french = locales.FirstOrDefault(locale => locale.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            if (french is not null) await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = french });
        }
        catch { }
    }
}
