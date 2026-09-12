using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class TrainMissionHubPage : ContentPage
{
    private sealed record TrainObject(string Key, string Icon, string Dutch, string French, double X, double Y);
    private static readonly TrainObject[] Objects =
    [
        new("train_door", "🚪", "de deur", "la porte", .18, .32),
        new("train_seat", "💺", "de stoel", "le siège", .10, .61),
        new("train_rack", "🧳", "het bagagerek", "le porte-bagages", .70, .15),
        new("train_window", "🪟", "het raam", "la fenêtre", .86, .43),
        new("train_table", "▰", "de tafel", "la table", .84, .67)
    ];
    private readonly AbsoluteLayout _playground = new();
    private readonly List<View> _markers = [];
    private readonly Label _stars = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _instruction = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center };
    private bool _introPlayed;

    public TrainMissionHubPage()
    {
        Title = "In de trein";
        BackgroundColor = Color.FromArgb("#172554");
        GameUi.AddHomeButton(this);
        var scene = new Image { Source = "scene_train_interior.jpg", Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(scene, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(scene, AbsoluteLayoutFlags.All);
        _playground.Add(scene);
        var hud = new Border { Padding = new Thickness(13, 9), Margin = 12, BackgroundColor = Color.FromArgb("#D917324D"), Stroke = Colors.White, StrokeThickness = 1, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 }, Content = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Children = { new Label { Text = "🚄  TREINAVONTUUR", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }, _stars } } };
        Grid.SetColumn(_stars, 1);
        AbsoluteLayout.SetLayoutBounds(hud, new Rect(0, 0, 1, 72));
        AbsoluteLayout.SetLayoutFlags(hud, AbsoluteLayoutFlags.WidthProportional);
        _playground.Add(hud);
        var listen = new Button { Text = "🔊", FontSize = 25, CornerRadius = 28, WidthRequest = 56, HeightRequest = 56, Padding = 0, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White };
        listen.Clicked += async (_, _) => await SpeakCurrentInstruction();
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
        base.OnAppearing(); Render();
        if (_introPlayed) return;
        _introPlayed = true; await Task.Delay(400); await SpeakCurrentInstruction();
    }

    private void Render()
    {
        foreach (var marker in _markers) _playground.Remove(marker);
        _markers.Clear();
        var completed = Objects.Count(x => Preferences.Default.Get(x.Key, false));
        _stars.Text = $"⭐ {completed}/5";
        for (var i = 0; i < Objects.Length; i++)
        {
            var done = Preferences.Default.Get(Objects[i].Key, false);
            AddMarker(i, Objects[i], i == 0 || Preferences.Default.Get(Objects[i - 1].Key, false), done);
        }
        if (completed == Objects.Length)
        {
            Preferences.Default.Set("adventure_stage", Math.Max(4, Preferences.Default.Get("adventure_stage", 0)));
            _instruction.Text = "☀️ Goed gedaan! Parijs komt dichterbij.";
            var next = new Button { Text = "▶  VERDER", FontSize = 18, FontAttributes = FontAttributes.Bold, CornerRadius = 24, WidthRequest = 190, HeightRequest = 52, BackgroundColor = Color.FromArgb("#16A34A"), TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 2 };
            next.Clicked += async (_, _) => await Navigation.PopAsync();
            AbsoluteLayout.SetLayoutBounds(next, new Rect(.5, .84, 190, 52));
            AbsoluteLayout.SetLayoutFlags(next, AbsoluteLayoutFlags.PositionProportional);
            _playground.Add(next); _markers.Add(next);
        }
        else _instruction.Text = $"🔎 Zoek {Objects[completed].Dutch}";
    }

    private void AddMarker(int index, TrainObject item, bool unlocked, bool done)
    {
        var button = new Button { Text = done ? $"✓\n{item.Icon}" : unlocked ? $"{index + 1}\n{item.Icon}" : "☁️\n🔒", FontSize = 21, FontAttributes = FontAttributes.Bold, Padding = 0, WidthRequest = 72, HeightRequest = 72, CornerRadius = 36, BackgroundColor = done ? Color.FromArgb("#16A34A") : unlocked ? Color.FromArgb("#F59E0B") : Color.FromArgb("#BBD1D5DB"), TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 3, Shadow = new Shadow { Brush = Colors.Black, Opacity = .45f, Radius = 9, Offset = new Point(0, 4) } };
        button.Clicked += async (_, _) =>
        {
            if (!unlocked) { await GameFeedback.FailureAsync(); await SpeakDutchAsync("Zoek eerst het lichtende voorwerp."); return; }
            if (done) { await SpeakFrenchAsync(item.French); return; }
            Preferences.Default.Set(item.Key, true);
            Preferences.Default.Set("stars", Preferences.Default.Get("stars", 0) + 1);
            await GameFeedback.SuccessAsync(); await SpeakFrenchAsync(item.French);
            await button.ScaleTo(1.25, 180, Easing.CubicOut); await button.ScaleTo(1, 180, Easing.CubicIn);
            Render(); if (index + 1 < Objects.Length) await SpeakCurrentInstruction();
        };
        AbsoluteLayout.SetLayoutBounds(button, new Rect(item.X, item.Y, 72, 72));
        AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(button); _markers.Add(button);
        if (unlocked && !done) _ = PulseAsync(button);
    }

    private async Task SpeakCurrentInstruction()
    {
        var completed = Objects.Count(x => Preferences.Default.Get(x.Key, false));
        await SpeakDutchAsync(completed >= Objects.Length ? "Goed gedaan! Parijs komt dichterbij." : $"Zoek {Objects[completed].Dutch}.");
    }
    private static async Task PulseAsync(View view) { while (view.Parent is not null && view.IsVisible) { await view.ScaleTo(1.12, 550, Easing.SinInOut); await view.ScaleTo(1, 550, Easing.SinInOut); } }
    private static async Task SpeakDutchAsync(string text) { try { var locales = await TextToSpeech.Default.GetLocalesAsync(); var locale = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase)); await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale }); } catch { } }
    private static async Task SpeakFrenchAsync(string text) { try { var locales = await TextToSpeech.Default.GetLocalesAsync(); var locale = locales.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase)); await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale }); } catch { } }
}
