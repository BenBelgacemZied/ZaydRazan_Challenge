namespace ZaydRazanChallenge;

public sealed class BrusselsMissionPage : ContentPage
{
    private readonly int _missionIndex;
    private readonly BrusselsMission _mission;
    private readonly Grid _hero = new();
    private readonly VerticalStackLayout _answers = new() { Spacing = 8 };
    private readonly Label _question = new() { FontSize = 19, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#17324D") };
    private readonly Label _feedback = new() { FontSize = 15, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _counter = new() { FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#2563EB") };
    private readonly Button _listen = new() { Text = "🔊 Écouter", BackgroundColor = Color.FromArgb("#7C3AED"), TextColor = Colors.White, CornerRadius = 18 };
    private int _questionIndex;
    private bool _busy;
    private bool _ready;

    private static readonly string[] Palette = ["#D62828", "#0077B6", "#F4B942", "#7C3AED", "#15803D", "#EA580C"];

    public BrusselsMissionPage(int missionIndex)
    {
        if (missionIndex < 0 || missionIndex >= BrusselsCatalog.Missions.Count)
            throw new ArgumentOutOfRangeException(nameof(missionIndex));

        _missionIndex = missionIndex;
        _mission = BrusselsCatalog.Missions[missionIndex];
        Title = $"Bruxelles · {_mission.Title}";
        BackgroundColor = Color.FromArgb("#EAF8FF");
        GameUi.AddHomeButton(this);

        _hero.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        _hero.Children.Add(new Image { Source = "brussels_hero.svg", Aspect = Aspect.AspectFill });
        _hero.Children.Add(new BoxView { Color = Color.FromArgb("#9917324D") });
        _hero.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        _hero.Children.Add(new Label
        {
            Text = _mission.Icon,
            FontSize = 88,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        });
        _hero.Children.Add(new Label
        {
            Text = $"{_mission.Title}\\n{_mission.Subtitle}",
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(15, 0, 15, 18)
        });
        Microsoft.Maui.Controls.Grid.SetRow(_hero.Children[1], 1);

        _counter.Text = $"Bruxelles · mission {_missionIndex + 1}/{BrusselsCatalog.Missions.Count}";

        var panel = new Border
        {
            Padding = new Thickness(14, 12),
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#D62828"),
            StrokeThickness = 3,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 },
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = { _counter, new Label { Text = _mission.Story, FontSize = 14, TextColor = Color.FromArgb("#475569") }, _question, _listen, _answers, _feedback }
            }
        };
        _listen.Clicked += async (_, _) => await SpeakCurrentAsync();
        _hero.MinimumHeightRequest = 250;

        var root = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) },
            Padding = new Thickness(10, 10, 10, 10)
        };
        root.Add(_hero, 0, 0);
        root.Add(panel, 0, 1);
        Content = root;

        RenderQuestion();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SpeakQuestionAsync();
    }

    private void RenderQuestion()
    {
        _busy = false;
        _ready = false;
        _question.Text = _mission.Questions[_questionIndex];
        _feedback.Text = "";
        _answers.Clear();
        var choices = _mission.Choices[_questionIndex];
        foreach (var answer in choices.OrderBy(_ => Random.Shared.Next()))
        {
            var choice = answer;
            var button = new Button
            {
                Text = choice,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#2563EB"),
                TextColor = Colors.White,
                CornerRadius = 13,
                MinimumHeightRequest = 48,
                IsEnabled = false,
                AutomationId = $"brussels-answer-{_questionIndex}-{choice}"
            };
            button.Clicked += async (_, _) => await AnswerAsync(choice, button);
            _answers.Add(button);
        }
    }

    private async Task SpeakQuestionAsync()
    {
        await SpeakDutchAsync(_question.Text);
        if (_busy) return;
        _ready = true;
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = true;
    }

    private async Task SpeakCurrentAsync() => await SpeakDutchAsync(_question.Text);

    private async Task AnswerAsync(string answer, Button selected)
    {
        if (_busy || !_ready) return;
        _busy = true;
        _ready = false;
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = false;

        await SpeakFrenchAsync(answer);
        var correct = answer == _mission.CorrectAnswers[_questionIndex];
        selected.BackgroundColor = Color.FromArgb(correct ? "#15803D" : "#DC2626");
        if (correct)
        {
            AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
            _feedback.TextColor = Color.FromArgb("#15803D");
            _feedback.Text = "Bravo ! Tu avances dans Bruxelles ⭐";
            await GameFeedback.SuccessAsync();
            await Task.Delay(500);
            _questionIndex++;
            if (_questionIndex >= _mission.Questions.Length)
            {
                var next = Math.Max(AdventureSave.Get("brussels_stage", 0), _missionIndex + 1);
                AdventureSave.Set("brussels_stage", next);
                await DisplayAlert("🎉 Monument découvert", $"Tu as découvert {_mission.Title} !", "Continuer");
                await Navigation.PopAsync();
                return;
            }
            RenderQuestion();
            await SpeakQuestionAsync();
        }
        else
        {
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            _feedback.TextColor = Color.FromArgb("#DC2626");
            _feedback.Text = "Presque ! Écoute encore la question.";
            await GameFeedback.FailureAsync();
            await Task.Delay(550);
            _busy = false;
            _ready = true;
            foreach (var child in _answers.Children)
                if (child is Button button) button.IsEnabled = true;
        }
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
