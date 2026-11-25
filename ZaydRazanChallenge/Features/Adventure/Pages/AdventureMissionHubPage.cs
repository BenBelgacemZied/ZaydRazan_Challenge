using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge.Features.Adventure.Pages;

public sealed class AdventureMissionHubPage : ContentPage
{
    private readonly IDestinationContent _destination;
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private static readonly string[] Keys =
    [
        "home_pack_zayd",
        "home_pack_razan",
        "home_documents"
    ];

    private readonly Label _speech = new()
    {
        Text = IsEnglish ? "Getting the suitcases ready..." : "De koffers worden klaargemaakt...",
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#17324D"),
        HorizontalTextAlignment = TextAlignment.Center
    };

    private readonly Label _stars = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White
    };

    private readonly Button _resume = new()
    {
        Text = IsEnglish ? "▶ Continue playing" : "▶ Verder spelen",
        IsVisible = false,
        BackgroundColor = Color.FromArgb("#F59E0B"),
        TextColor = Colors.White,
        FontAttributes = FontAttributes.Bold,
        CornerRadius = 20
    };

    // PushAsync returns when a mission appears, not when it is completed.
    // OnAppearing is the only place that starts the next mission after a return.
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";
    private int _launchedMission = -1;
    private bool _navigating;

    public AdventureMissionHubPage(IGameProgressStore progress, ISpeechService speech, IDestinationContent destination)
    {
        _destination = destination;
        _speechService = speech;
        _gameProgress = progress;
        Title = IsEnglish ? "Home" : "Thuis";
        BackgroundColor = Color.FromArgb("#172554");
        GameUi.AddHomeButton(this);

        var playground = new AbsoluteLayout();
        var scene = new Image { Source = "scene_home.jpg", Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(scene, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(scene, AbsoluteLayoutFlags.All);
        playground.Add(scene);

        var characters = GameUi.OfficialCharacters(330);
        AbsoluteLayout.SetLayoutBounds(characters, new Rect(.5, .91, 330, 360));
        AbsoluteLayout.SetLayoutFlags(characters, AbsoluteLayoutFlags.PositionProportional);
        playground.Add(characters);

        var hud = new Border
        {
            Padding = new Thickness(13, 9),
            Margin = 12,
            BackgroundColor = Color.FromArgb("#C917324D"),
            Stroke = Colors.White,
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 },
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                Children =
                {
                    new Label
                    {
                        Text = IsEnglish ? "🏠  HOME ADVENTURE" : "🏠  THUISAVONTUUR",
                        FontSize = 17,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White
                    },
                    _stars
                }
            }
        };
        Grid.SetColumn(_stars, 1);
        AbsoluteLayout.SetLayoutBounds(hud, new Rect(0, 0, 1, 72));
        AbsoluteLayout.SetLayoutFlags(hud, AbsoluteLayoutFlags.WidthProportional);
        playground.Add(hud);

        _resume.Clicked += async (_, _) => await OpenNextMissionAsync();
        var panel = new Border
        {
            Padding = new Thickness(14, 10),
            Margin = new Thickness(18, 0, 18, 14),
            BackgroundColor = Color.FromArgb("#EFFFFFFF"),
            Stroke = Color.FromArgb("#F59E0B"),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 },
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = { _speech, _resume }
            }
        };
        AbsoluteLayout.SetLayoutBounds(panel, new Rect(0, 1, 1, 110));
        AbsoluteLayout.SetLayoutFlags(panel,
            AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
        playground.Add(panel);

        Content = playground;
    }

    private bool _initialLaunchHandled;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_navigating) return;

        if (_launchedMission >= 0)
        {
            var completed = _gameProgress.Get(Keys[_launchedMission], false);
            _launchedMission = -1;
            if (!completed)
            {
                _speech.Text = IsEnglish ? "↩️ You can continue whenever you like." : "↩️ Je kunt verder spelen wanneer je wilt.";
                _resume.IsVisible = true;
            }

            // The mission page explicitly advances after its pop transition completes.
            // Do not start another navigation from inside this appearance callback.
            return;
        }

        if (_initialLaunchHandled) return;
        _initialLaunchHandled = true;
        await OpenNextMissionAsync();
    }

    public Task ContinueAfterMissionAsync() => OpenNextMissionAsync();

    private async Task OpenNextMissionAsync()
    {
        if (_navigating) return;
        _navigating = true;
        _resume.IsVisible = false;

        try
        {
            var nextIndex = Array.FindIndex(Keys,
                key => !_gameProgress.Get(key, false));
            _stars.Text = $"⭐ {Keys.Count(key => _gameProgress.Get(key, false))}/3";

            if (nextIndex < 0)
            {
                _gameProgress.Set("adventure_stage",
                    Math.Max(2, _gameProgress.Get("adventure_stage", 0)));
                _speech.Text = IsEnglish ? "☀️ Well done! On to the station." : "☀️ Goed gedaan! Op naar het station.";
                // In regular play, continue to the station. In test mode, return to
                // the test menu after the house missions rather than chaining into it.
                if (!_gameProgress.IsTestMode)
                    Navigation.InsertPageBefore(new StationMissionHubPage(_gameProgress, _speechService, _destination), this);
                await Navigation.PopAsync();
                return;
            }

            _speech.Text = IsEnglish ? "🎯 The next mission is starting..." : "🎯 De volgende opdracht begint...";
            _launchedMission = nextIndex;
            await Task.Delay(250);
            var mission = nextIndex switch
            {
                0 => PackingMissionPage.ForZayd(_gameProgress, _speechService, _destination),
                1 => PackingMissionPage.ForRazan(_gameProgress, _speechService, _destination),
                _ => PackingMissionPage.ForDocuments(_gameProgress, _speechService, _destination)
            };
            await Navigation.PushAsync(mission);
        }
        finally
        {
            _navigating = false;
        }
    }
}
