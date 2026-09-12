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
    private readonly Grid _choices = new()
    {
        ColumnSpacing = 8,
        RowSpacing = 8,
        ColumnDefinitions =
        {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star)
        },
        RowDefinitions =
        {
            new RowDefinition(new GridLength(66)),
            new RowDefinition(new GridLength(66)),
            new RowDefinition(new GridLength(66))
        }
    };
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
        var characters = GameUi.OfficialCharacters(300);
        AbsoluteLayout.SetLayoutBounds(characters, new Rect(.5, .96, 300, 330));
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
        var bottomPanel = new Border
        {
            Padding = new Thickness(10),
            BackgroundColor = Color.FromArgb("#17324D"),
            StrokeThickness = 0,
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    new Border
                    {
                        Padding = new Thickness(10, 7),
                        BackgroundColor = Colors.White,
                        Stroke = Color.FromArgb("#F59E0B"),
                        StrokeThickness = 2,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                        Content = _instruction
                    },
                    _choices
                }
            }
        };
        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(new GridLength(276))
            }
        };
        root.Add(_playground, 0, 0);
        root.Add(bottomPanel, 0, 1);
        Content = root;
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
                Text = $"{item.Icon}  {item.French}",
                FontSize = 15,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(6),
                HeightRequest = 66,
                CornerRadius = 18,
                BackgroundColor = Color.FromArgb("#2563EB"),
                TextColor = Colors.White,
                BorderColor = Colors.White,
                BorderWidth = 2
            };
            choice.Clicked += async (_, _) =>
            {
                if (_busy) return;
                if (choiceIndex != Objects.Count(x => Preferences.Default.Get(x.Key, false)))
                {
                    _busy = true;
                    choice.BackgroundColor = Color.FromArgb("#DC2626");
                    await GameFeedback.FailureAsync();
                    await SpeakDutchAsync("Dat is niet juist. Probeer opnieuw.");
                    choice.BackgroundColor = Color.FromArgb("#2563EB");
                    _busy = false;
                    return;
                }
                await CompleteObjectAsync(choiceIndex, choice);
            };
            _choices.Add(choice, i % 2, i / 2);
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
    private static async Task SpeakDutchAsync(string text) { try { var locales = await TextToSpeech.Default.GetLocalesAsync(); var locale = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase)); await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale }); } catch { } }
    private static async Task SpeakFrenchAsync(string text) { try { var locales = await TextToSpeech.Default.GetLocalesAsync(); var locale = locales.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase)); await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale }); } catch { } }
}
