using ZaydRazanChallenge.Features.Shared;
using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge.Features.Adventure.Pages;

public sealed class TrainMissionHubPage : ContentPage
{
    private readonly IDestinationContent _destination;
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private sealed record TrainObject(string Key, string Icon, string Dutch, string English, string French, double X, double Y);
    private static readonly TrainObject[] Objects =
    [
        new("train_door", "🚪", "de deur", "the door", "la porte", .27, .34),
        new("train_seat", "💺", "de stoel", "the seat", "le siège", .09, .61),
        new("train_rack", "🧳", "het bagagerek", "the luggage rack", "le porte-bagages", .72, .14),
        new("train_window", "🪟", "het raam", "the window", "la fenêtre", .84, .42),
        new("train_table", "▰", "de tafel", "the table", "la table", .84, .60)
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
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";
    private bool _busy;
    private bool _introPlayed;
    private readonly int? _testEndAt;

    public TrainMissionHubPage(IGameProgressStore progress, ISpeechService speech, IDestinationContent destination, int? testIndex = null)
    {
        _destination = destination;
        _speechService = speech;
        _gameProgress = progress;
        _testEndAt = testIndex;
        if (testIndex is int index && _gameProgress.IsTestMode)
        {
            if (index < 0 || index >= Objects.Length) throw new ArgumentOutOfRangeException(nameof(testIndex));
            _gameProgress.Set("train_mission_v3_initialized", true);
            for (var i = 0; i < index; i++) _gameProgress.Set(Objects[i].Key, true);
        }
        Title = IsEnglish ? "On the train" : "In de trein";
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
        var hud = new Border { Padding = new Thickness(13, 9), Margin = 12, BackgroundColor = Color.FromArgb("#D917324D"), Stroke = Colors.White, StrokeThickness = 1, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 }, Content = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Children = { new Label { Text = IsEnglish ? "🚄  TRAIN ADVENTURE" : "🚄  TREINAVONTUUR", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White }, _stars } } };
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
        if (!_gameProgress.Get("train_mission_v3_initialized", false))
        {
            foreach (var item in Objects) _gameProgress.Remove(item.Key);
            _gameProgress.Set("train_mission_v3_initialized", true);
        }
        Render();
        if (_introPlayed) return;
        _introPlayed = true; await Task.Delay(400); await SpeakCurrentInstruction();
    }

    private void Render()
    {
        foreach (var marker in _markers) _playground.Remove(marker);
        _markers.Clear();
        var completed = Objects.Count(x => _gameProgress.Get(x.Key, false));
        _stars.Text = $"⭐ {completed}/5";
        BuildChoices(completed);
        if (completed == Objects.Length)
        {
            _gameProgress.Set("adventure_stage", Math.Max(4, _gameProgress.Get("adventure_stage", 0)));
            _instruction.Text = IsEnglish ? "☀️ Well done! Paris is getting closer." : "☀️ Goed gedaan! Parijs komt dichterbij.";
            var next = new Button { Text = IsEnglish ? "▶  CONTINUE" : "▶  VERDER", FontSize = 18, FontAttributes = FontAttributes.Bold, CornerRadius = 24, WidthRequest = 190, HeightRequest = 52, BackgroundColor = Color.FromArgb("#16A34A"), TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 2 };
            next.Clicked += async (_, _) => await Navigation.PopAsync();
            AbsoluteLayout.SetLayoutBounds(next, new Rect(.5, .84, 190, 52));
            AbsoluteLayout.SetLayoutFlags(next, AbsoluteLayoutFlags.PositionProportional);
            _playground.Add(next); _markers.Add(next);
        }
        else _instruction.Text = $"🔎 {(IsEnglish ? Objects[completed].English : Objects[completed].Dutch)}";
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
                Text = item.French,
                FontSize = 16,
                LineBreakMode = LineBreakMode.WordWrap,
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
                if (choiceIndex != Objects.Count(x => _gameProgress.Get(x.Key, false)))
                {
                    _busy = true;
                    choice.BackgroundColor = Color.FromArgb("#DC2626");
                    await GameFeedback.FailureAsync();
                    await SpeakDutchAsync(IsEnglish ? "That’s not right. Try again." : "Dat is niet juist. Probeer opnieuw.");
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
        if (_busy || index >= Objects.Length || _gameProgress.Get(Objects[index].Key, false)) return;
        _busy = true;
        var item = Objects[index];
        _gameProgress.Set(item.Key, true);
        _gameProgress.Set("stars", _gameProgress.Get("stars", 0) + 1);
        await GameFeedback.SuccessAsync();
        _discovery.Text = $"{item.Icon}  {item.French}";
        _discovery.IsVisible = true;
        await SpeakFrenchAsync(item.French);
        await selected.ScaleTo(1.18, 160, Easing.CubicOut);
        await selected.ScaleTo(1, 160, Easing.CubicIn);
        await Task.Delay(550);
        _discovery.IsVisible = false;

        var completed = Objects.Count(x => _gameProgress.Get(x.Key, false));
        if ((_testEndAt.HasValue && _gameProgress.IsTestMode) || completed >= Objects.Length)
        {
            if (completed >= Objects.Length)
                _gameProgress.Set("adventure_stage", Math.Max(4, _gameProgress.Get("adventure_stage", 0)));

            await SuccessCelebration.ShowAsync(
                this,
                () => SpeakDutchAsync(IsEnglish ? "Well done! Paris is getting closer." : "Goed gedaan! Parijs komt dichterbij."));
            _busy = false;
            await Navigation.PopAsync();
            return;
        }

        _busy = false;
        Render();
        await SpeakCurrentInstruction();
    }

    private async Task SpeakCurrentInstruction()
    {
        var completed = Objects.Count(x => _gameProgress.Get(x.Key, false));
        await SpeakDutchAsync(completed >= Objects.Length
            ? (IsEnglish ? "Well done! Paris is getting closer." : "Goed gedaan! Parijs komt dichterbij.")
            : (IsEnglish ? $"Find {Objects[completed].English}." : $"Zoek {Objects[completed].Dutch}."));
    }
    private Task SpeakDutchAsync(string text) => _speechService.SpeakAsync(text, _destination.LanguagePair.SourceLanguage.Value);
    private Task SpeakFrenchAsync(string text) => _speechService.SpeakAsync(text, _destination.LanguagePair.TargetLanguage.Value);
}
