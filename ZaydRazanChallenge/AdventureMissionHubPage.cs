using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class AdventureMissionHubPage : ContentPage
{
    private readonly AbsoluteLayout _playground = new();
    private readonly Label _speech = new() { Text = "Tik op het lichtende voorwerp!", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _stars = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly List<View> _activeMarkers = [];
    private bool _introPlayed;

    public AdventureMissionHubPage()
    {
        Title = "Thuis";
        BackgroundColor = Color.FromArgb("#172554");
        GameUi.AddHomeButton(this);

        var scene = new Image { Source = "scene_home.jpg", Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(scene, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(scene, AbsoluteLayoutFlags.All);
        _playground.Add(scene);

        var characters = GameUi.OfficialCharacters(330);
        AbsoluteLayout.SetLayoutBounds(characters, new Rect(.5, .91, 330, 360));
        AbsoluteLayout.SetLayoutFlags(characters, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(characters);

        var hud = new Border
        {
            Padding = new Thickness(13, 9), Margin = 12, BackgroundColor = Color.FromArgb("#C917324D"), Stroke = Colors.White, StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 },
            Content = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                Children = { new Label { Text = "🏠  THUISAVONTUUR", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }, _stars }
            }
        };
        Grid.SetColumn(_stars, 1);
        AbsoluteLayout.SetLayoutBounds(hud, new Rect(0, 0, 1, 72));
        AbsoluteLayout.SetLayoutFlags(hud, AbsoluteLayoutFlags.WidthProportional);
        _playground.Add(hud);

        var voiceButton = new Button { Text = "🔊", FontSize = 25, CornerRadius = 28, WidthRequest = 56, HeightRequest = 56, Padding = 0, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White };
        voiceButton.Clicked += async (_, _) => await SpeakDutchAsync("Zayd en Razan gaan op reis. Tik op het lichtende voorwerp en help hen de koffers klaar te maken!");
        AbsoluteLayout.SetLayoutBounds(voiceButton, new Rect(.91, .12, 56, 56));
        AbsoluteLayout.SetLayoutFlags(voiceButton, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(voiceButton);

        var speechPanel = new Border
        {
            Padding = new Thickness(14, 10), Margin = new Thickness(18, 0, 18, 14), BackgroundColor = Color.FromArgb("#EFFFFFFF"),
            Stroke = Color.FromArgb("#F59E0B"), StrokeThickness = 2, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 }, Content = _speech
        };
        AbsoluteLayout.SetLayoutBounds(speechPanel, new Rect(0, 1, 1, 74));
        AbsoluteLayout.SetLayoutFlags(speechPanel, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
        _playground.Add(speechPanel);
        Content = _playground;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        RenderMissionMarkers();
        if (!_introPlayed)
        {
            _introPlayed = true;
            await Task.Delay(450);
            await SpeakDutchAsync("Zayd en Razan gaan op reis. Tik op het lichtende voorwerp!");
        }
    }

    private void RenderMissionMarkers()
    {
        foreach (var marker in _activeMarkers) _playground.Remove(marker);
        _activeMarkers.Clear();
        var zaydDone = Preferences.Default.Get("home_pack_zayd", false);
        var razanDone = Preferences.Default.Get("home_pack_razan", false);
        var documentsDone = Preferences.Default.Get("home_documents", false);
        _stars.Text = $"⭐ {Convert.ToInt32(zaydDone) + Convert.ToInt32(razanDone) + Convert.ToInt32(documentsDone)}/3";
        AddMarker(.84, .57, "1", "🧳", true, zaydDone, "Zayds koffer", () => Navigation.PushAsync(PackingMissionPage.ForZayd()));
        AddMarker(.14, .61, "2", "🎒", zaydDone, razanDone, "Razans koffer", () => Navigation.PushAsync(PackingMissionPage.ForRazan()));
        AddMarker(.78, .88, "3", "🎫", razanDone, documentsDone, "Reispapieren", () => Navigation.PushAsync(PackingMissionPage.ForDocuments()));
        if (zaydDone && razanDone && documentsDone)
        {
            if (Preferences.Default.Get("adventure_stage", 0) < 1) Preferences.Default.Set("adventure_stage", 1);
            _speech.Text = "☀️ Goed gedaan! Het station is open!";
        }
        else _speech.Text = "🔊 Tik op het lichtende voorwerp";
    }

    private void AddMarker(double x, double y, string number, string icon, bool unlocked, bool completed, string spokenName, Func<Task> open)
    {
        var button = new Button
        {
            Text = completed ? $"✓\n{icon}" : unlocked ? $"{number}\n{icon}" : "☁️\n🔒", FontSize = 22, FontAttributes = FontAttributes.Bold, Padding = 0,
            WidthRequest = 76, HeightRequest = 76, CornerRadius = 38,
            BackgroundColor = completed ? Color.FromArgb("#16A34A") : unlocked ? Color.FromArgb("#F59E0B") : Color.FromArgb("#BBD1D5DB"),
            TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 3,
            Shadow = new Shadow { Brush = Colors.Black, Opacity = .45f, Radius = 9, Offset = new Point(0, 4) }
        };
        button.Clicked += async (_, _) =>
        {
            if (!unlocked)
            {
                await GameFeedback.FailureAsync();
                await SpeakDutchAsync("Deze missie zit nog achter de wolken. Speel eerst de lichtende missie.");
                return;
            }
            _speech.Text = $"▶ {spokenName}";
            await SpeakDutchAsync($"Start de missie: {spokenName}.");
            await open();
        };
        AbsoluteLayout.SetLayoutBounds(button, new Rect(x, y, 76, 76));
        AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(button);
        _activeMarkers.Add(button);
        if (unlocked && !completed) _ = PulseAsync(button);
    }

    private static async Task PulseAsync(View view)
    {
        while (view.Parent is not null && view.IsVisible)
        {
            await view.ScaleTo(1.12, 550, Easing.SinInOut);
            await view.ScaleTo(1.0, 550, Easing.SinInOut);
        }
    }

    private static async Task SpeakDutchAsync(string text)
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
