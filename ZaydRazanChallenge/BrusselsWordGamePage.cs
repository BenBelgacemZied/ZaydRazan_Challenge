namespace ZaydRazanChallenge;

public sealed class BrusselsWordGamePage : ContentPage
{
    private sealed record Word(string Dutch, string French);

    private static readonly Word[] Words =
    [
        new("het station", "la gare"),
        new("het standbeeld", "la statue"),
        new("de wafel", "la gaufre"),
        new("klein", "petit"),
        new("groot", "grand"),
        new("het plein", "la place"),
        new("kijken", "regarder"),
        new("mooi", "magnifique")
    ];

    private readonly VerticalStackLayout _answers = new() { Spacing = 9 };
    private readonly Label _question = new() { FontSize = 22, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#17324D") };
    private readonly Label _progress = new() { FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#2563EB") };
    private readonly Label _feedback = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private int _index;
    private int _score;
    private bool _busy;
    private bool _ready;

    public BrusselsWordGamePage()
    {
        Title = "Jeux de mots · Bruxelles";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);
        Content = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) },
            Children =
            {
                new VerticalStackLayout
                {
                    Padding = 16,
                    Spacing = 12,
                    Children =
                    {
                        new Label { Text = "🔤 Les mots de Bruxelles", FontSize = 27, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#7C3AED"), HorizontalTextAlignment = TextAlignment.Center },
                        new Label { Text = "Écoute le néerlandais puis choisis le français.", FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#475569") },
                        _progress,
                        _question,
                        _answers,
                        _feedback
                    }
                },
                new BoxView { Color = Color.FromArgb("#D62828"), HeightRequest = 10, VerticalOptions = LayoutOptions.End }
            }
        };
        Render();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SpeakQuestionAsync();
    }

    private void Render()
    {
        _busy = false;
        _ready = false;
        _progress.Text = $"Bruxelles · mot {_index + 1}/{Words.Length} · ⭐ {_score}";
        _question.Text = $"Comment dit-on « {Words[_index].Dutch} » en français ?";
        _feedback.Text = "";
        _answers.Clear();
        var options = Words.Where((_, i) => i != _index).OrderBy(_ => Random.Shared.Next()).Take(2).Select(x => x.French).Append(Words[_index].French).OrderBy(_ => Random.Shared.Next());
        foreach (var option in options)
        {
            var answer = option;
            var button = new Button
            {
                Text = answer,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#2563EB"),
                TextColor = Colors.White,
                CornerRadius = 14,
                MinimumHeightRequest = 52,
                IsEnabled = false
            };
            button.Clicked += async (_, _) => await AnswerAsync(answer, button);
            _answers.Add(button);
        }
    }

    private async Task SpeakQuestionAsync()
    {
        await SpeakDutchAsync(Words[_index].Dutch);
        if (_busy) return;
        _ready = true;
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = true;
    }

    private async Task AnswerAsync(string answer, Button selected)
    {
        if (_busy || !_ready) return;
        _busy = true;
        _ready = false;
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = false;

        await SpeakFrenchAsync(answer);
        if (answer == Words[_index].French)
        {
            _score++;
            _feedback.Text = $"Bravo ! {Words[_index].Dutch} = {Words[_index].French}";
            _feedback.TextColor = Color.FromArgb("#15803D");
            AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
            await GameFeedback.SuccessAsync();
        }
        else
        {
            _feedback.Text = $"La bonne réponse est : {Words[_index].French}";
            _feedback.TextColor = Color.FromArgb("#DC2626");
            await GameFeedback.FailureAsync();
        }

        await Task.Delay(650);
        _index++;
        if (_index >= Words.Length)
        {
            await DisplayAlert("🏆 Jeu terminé", $"{_score}/{Words.Length} mots trouvés.", "Continuer");
            await Navigation.PopAsync();
            return;
        }
        Render();
        await SpeakQuestionAsync();
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var dutch = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, dutch is null ? null : new SpeechOptions { Locale = dutch });
        }
        catch { }
    }

    private static async Task SpeakFrenchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var french = locales.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, french is null ? null : new SpeechOptions { Locale = french });
        }
        catch { }
    }
}
