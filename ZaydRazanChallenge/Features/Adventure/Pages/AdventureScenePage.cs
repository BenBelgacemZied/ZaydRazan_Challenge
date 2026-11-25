using ZaydRazanChallenge.Features.Shared;
namespace ZaydRazanChallenge.Features.Adventure.Pages;

public sealed class AdventureScenePage : ContentPage
{
    private readonly IDestinationContent _destination;
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private sealed record Choice(string Text, double X, double Y);
    private sealed record Mission(
        string Instruction,
        string FrenchPhrase,
        int CorrectChoice,
        Choice[] Choices);
    private sealed record Scene(
        string Title,
        string Background,
        Mission[] Missions);

    private static readonly Scene[] Scenes =
    [
        new("🏠 De reis voorbereiden", "scene_home.jpg",
        [
            new("Zoek de rugzak · « le sac à dos »", "Trouve le sac à dos", 0,
            [
                new("le sac à dos", .12, .55), new("la valise", .85, .58), new("les chaussures", .14, .72)
            ]),
            new("Zoek een kledingstuk · « un vêtement »", "Trouve le vêtement", 1,
            [
                new("le chapeau", .78, .43), new("le tee-shirt", .48, .29), new("le billet", .82, .84)
            ]),
            new("Zoek het treinkaartje · « le billet de train »", "Le billet de train", 2,
            [
                new("les chaussures", .13, .72), new("la valise", .86, .58), new("le billet", .82, .84)
            ])
        ]),
        new("🎫 Tickets kopen", "scene_station.jpg",
        [
            new("Wat zeggen Zayd en Razan aan het loket?", "Deux billets pour Paris, s'il vous plaît", 1,
            [
                new("Au revoir", .22, .70), new("Deux billets, s’il vous plaît", .50, .63),
                new("Bonne nuit", .78, .70)
            ]),
            new("Kies de juiste bestemming.", "Nous allons à Paris", 2,
            [
                new("Londres", .22, .67), new("Rome", .50, .60), new("Paris", .78, .67)
            ]),
            new("Hoe bedank je de medewerker aan het loket?", "Merci beaucoup", 0,
            [
                new("Merci", .22, .66), new("Bonjour", .50, .59), new("Pardon", .78, .66)
            ])
        ]),
        new("🚉 Het perron vinden", "scene_station.jpg",
        [
            new("Op het ticket staat perron 3. Zoek het!", "Le quai numéro trois", 2,
            [
                new("1", .24, .31), new("2", .50, .25), new("3", .75, .31)
            ]),
            new("Waar kijk je naar de vertrektijden?", "Le tableau des départs", 0,
            [
                new("le tableau des départs", .28, .18), new("la valise", .82, .52), new("la chaise", .78, .75)
            ]),
            new("Welk Frans woord betekent « perron »?", "Le quai", 1,
            [
                new("la rue", .24, .68), new("le quai", .50, .60), new("la maison", .77, .68)
            ])
        ]),
        new("🚄 In de trein stappen", "scene_platform.jpg",
        [
            new("Op het ticket staat wagon 7. Zoek hem!", "Le wagon numéro sept", 1,
            [
                new("5", .30, .31), new("7", .53, .28), new("9", .76, .31)
            ]),
            new("Zoek de deur van de trein.", "La porte du train", 2,
            [
                new("la chaise", .22, .50), new("la valise", .79, .56), new("la porte", .68, .35)
            ]),
            new("Wat zeggen ze wanneer ze instappen?", "Bonjour, voici nos billets", 0,
            [
                new("Voici nos billets", .30, .66), new("Bonne nuit", .53, .60),
                new("Je ne sais pas", .76, .66)
            ])
        ])
    ];

    private readonly int _sceneIndex;
    private readonly Scene _scene;
    private readonly AbsoluteLayout _playfield = new();
    private readonly Label _instruction = new()
    {
        FontSize = 20,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        TextColor = Colors.White
    };
    private readonly Label _counter = new()
    {
        FontSize = 15,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White
    };
    private readonly Image _heroes = new()
    {
        Source = "zayd_razan_walk.png",
        Aspect = Aspect.AspectFit,
        WidthRequest = 120,
        HeightRequest = 160
    };
    private readonly List<View> _clouds = [];
    private readonly double _width;
    private readonly double _height;
    private int _missionIndex;
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";
    private bool _checking;

    public AdventureScenePage(int sceneIndex, IGameProgressStore progress, ISpeechService speech, IDestinationContent destination)
    {
        _destination = destination;
        _speechService = speech;
        _gameProgress = progress;
        _sceneIndex = Math.Clamp(sceneIndex, 0, Scenes.Length - 1);
        _scene = Scenes[_sceneIndex];
        Title = LocalizeInstruction(_scene.Title);
        BackgroundColor = Color.FromArgb("#DFF4FF");
        GameUi.AddHomeButton(this);

        var display = DeviceDisplay.Current.MainDisplayInfo;
        _width = Math.Min(430d, Math.Max(300d, display.Width / display.Density));
        _height = _width * 1.5;

        _playfield.WidthRequest = _width;
        _playfield.HeightRequest = _height;
        _playfield.HorizontalOptions = LayoutOptions.Center;
        var backgroundImage = new Image
        {
            Source = _scene.Background,
            Aspect = Aspect.AspectFill,
            WidthRequest = _width,
            HeightRequest = _height,
            InputTransparent = true
        };
        AbsoluteLayout.SetLayoutBounds(
            backgroundImage, new Rect(0, 0, _width, _height));
        _playfield.Children.Add(backgroundImage);

        AddCloud(.02, .03, .96, .27);
        AddCloud(.02, .30, .96, .27);
        AddCloud(.02, .57, .96, .27);

        AbsoluteLayout.SetLayoutBounds(_heroes,
            new Rect(_width * .35, _height * .70, 120, 160));
        _playfield.Children.Add(_heroes);

        var hud = new Border
        {
            Margin = 10,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Color.FromArgb("#E6183153"),
            Stroke = Colors.White,
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
            Padding = 12,
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                Children = { _instruction, _counter }
            }
        };
        Grid.SetColumn(_counter, 1);

        Content = new Grid
        {
            Children = { _playfield, hud }
        };

        ShowMission();
    }

    private void AddCloud(double x, double y, double width, double height)
    {
        var cloud = new Border
        {
            BackgroundColor = Color.FromArgb("#E6F8FCFF"),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 45 },
            InputTransparent = true,
            Content = new Label
            {
                Text = "☁️  ☁️  ☁️",
                FontSize = 52,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                InputTransparent = true
            }
        };
        AbsoluteLayout.SetLayoutBounds(cloud,
            new Rect(x * _width, y * _height, width * _width, height * _height));
        _clouds.Add(cloud);
        _playfield.Children.Add(cloud);
    }

    private async void ShowMission()
    {
        _checking = false;
        var mission = _scene.Missions[_missionIndex];
        _instruction.Text = LocalizeInstruction(mission.Instruction);
        _counter.Text = $"{_missionIndex + 1}/{_scene.Missions.Length}";

        for (var i = _playfield.Children.Count - 1; i >= 0; i--)
            if (_playfield.Children[i] is Button)
                _playfield.Children.RemoveAt(i);

        for (var index = 0; index < mission.Choices.Length; index++)
        {
            var selectedIndex = index;
            var choice = mission.Choices[index];
            var button = new Button
            {
                Text = choice.Text,
                FontSize = choice.Text.Length > 18 ? 11 : choice.Text.Length > 11 ? 12 : 14,
                LineBreakMode = LineBreakMode.WordWrap,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#E6FFFFFF"),
                TextColor = Color.FromArgb("#183153"),
                BorderColor = Color.FromArgb("#F59E0B"),
                BorderWidth = 3,
                CornerRadius = 24,
                Padding = new Thickness(7, 2),
                MinimumWidthRequest = 120,
                HeightRequest = 58
            };
            button.Clicked += async (_, _) => await CheckChoice(selectedIndex, button);
            var buttonWidth = Math.Min(142, _width - 16);
            var buttonX = Math.Clamp(choice.X * _width - buttonWidth / 2, 8, _width - buttonWidth - 8);
            var buttonY = Math.Clamp(choice.Y * _height - 29, 72, _height - 66);
            AbsoluteLayout.SetLayoutBounds(button, new Rect(buttonX, buttonY, buttonWidth, 58));
            _playfield.Children.Add(button);
        }

        _counter.Text =
            $"{_missionIndex + 1}/{_scene.Missions.Length}   ⚡ 10   ⭐ {_gameProgress.Get("stars", 0)}";
        var spokenInstruction = LocalizeInstruction(mission.Instruction.Split('·')[0].Trim());
        await _speechService.SpeakAsync(spokenInstruction, _destination.LanguagePair.SourceLanguage.Value);
    }

    private string LocalizeInstruction(string text)
    {
        if (!IsEnglish) return text;

        var parts = text.Split('·', 2);
        var sourceText = parts[0].Trim();
        var translated = sourceText switch
        {
            "🏠 De reis voorbereiden" => "🏠 Prepare for the trip",
            "🎫 Tickets kopen" => "🎫 Buy tickets",
            "🚉 Het perron vinden" => "🚉 Find the platform",
            "🚄 In de trein stappen" => "🚄 Board the train",
            "Zoek de rugzak" => "Find the backpack",
            "Zoek een kledingstuk" => "Find an item of clothing",
            "Zoek het treinkaartje" => "Find the train ticket",
            "Wat zeggen Zayd en Razan aan het loket?" => "What do Zayd and Razan say at the ticket counter?",
            "Kies de juiste bestemming." => "Choose the right destination.",
            "Hoe bedank je de medewerker aan het loket?" => "How do you thank the ticket clerk?",
            "Op het ticket staat perron 3. Zoek het!" => "The ticket says platform 3. Find it!",
            "Waar kijk je naar de vertrektijden?" => "Where do you check the departure times?",
            "Welk Frans woord betekent « perron »?" => "Which French word means “platform”?",
            "Op het ticket staat wagon 7. Zoek hem!" => "The ticket says carriage 7. Find it!",
            "Zoek de deur van de trein." => "Find the train door.",
            "Wat zeggen ze wanneer ze instappen?" => "What do they say when they board?",
            _ => sourceText
        };

        return parts.Length > 1 ? $"{translated} · {parts[1].Trim()}" : translated;
    }

    private async Task CheckChoice(int choiceIndex, Button button)
    {
        if (_checking) return;
        _checking = true;
        var mission = _scene.Missions[_missionIndex];

        if (choiceIndex != mission.CorrectChoice)
        {
            await GameFeedback.FailureAsync();
            button.BackgroundColor = Color.FromArgb("#FCA5A5");
            _gameProgress.Set("stars",
                Math.Max(0, _gameProgress.Get("stars", 0) - 1));
            await button.ShakeAsync();
            _checking = false;
            return;
        }

        await GameFeedback.SuccessAsync();
        button.BackgroundColor = Color.FromArgb("#86EFAC");
        _gameProgress.Set("stars",
            _gameProgress.Get("stars", 0) + 1);

        var target = mission.Choices[choiceIndex];
        await WalkHeroesTo(
            target.X * _width - (_width * .35 + 60),
            target.Y * _height - (_height * .70 + 80));

        var discovery = new Label
        {
            Text = "🔎 " + mission.FrenchPhrase,
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#183153"),
            BackgroundColor = Color.FromArgb("#F2FFFFFF"),
            Padding = new Thickness(10, 6),
            HorizontalTextAlignment = TextAlignment.Center
        };
        AbsoluteLayout.SetLayoutBounds(discovery, new Rect(
            Math.Clamp(target.X * _width - 80, 6, _width - 166),
            Math.Clamp(target.Y * _height - 82, 70, _height - 90),
            160, 62));
        _playfield.Children.Add(discovery);
        discovery.Opacity = 0;
        await discovery.FadeTo(1, 220);

        if (_missionIndex < _clouds.Count)
        {
            await _clouds[_missionIndex].FadeTo(0, 550, Easing.CubicOut);
            _clouds[_missionIndex].IsVisible = false;
        }

        _missionIndex++;

        if (_missionIndex >= _scene.Missions.Length)
        {
            var currentProgress = _gameProgress.Get("adventure_stage", 0);
            _gameProgress.Set("adventure_stage",
                Math.Max(currentProgress, _sceneIndex + 1));
            await SuccessCelebration.ShowAsync(
                this,
                () => _speechService.SpeakAsync(
                    "Goed gedaan! Zayd en Razan hebben deze scène voltooid. De volgende etappe is nu open.",
                    _destination.LanguagePair.SourceLanguage.Value));
            await Navigation.PopAsync();
            return;
        }

        _heroes.TranslationX = 0;
        _heroes.TranslationY = 0;
        ShowMission();
    }

    private async Task WalkHeroesTo(double destinationX, double destinationY)
    {
        const int steps = 7;
        for (var step = 1; step <= steps; step++)
        {
            var progress = (double)step / steps;
            _heroes.Rotation = step % 2 == 0 ? -5 : 5;
            _heroes.Scale = step % 2 == 0 ? 1.04 : .96;
            await _heroes.TranslateTo(
                destinationX * progress,
                destinationY * progress - (step % 2 == 0 ? 5 : 0),
                90,
                Easing.Linear);
        }
        _heroes.Rotation = 0;
        _heroes.Scale = 1;
        await _heroes.TranslateTo(destinationX, destinationY, 80);
    }

}

public static class AnimationExtensions
{
    public static async Task ShakeAsync(this VisualElement view)
    {
        await view.TranslateTo(-8, 0, 55);
        await view.TranslateTo(8, 0, 55);
        await view.TranslateTo(-5, 0, 55);
        await view.TranslateTo(0, 0, 55);
    }
}
