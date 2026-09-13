namespace ZaydRazanChallenge;

public sealed class StationMiniMissionPage : ContentPage
{
    private sealed record Mission(string Key, string Title, string Story, string Instruction,
        string Image, string Correct, string[] Choices, string Success);

    private static readonly Mission[] Missions =
    [
        new("station_find_counter", "Vind het loket", "Zayd en Razan zoeken het loket.", "Welk symbool hoort bij de tickets?", "station_concourse.jpg", "🎫 Les billets", ["🎫 Les billets", "☕ Le café", "🧳 Les bagages"], "Jullie hebben het loket gevonden!"),
        new("station_ask_tickets", "Vraag de tickets", "Razan staat aan het loket.", "Welke Franse zin moet Razan zeggen?", "station_ticket_counter.jpg", "Deux billets pour Paris, s'il vous plaît.", ["Où est le café ?", "Deux billets pour Paris, s'il vous plaît.", "Au revoir Paris !"], "De twee tickets zijn klaar!"),
        new("station_pay", "Betaal de tickets", "Zayd ziet het betaaltoestel.", "Welk Frans woord betekent ‘betalen’?", "station_ticket_counter.jpg", "payer", ["voyager", "payer", "manger"], "De betaling is gelukt!"),
        new("station_find_platform", "Vind het spoor", "De trein naar Parijs vertrekt op spoor drie.", "Kies het juiste spoor.", "station_concourse.jpg", "Quai 3", ["Quai 1", "Quai 3", "Quai 8"], "Spoor drie is gevonden!"),
        new("station_find_wagon", "Vind de wagon", "Razan zoekt het nummer op de wagon.", "Kies wagon zeven.", "scene_platform.jpg", "Wagon 7", ["Wagon 2", "Wagon 7", "Wagon 10"], "Jullie zitten in de juiste wagon!")
    ];

    private readonly int _index;
    private readonly Mission _mission;
    private readonly VerticalStackLayout _choices = new() { Spacing = 10 };
    private readonly Label _feedback = new() { FontSize = 19, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private bool _answered;
    private readonly Button _listen = new() { Text = "🔊", FontSize = 24, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White, CornerRadius = 26, WidthRequest = 54, HeightRequest = 54, Padding = 0, HorizontalOptions = LayoutOptions.Center };

    public StationMiniMissionPage(int index)
    {
        _index = index;
        _mission = Missions[index];
        Title = $"Mission {index + 1}";
        BackgroundColor = Color.FromArgb("#EFF6FF");
        GameUi.AddHomeButton(this);
        _listen.Clicked += async (_, _) => await SpeakDutch(_mission.Story + " " + _mission.Instruction);

        foreach (var choice in _mission.Choices.OrderBy(_ => Random.Shared.Next()))
        {
            var button = new Button { Text = choice, FontSize = 17, FontAttributes = FontAttributes.Bold, BackgroundColor = Color.FromArgb("#2563EB"), TextColor = Colors.White, CornerRadius = 17, HeightRequest = 55 };
            button.Clicked += async (_, _) => await Check(choice, button);
            _choices.Add(button);
        }

        Content = new ScrollView { Content = new VerticalStackLayout { Spacing = 0, Children =
        {
            new Grid { HeightRequest = 390, Children =
            {
                new Image { Source = _mission.Image, Aspect = Aspect.AspectFill },
                new BoxView { Color = Color.FromArgb("#3310203A") },
                GameUi.OfficialCharacters(235),
                new Border { Margin = 12, Padding = 12, VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.FromArgb("#D917324D"), StrokeThickness = 0, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 }, Content =
                    new Label { Text = $"MISSION {_index + 1}/5 · {_mission.Title}", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }}
            }},
            new VerticalStackLayout { Padding = 18, Spacing = 12, Children =
            {
                _listen,
                new Label { Text = _mission.Instruction, FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center },
                _choices, _feedback
            }}
        }}};
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(300);
        await SpeakDutch(_mission.Story + " " + _mission.Instruction);
    }

    private async Task Check(string answer, Button selected)
    {
        if (_answered) return;
        var correct = answer == _mission.Correct;
        if (!correct)
        {
            selected.BackgroundColor = Color.FromArgb("#DC2626");
            _feedback.Text = "Kijk goed en probeer opnieuw.";
            _feedback.TextColor = Color.FromArgb("#DC2626");
            await GameFeedback.FailureAsync();
            await selected.TranslateTo(-10, 0, 70); await selected.TranslateTo(10, 0, 70); await selected.TranslateTo(0, 0, 70);
            selected.BackgroundColor = Color.FromArgb("#2563EB");
            return;
        }

        _answered = true;
        selected.BackgroundColor = Color.FromArgb("#16A34A");
        Preferences.Default.Set(_mission.Key, true);
        var stars = Preferences.Default.Get("stars", 0) + 1;
        Preferences.Default.Set("stars", stars);
        _feedback.Text = $"⭐ {_mission.Success}";
        _feedback.TextColor = Color.FromArgb("#16A34A");
        await GameFeedback.SuccessAsync();
        await SpeakDutch("Goed gedaan! " + _mission.Success);
        await Task.Delay(650);
        await DisplayAlert("⭐ Missie voltooid!", _mission.Success, "Verder");
        if (_index == Missions.Length - 1)
            Preferences.Default.Set("adventure_stage", 3);

        await Navigation.PopAsync();
    }

    private static async Task SpeakDutch(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var dutch = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = dutch });
        }
        catch { }
    }
}
