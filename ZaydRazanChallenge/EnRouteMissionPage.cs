namespace ZaydRazanChallenge;

public sealed class EnRouteMissionPage : ContentPage
{
    private readonly Label _feedback = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private bool _answered;

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

        var question = new Label
        {
            Text = "Welk Frans woord betekent « waar »?",
            FontSize = 21,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.FromArgb("#17324D")
        };

        var hint = new Label
        {
            Text = "Zayd en Razan zoeken samen de weg naar het station.",
            FontSize = 16,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.FromArgb("#475569")
        };

        var answers = new VerticalStackLayout { Spacing = 10 };
        foreach (var answer in new[] { "où", "quand", "merci" })
        {
            var button = new Button
            {
                Text = answer,
                FontSize = 19,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Colors.White,
                TextColor = Color.FromArgb("#17324D"),
                BorderColor = Color.FromArgb("#F59E0B"),
                BorderWidth = 2,
                CornerRadius = 20,
                HeightRequest = 54,
                HorizontalOptions = LayoutOptions.Fill
            };
            button.Clicked += async (_, _) => await CheckAnswer(answer, button);
            answers.Add(button);
        }

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
                            Children = { question, hint, answers, _feedback }
                        }
                    }
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(250);
        await SpeakDutchAsync("Welk Frans woord betekent waar?");
    }

    private async Task CheckAnswer(string answer, Button selected)
    {
        if (_answered) return;
        if (answer != "où")
        {
            selected.BackgroundColor = Color.FromArgb("#FCA5A5");
            _feedback.TextColor = Color.FromArgb("#B91C1C");
            _feedback.Text = "Probeer opnieuw.";
            await GameFeedback.FailureAsync();
            return;
        }

        _answered = true;
        selected.BackgroundColor = Color.FromArgb("#86EFAC");
        _feedback.TextColor = Color.FromArgb("#15803D");
        _feedback.Text = "✅ Où ! Très bien.";
        AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
        AdventureSave.Set("adventure_stage", Math.Max(2, AdventureSave.Get("adventure_stage", 0)));
        await GameFeedback.SuccessAsync();
        await SpeakFrenchAsync("où");
        await DisplayAlert("🌤️ Étape terminée", "La route vers la gare est trouvée. La gare est maintenant ouverte.", "Continuer");
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
