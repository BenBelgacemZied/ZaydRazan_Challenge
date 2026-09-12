using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class StationMissionHubPage : ContentPage
{
    private static readonly string[] Keys = ["station_find_counter", "station_ask_tickets", "station_pay", "station_find_platform", "station_find_wagon"];
    private readonly AbsoluteLayout _playground = new();
    private readonly List<View> _markers = [];
    private readonly Label _stars = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _instruction = new() { Text = "Tik op de lichtende missie!", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center };
    private bool _introPlayed;

    public StationMissionHubPage()
    {
        Title = "Het station";
        BackgroundColor = Color.FromArgb("#172554");
        GameUi.AddHomeButton(this);

        var scene = new Image { Source = "station_concourse.jpg", Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(scene, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(scene, AbsoluteLayoutFlags.All);
        _playground.Add(scene);

        var shade = new BoxView { Color = Color.FromArgb("#2210203A") };
        AbsoluteLayout.SetLayoutBounds(shade, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(shade, AbsoluteLayoutFlags.All);
        _playground.Add(shade);

        var hud = new Border
        {
            Padding = new Thickness(13, 9), Margin = 12, BackgroundColor = Color.FromArgb("#D917324D"), Stroke = Colors.White, StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 },
            Content = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Children = { new Label { Text = "🚉  STATIONSAVONTUUR", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }, _stars } }
        };
        Grid.SetColumn(_stars, 1);
        AbsoluteLayout.SetLayoutBounds(hud, new Rect(0, 0, 1, 72));
        AbsoluteLayout.SetLayoutFlags(hud, AbsoluteLayoutFlags.WidthProportional);
        _playground.Add(hud);

        var listen = new Button { Text = "🔊", FontSize = 25, CornerRadius = 28, WidthRequest = 56, HeightRequest = 56, Padding = 0, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White };
        listen.Clicked += async (_, _) => await SpeakDutchAsync("Help Zayd en Razan de trein te vinden. Tik op de lichtende missie!");
        AbsoluteLayout.SetLayoutBounds(listen, new Rect(.91, .12, 56, 56));
        AbsoluteLayout.SetLayoutFlags(listen, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(listen);

        var panel = new Border { Padding = new Thickness(14, 10), Margin = new Thickness(18, 0, 18, 14), BackgroundColor = Color.FromArgb("#EFFFFFFF"), Stroke = Color.FromArgb("#F59E0B"), StrokeThickness = 2, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 }, Content = _instruction };
        AbsoluteLayout.SetLayoutBounds(panel, new Rect(0, 1, 1, 74));
        AbsoluteLayout.SetLayoutFlags(panel, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
        _playground.Add(panel);
        Content = _playground;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        RenderMarkers();
        if (_introPlayed) return;
        _introPlayed = true;
        await Task.Delay(400);
        await SpeakDutchAsync("Help Zayd en Razan de trein te vinden. Tik op de lichtende missie!");
    }

    private void RenderMarkers()
    {
        foreach (var marker in _markers) _playground.Remove(marker);
        _markers.Clear();
        _stars.Text = $"⭐ {Keys.Count(k => Preferences.Default.Get(k, false))}/5";
        var positions = new[] { (.18, .34), (.72, .34), (.48, .52), (.22, .70), (.76, .72) };
        var icons = new[] { "🎫", "💬", "💳", "3️⃣", "7️⃣" };
        var names = new[] { "het loket", "de tickets", "betalen", "spoor drie", "wagon zeven" };
        for (var i = 0; i < Keys.Length; i++)
        {
            var complete = Preferences.Default.Get(Keys[i], false);
            var unlocked = i == 0 || Preferences.Default.Get(Keys[i - 1], false);
            AddMarker(i, positions[i].Item1, positions[i].Item2, icons[i], names[i], unlocked, complete);
        }
        if (Keys.All(k => Preferences.Default.Get(k, false)))
        {
            Preferences.Default.Set("adventure_stage", Math.Max(2, Preferences.Default.Get("adventure_stage", 0)));
            _instruction.Text = "☀️ Goed gedaan! De trein is klaar.";
        }
        else _instruction.Text = "🔊 Tik op de lichtende missie";
    }

    private void AddMarker(int index, double x, double y, string icon, string spokenName, bool unlocked, bool complete)
    {
        var button = new Button { Text = complete ? $"✓\n{icon}" : unlocked ? $"{index + 1}\n{icon}" : "☁️\n🔒", FontSize = 22, FontAttributes = FontAttributes.Bold, Padding = 0, WidthRequest = 76, HeightRequest = 76, CornerRadius = 38, TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 3, BackgroundColor = complete ? Color.FromArgb("#16A34A") : unlocked ? Color.FromArgb("#F59E0B") : Color.FromArgb("#BBD1D5DB"), Shadow = new Shadow { Brush = Colors.Black, Opacity = .45f, Radius = 9, Offset = new Point(0, 4) } };
        button.Clicked += async (_, _) =>
        {
            if (!unlocked) { await GameFeedback.FailureAsync(); await SpeakDutchAsync("Speel eerst de lichtende missie."); return; }
            _instruction.Text = $"▶ {spokenName}";
            await SpeakDutchAsync($"Start de missie: {spokenName}.");
            await Navigation.PushAsync(new StationMiniMissionPage(index));
        };
        AbsoluteLayout.SetLayoutBounds(button, new Rect(x, y, 76, 76));
        AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(button);
        _markers.Add(button);
        if (unlocked && !complete) _ = PulseAsync(button);
    }

    private static async Task PulseAsync(View view)
    {
        while (view.Parent is not null && view.IsVisible) { await view.ScaleTo(1.12, 550, Easing.SinInOut); await view.ScaleTo(1, 550, Easing.SinInOut); }
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try { var locales = await TextToSpeech.Default.GetLocalesAsync(); var dutch = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase)); await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = dutch }); } catch { }
    }
}
