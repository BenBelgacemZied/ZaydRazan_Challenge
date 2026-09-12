using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class TrainMissionHubPage : ContentPage
{
    private sealed record TrainObject(string Key, string Icon, string Dutch, string French, double X, double Y);
    private static readonly TrainObject[] Objects =
    [
        new("train_door", "🚪", "de deur", "la porte", .27, .34),
        new("train_seat", "💺", "de stoel", "le siège", .09, .61),
        new("train_rack", "🧳", "het bagagerek", "le porte-bagages", .72, .14),
        new("train_window", "🪟", "het raam", "la fenêtre", .84, .42),
        new("train_table", "▰", "de tafel", "la table", .84, .60)
    ];
    private readonly AbsoluteLayout _playground = new();
    private readonly List<View> _markers = [];
    private readonly Label _stars = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _instruction = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _discovery = new() { FontSize = 24, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center, IsVisible = false };
    private readonly HorizontalStackLayout _choices = new() { Spacing = 6, HorizontalOptions = LayoutOptions.Center };
    private bool _busy;
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
        var characters = GameUi.OfficialCharacters(205);
        AbsoluteLayout.SetLayoutBounds(characters, new Rect(.5, .92, 205, 225));
        AbsoluteLayout.SetLayoutFlags(characters, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(characters);
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
        var discoveryPanel = new Border { Padding = new Thickness(16, 10), BackgroundColor = Color.FromArgb("#D917324D"), Stroke = Colors.White, StrokeThickness = 2, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 }, Content = _discovery };
        AbsoluteLayout.SetLayoutBounds(discoveryPanel, new Rect(.5, .22, 260, 60));
        AbsoluteLayout.SetLayoutFlags(discoveryPanel, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(discoveryPanel);
        discoveryPanel.SetBinding(IsVisibleProperty, new Binding(nameof(Label.IsVisible), source: _discovery));
        var choicesPanel = new Border
        {
            Padding = new Thickness(8, 7),
            Margin = new Thickness(8, 0),
            BackgroundColor = Color.FromArgb("#E617324D"),
            Stroke = Colors.White,
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 },
            Content = _choices
        };
        AbsoluteLayout.SetLayoutBounds(choicesPanel, new Rect(0, .82, 1, 88));
        AbsoluteLayout.SetLayoutFlags(choicesPanel, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
        _playground.Add(choicesPanel);
        var panel = new Border { Padding = new Thickness(14, 10), Margin = new Thickness(18, 0, 18, 14), BackgroundColor = Color.FromArgb("#EFFFFFFF"), Stroke = Color.FromArgb("#F59E0B"), StrokeThickness = 2, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 }, Content = _instruction };
        AbsoluteLayout.SetLayoutBounds(panel, new Rect(0, 1, 1, 74));
        AbsoluteLayout.SetLayoutFlags(panel, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
        _playground.Add(panel);
        Content = _playground;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!Preferences.Default.Get("train_mission_v3_initialized", false))
        {
            foreach (var item in Objects) Preferences.Default.Remove(item.Key);
            Preferences.Default.Set("train_mission_v3_initialized", true);
        }
        Render();
        if (_introPlayed) return;
        _introPlayed = true; await Task.Delay(400); await SpeakCurrentInstruction();
    }

    private void Render()
    {
        foreach (var marker in _markers) _playground.Remove(marker);
        _markers.Clear();
        var completed = Objects.Count(x => Preferences.Default.Get(x.Key, false));
        _stars.Text = $"⭐ {completed}/5";
        BuildChoices(completed);
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
        var button = new Button { Text = done ? $"✓\n{item.Icon}" : $"{index + 1}\n{item.Icon}", FontSize = 23, FontAttributes = FontAttributes.Bold, Padding = 0, WidthRequest = 86, HeightRequest = 86, CornerRadius = 43, BackgroundColor = done ? Color.FromArgb("#B316A34A") : Color.FromArgb("#E6F59E0B"), TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 4, IsEnabled = unlocked, IsVisible = unlocked || done, Shadow = new Shadow { Brush = Colors.Black, Opacity = .5f, Radius = 12, Offset = new Point(0, 5) } };
        button.Clicked += async (_, _) =>
        {
            if (done) { await SpeakFrenchAsync(item.French); return; }
            await CompleteObjectAsync(index, button);
        };
        AbsoluteLayout.SetLayoutBounds(button, new Rect(item.X, item.Y, 86, 86));
        AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
        _playground.Add(button); _markers.Add(button);
        if (unlocked && !done) _ = PulseAsync(button);
    }

    private void BuildChoices(int expectedIndex)
    {
        _choices.Clear();
        if (expectedIndex >= Objects.Length) return;
        for (var i = 0; i < Objects.Length; i++)
        {
            var choiceIndex = i;
            var item = Objects[i];
            var choice = new Button
            {
                Text = $"{item.Icon}\n{item.Dutch}",
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(3),
                WidthRequest = 64,
                HeightRequest = 70,
                CornerRadius = 15,
                BackgroundColor = Colors.White,
                TextColor = Color.FromArgb("#17324D")
            };
            choice.Clicked += async (_, _) =>
            {
                if (_busy) return;
                if (choiceIndex != Objects.Count(x => Preferences.Default.Get(x.Key, false)))
                {
                    _busy = true;
                    choice.BackgroundColor = Color.FromArgb("#FCA5A5");
                    await GameFeedback.FailureAsync();
                    await SpeakDutchAsync("Dat is niet juist. Probeer opnieuw.");
                    choice.BackgroundColor = Colors.White;
                    _busy = false;
                    return;
                }
                await CompleteObjectAsync(choiceIndex, choice);
            };
            _choices.Add(choice);
        }
    }

    private async Task CompleteObjectAsync(int index, View selected)
    {
        if (_busy || index >= Objects.Length || Preferences.Default.Get(Objects[index].Key, false)) return;
        _busy = true;
        var item = Objects[index];
        Preferences.Default.Set(item.Key, true);
        Preferences.Default.Set("stars", Preferences.Default.Get("stars", 0) + 1);
        await GameFeedback.SuccessAsync();
        _discovery.Text = $"{item.Icon}  {item.French}";
        _discovery.IsVisible = true;
        await SpeakFrenchAsync(item.French);
        await selected.ScaleTo(1.18, 160, Easing.CubicOut);
        await selected.ScaleTo(1, 160, Easing.CubicIn);
        await Task.Delay(550);
        _discovery.IsVisible = false;
        _busy = false;
        Render();
        if (index + 1 < Objects.Length) await SpeakCurrentInstruction();
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
