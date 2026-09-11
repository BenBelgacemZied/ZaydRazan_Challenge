namespace ZaydRazanChallenge;

public sealed class StationMiniMissionPage : ContentPage
{
    private sealed record Mission(string Key, string Title, string Story, string Instruction,
        string Image, string Correct, string[] Choices, string Success);

    private static readonly Mission[] Missions =
    [
        new("station_find_counter", "Het loket vinden",
            "Zayd en Razan komen het station binnen. De hal is groot. Ze zoeken waar ze hun tickets kunnen kopen.",
            "Zoek het bord van het loket. Welk symbool toont de tickets?", "station_concourse.jpg",
            "🎫 Les billets", ["🎫 Les billets", "☕ Café", "🧳 Bagages"], "Daar is het loket!"),
        new("station_ask_tickets", "Tickets vragen",
            "Razan staat aan het loket. Ze wil beleefd twee tickets naar Parijs vragen.",
            "Wat moet Razan zeggen?", "station_ticket_counter.jpg",
            "Deux billets pour Paris, s'il vous plaît.", ["Où est le café ?", "Deux billets pour Paris, s'il vous plaît.", "Au revoir Paris !"], "De medewerker maakt de twee tickets klaar."),
        new("station_pay", "De tickets betalen",
            "De tickets kosten twintig euro. Zayd kijkt naar de betaalterminal en de munten op de balie.",
            "Hoe zeg je « betalen » in het Frans?", "station_ticket_counter.jpg",
            "payer", ["voyager", "payer", "manger"], "De betaling is gelukt!"),
        new("station_find_platform", "Het perron vinden",
            "Op het bord staat de trein naar Parijs. Op het ticket staat perron drie.",
            "Kies het juiste perron.", "station_concourse.jpg",
            "Quai 3", ["Quai 1", "Quai 3", "Quai 8"], "Perron 3 is gevonden!"),
        new("station_find_wagon", "De wagon vinden",
            "Op het perron houdt Zayd de tickets vast. Razan zoekt het nummer van hun wagon.",
            "Op het ticket staat wagon zeven. Welke wagon kiezen ze?", "scene_platform.jpg",
            "Wagon 7", ["Wagon 2", "Wagon 7", "Wagon 10"], "Ze stappen in de juiste wagon!")
    ];

    private readonly int _index;
    private readonly Mission _mission;
    private readonly VerticalStackLayout _choices = new() { Spacing = 10 };
    private readonly Label _feedback = new() { FontSize = 19, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
    private bool _answered;

    public StationMiniMissionPage(int index)
    {
        _index = index;
        _mission = Missions[index];
        Title = $"Mission {index + 1}";
        BackgroundColor = Color.FromArgb("#EFF6FF");
        GameUi.AddHomeButton(this);

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
                new Border { Margin = 12, Padding = 12, VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.FromArgb("#D917324D"), StrokeThickness = 0, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 }, Content =
                    new Label { Text = $"MISSION {_index + 1}/5 · {_mission.Title}", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }}
            }},
            new VerticalStackLayout { Padding = 18, Spacing = 12, Children =
            {
                new Label { Text = _mission.Story, FontSize = 17, LineHeight = 1.25 },
                new Label { Text = "🔊 " + _mission.Instruction, FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D") },
                _choices, _feedback
            }}
        }}};
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(300);
        await Speak(_mission.Story + " " + _mission.Instruction);
    }

    private async Task Check(string answer, Button selected)
    {
        if (_answered) return;
        var correct = answer == _mission.Correct;
        if (!correct)
        {
            selected.BackgroundColor = Color.FromArgb("#DC2626");
            _feedback.Text = "Probeer opnieuw. Kijk naar de scène en luister naar de opdracht.";
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
        await Speak("Goed gedaan! " + _mission.Success);
        await Task.Delay(650);
        await DisplayAlert("Missie voltooid", _mission.Success + " Je verdient een ster.", "Verder");
        await Navigation.PopAsync();
    }

    private static async Task Speak(string text)
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
