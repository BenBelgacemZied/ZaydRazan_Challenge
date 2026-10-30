namespace ZaydRazanChallenge;

public sealed class EnRouteMissionPage : ContentPage
{
    private sealed record Question(string Dutch, string French, string[] Answers, int Correct);

    private static readonly Question[] Questions =
    [
        new("Hoe vraag je: Waar is het station?", "Où est la gare ?", ["Où est la gare ?", "Merci beaucoup", "Deux billets"], 0),
        new("Hoe zeg je: Links?", "À gauche", ["À droite", "À gauche", "Tout droit"], 1),
        new("Hoe zeg je: Rechts?", "À droite", ["À gauche", "Tout droit", "À droite"], 2),
        new("Hoe zeg je: Ga rechtdoor?", "Tout droit", ["Tout droit", "Au revoir", "Pardon"], 0),
        new("Wat zeg je als je hulp nodig hebt?", "Excusez-moi", ["Bonjour", "Excusez-moi", "Bonne nuit"], 1)
    ];

    private readonly Label _questionLabel = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        TextColor = Color.FromArgb("#17324D")
    };
    private readonly Label _progressLabel = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        TextColor = Color.FromArgb("#B45309")
    };
    private readonly Label _feedback = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly VerticalStackLayout _answers = new() { Spacing = 10 };
    private int _questionIndex;
    private bool _answered;
    private bool _loading;

    public EnRouteMissionPage()
    {
        Title = "Missie · En route";
        BackgroundColor = Color.FromArgb("#E0F2FE");
        GameUi.AddHomeButton(this);

        var hero = new Image
        {
            Source = "adventure_map.jpg",
            Aspect = Aspect.AspectFill,
            HeightRequest = 270
        };

        var hint = new Label
        {
            Text = "Zayd en Razan zoeken samen de weg naar het station.",
            FontSize = 16,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.FromArgb("#475569")
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children =
                {
                    hero,
                    new Border
                    {
                        Margin = new Thickness(14, -20, 14, 0),
                        Padding = 18,
                        BackgroundColor = Colors.White,
                        Stroke = Color.FromArgb("#F59E0B"),
                        StrokeThickness = 2,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 },
                        Content = new VerticalStackLayout
                        {
                            Spacing = 12,
                            Children = { _progressLabel, _questionLabel, hint, _answers, _feedback }
                        }
                    }
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loading) return;
        await ShowQuestionAsync();
    }

    private async Task ShowQuestionAsync()
    {
        _loading = true;
        _answered = false;
        _feedback.Text = string.Empty;
        _answers.Children.Clear();
        _answers.IsVisible = false;

        var question = Questions[_questionIndex];
        _progressLabel.Text = $"Question {_questionIndex + 1} sur {Questions.Length}";
        _questionLabel.Text = question.Dutch;

        for (var i = 0; i < question.Answers.Length; i++)
        {
            var index = i;
            var button = new Button
            {
                Text = question.Answers[i],
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Colors.White,
                TextColor = Color.FromArgb("#17324D"),
                BorderColor = Color.FromArgb("#F59E0B"),
                BorderWidth = 2,
                CornerRadius = 20,
                HeightRequest = 54,
                HorizontalOptions = LayoutOptions.Fill
            };
            button.Clicked += async (_, _) => await CheckAnswerAsync(index, button);
            _answers.Children.Add(button);
        }

        await SpeakDutchAsync(question.Dutch);
        // The child hears the complete Dutch question before seeing the French choices.
        _answers.IsVisible = true;
        _loading = false;
    }

    private async Task CheckAnswerAsync(int answerIndex, Button selected)
    {
        if (_answered || _loading) return;
        var question = Questions[_questionIndex];
        if (answerIndex != question.Correct)
        {
            selected.BackgroundColor = Color.FromArgb("#FCA5A5");
            _feedback.TextColor = Color.FromArgb("#B91C1C");
            _feedback.Text = "Probeer opnieuw.";
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            await GameFeedback.FailureAsync();
            return;
        }

        _answered = true;
        selected.BackgroundColor = Color.FromArgb("#86EFAC");
        _feedback.TextColor = Color.FromArgb("#15803D");
        _feedback.Text = $"✅ {question.French}";
        AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
        await GameFeedback.SuccessAsync();
        await SpeakFrenchAsync(question.French);

        if (_questionIndex < Questions.Length - 1)
        {
            await Task.Delay(450);
            _questionIndex++;
            await ShowQuestionAsync();
            return;
        }

        AdventureSave.Set("adventure_stage", Math.Max(2, AdventureSave.Get("adventure_stage", 0)));
        await DisplayAlert("🌤️ Étape terminée", "Bravo ! La route vers la gare est trouvée. La gare est maintenant ouverte.", "Continuer");
        await Navigation.PopAsync();
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var locale = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale });
        }
        catch { }
    }

    private static async Task SpeakFrenchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var locale = locales.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale });
        }
        catch { }
    }
}
