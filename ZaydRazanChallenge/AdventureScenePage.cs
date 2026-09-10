namespace ZaydRazanChallenge;

public sealed class AdventureScenePage : ContentPage
{
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
        new("🏠 Préparer le voyage", "scene_home.jpg",
        [
            new("Trouve le sac à dos · Zoek de rugzak", "Trouve le sac à dos", 0,
            [
                new("🎒", .12, .55), new("🧳", .85, .58), new("👟", .14, .72)
            ]),
            new("Trouve un vêtement · Zoek een kledingstuk", "Trouve le vêtement", 1,
            [
                new("🎩", .78, .43), new("👕", .48, .29), new("🎫", .82, .84)
            ]),
            new("Trouve le billet de train · Zoek het treinkaartje", "Le billet de train", 2,
            [
                new("👟", .13, .72), new("🧳", .86, .58), new("🎫", .82, .84)
            ])
        ]),
        new("🎫 Acheter les billets", "scene_station.jpg",
        [
            new("Que disent Zayd et Razan au guichet ?", "Deux billets pour Paris, s'il vous plaît", 1,
            [
                new("Au revoir", .22, .70), new("Deux billets, s’il vous plaît", .50, .63),
                new("Bonne nuit", .78, .70)
            ]),
            new("Choisis la destination correcte", "Nous allons à Paris", 2,
            [
                new("Londres", .22, .67), new("Rome", .50, .60), new("Paris", .78, .67)
            ]),
            new("Comment remercier au guichet ?", "Merci beaucoup", 0,
            [
                new("Merci", .22, .66), new("Bonjour", .50, .59), new("Pardon", .78, .66)
            ])
        ]),
        new("🚉 Trouver le quai", "scene_station.jpg",
        [
            new("Le billet indique le quai 3. Trouve-le !", "Le quai numéro trois", 2,
            [
                new("1", .24, .31), new("2", .50, .25), new("3", .75, .31)
            ]),
            new("Où regarde-t-on les départs ?", "Le tableau des départs", 0,
            [
                new("📋", .28, .18), new("🧳", .82, .52), new("🪑", .78, .75)
            ]),
            new("Quel mot français signifie « quai » ?", "Le quai", 1,
            [
                new("la rue", .24, .68), new("le quai", .50, .60), new("la maison", .77, .68)
            ])
        ]),
        new("🚄 Monter dans le train", "scene_platform.jpg",
        [
            new("Le billet indique le wagon 7. Trouve-le !", "Le wagon numéro sept", 1,
            [
                new("5", .30, .31), new("7", .53, .28), new("9", .76, .31)
            ]),
            new("Trouve la porte du train", "La porte du train", 2,
            [
                new("🪑", .22, .50), new("🧳", .79, .56), new("🚪", .68, .35)
            ]),
            new("Que disent-ils en montant ?", "Bonjour, voici nos billets", 0,
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
    private readonly Label _heroes = new()
    {
        Text = "👦🏽👧🏽",
        FontSize = 34,
        WidthRequest = 82,
        HeightRequest = 50,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly List<View> _clouds = [];
    private readonly double _width;
    private readonly double _height;
    private int _missionIndex;
    private bool _checking;

    public AdventureScenePage(int sceneIndex)
    {
        _sceneIndex = Math.Clamp(sceneIndex, 0, Scenes.Length - 1);
        _scene = Scenes[_sceneIndex];
        Title = _scene.Title;
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
            new Rect(_width * .39, _height * .82, 82, 50));
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

    private void ShowMission()
    {
        _checking = false;
        var mission = _scene.Missions[_missionIndex];
        _instruction.Text = mission.Instruction;
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
                FontSize = choice.Text.Length > 5 ? 13 : 22,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#E6FFFFFF"),
                TextColor = Color.FromArgb("#183153"),
                BorderColor = Color.FromArgb("#F59E0B"),
                BorderWidth = 3,
                CornerRadius = 24,
                Padding = new Thickness(7, 2),
                MinimumWidthRequest = 52,
                HeightRequest = 50
            };
            button.Clicked += async (_, _) => await CheckChoice(selectedIndex, button);
            AbsoluteLayout.SetLayoutBounds(button, new Rect(
                choice.X * _width - 48,
                choice.Y * _height - 25,
                96,
                50));
            _playfield.Children.Add(button);
        }
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
            Preferences.Default.Set("stars",
                Math.Max(0, Preferences.Default.Get("stars", 0) - 1));
            await button.ShakeAsync();
            _checking = false;
            return;
        }

        await GameFeedback.SuccessAsync();
        button.BackgroundColor = Color.FromArgb("#86EFAC");
        Preferences.Default.Set("stars",
            Preferences.Default.Get("stars", 0) + 1);

        var target = mission.Choices[choiceIndex];
        await WalkHeroesTo(
            target.X * _width - (_width * .39 + 41),
            target.Y * _height - (_height * .82 + 25));

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

        await TextToSpeech.Default.SpeakAsync(mission.FrenchPhrase,
            new SpeechOptions { Locale = await FindFrenchLocale() });
        _missionIndex++;

        if (_missionIndex >= _scene.Missions.Length)
        {
            var currentProgress = Preferences.Default.Get("adventure_stage", 0);
            Preferences.Default.Set("adventure_stage",
                Math.Max(currentProgress, _sceneIndex + 1));
            await DisplayAlert("🌤️ Zone découverte !",
                "Bravo ! Zayd et Razan ont terminé cette scène et la prochaine étape est ouverte.",
                "Continuer");
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

    private static async Task<Locale?> FindFrenchLocale() =>
        (await TextToSpeech.Default.GetLocalesAsync())
        .FirstOrDefault(x => x.Language.StartsWith("fr",
            StringComparison.OrdinalIgnoreCase));
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
