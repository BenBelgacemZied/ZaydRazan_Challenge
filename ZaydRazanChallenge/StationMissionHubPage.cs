using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class StationMissionHubPage : ContentPage
{
    private static readonly string[] Keys =
    [
        "station_find_counter",
        "station_ask_tickets",
        "station_pay",
        "station_find_platform",
        "station_find_wagon"
    ];

    private readonly Label _stars = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White
    };

    private readonly Label _instruction = new()
    {
        Text = "De missie begint...",
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#17324D"),
        HorizontalTextAlignment = TextAlignment.Center
    };

    private int _launchedMission = -1;

    public StationMissionHubPage()
    {
        Title = "Het station";
        BackgroundColor = Color.FromArgb("#172554");
        GameUi.AddHomeButton(this);

        var playground = new AbsoluteLayout();
        var scene = new Image { Source = "station_concourse.jpg", Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(scene, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(scene, AbsoluteLayoutFlags.All);
        playground.Add(scene);

        var shade = new BoxView { Color = Color.FromArgb("#2210203A") };
        AbsoluteLayout.SetLayoutBounds(shade, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(shade, AbsoluteLayoutFlags.All);
        playground.Add(shade);

        var characters = GameUi.OfficialCharacters(300);
        AbsoluteLayout.SetLayoutBounds(characters, new Rect(.5, .88, 300, 330));
        AbsoluteLayout.SetLayoutFlags(characters, AbsoluteLayoutFlags.PositionProportional);
        playground.Add(characters);

        var hud = new Border
        {
            Padding = new Thickness(13, 9),
            Margin = 12,
            BackgroundColor = Color.FromArgb("#D917324D"),
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
                        Text = "🚉  STATIONSAVONTUUR",
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

        var panel = new Border
        {
            Padding = new Thickness(14, 12),
            Margin = new Thickness(18, 0, 18, 20),
            BackgroundColor = Color.FromArgb("#EFFFFFFF"),
            Stroke = Color.FromArgb("#F59E0B"),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 24 },
            Content = _instruction
        };
        AbsoluteLayout.SetLayoutBounds(panel, new Rect(0, 1, 1, 74));
        AbsoluteLayout.SetLayoutFlags(panel,
            AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
        playground.Add(panel);

        Content = playground;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_launchedMission >= 0)
        {
            var completed = Preferences.Default.Get(Keys[_launchedMission], false);
            _launchedMission = -1;

            if (!completed)
            {
                _instruction.Text = "↩️ De missie is gestopt.";
                return;
            }
        }

        await OpenNextMissionAsync();
    }

    private async Task OpenNextMissionAsync()
    {
        var nextIndex = Array.FindIndex(Keys,
            key => !Preferences.Default.Get(key, false));

        _stars.Text = $"⭐ {Keys.Count(key => Preferences.Default.Get(key, false))}/5";

        if (nextIndex < 0)
        {
            Preferences.Default.Set("adventure_stage",
                Math.Max(3, Preferences.Default.Get("adventure_stage", 0)));
            _instruction.Text = "☀️ Goed gedaan! Ga verder naar de trein.";
            await SpeakDutchAsync("Het station is voltooid. Ga verder naar de trein.");
            await Task.Delay(350);
            await Navigation.PopAsync();
            return;
        }

        _instruction.Text = "🎯 De volgende vraag begint...";
        _launchedMission = nextIndex;
        await Task.Delay(250);
        await Navigation.PushAsync(new StationMiniMissionPage(nextIndex));
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var dutch = locales.FirstOrDefault(x =>
                x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text,
                new SpeechOptions { Locale = dutch });
        }
        catch { }
    }
}
