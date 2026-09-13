using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class AdventureMissionHubPage : ContentPage
{
    private static readonly string[] Keys =
    [
        "home_pack_zayd",
        "home_pack_razan",
        "home_documents"
    ];

    private readonly Label _speech = new()
    {
        Text = "De koffers worden klaargemaakt...",
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
        Text = "▶ Verder spelen",
        IsVisible = false,
        BackgroundColor = Color.FromArgb("#F59E0B"),
        TextColor = Colors.White,
        FontAttributes = FontAttributes.Bold,
        CornerRadius = 20
    };

    // PushAsync returns when a mission appears, not when it is completed.
    // OnAppearing is the only place that starts the next mission after a return.
    private int _launchedMission = -1;
    private bool _navigating;

    public AdventureMissionHubPage()
    {
        Title = "Thuis";
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
                        Text = "🏠  THUISAVONTUUR",
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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_navigating) return;

        if (_launchedMission >= 0)
        {
            var completed = Preferences.Default.Get(Keys[_launchedMission], false);
            _launchedMission = -1;
            if (!completed)
            {
                _speech.Text = "↩️ Je kunt verder spelen wanneer je wilt.";
                _resume.IsVisible = true;
                return;
            }
        }

        await OpenNextMissionAsync();
    }

    private async Task OpenNextMissionAsync()
    {
        if (_navigating) return;
        _navigating = true;
        _resume.IsVisible = false;

        try
        {
            var nextIndex = Array.FindIndex(Keys,
                key => !Preferences.Default.Get(key, false));
            _stars.Text = $"⭐ {Keys.Count(key => Preferences.Default.Get(key, false))}/3";

            if (nextIndex < 0)
            {
                Preferences.Default.Set("adventure_stage",
                    Math.Max(1, Preferences.Default.Get("adventure_stage", 0)));
                _speech.Text = "☀️ Goed gedaan! Op naar het station.";
                // Replace the hub in the navigation stack so Back returns to the map.
                Navigation.InsertPageBefore(new AdventureScenePage(1), this);
                await Navigation.PopAsync();
                return;
            }

            _speech.Text = "🎯 De volgende opdracht begint...";
            _launchedMission = nextIndex;
            await Task.Delay(250);
            var mission = nextIndex switch
            {
                0 => PackingMissionPage.ForZayd(),
                1 => PackingMissionPage.ForRazan(),
                _ => PackingMissionPage.ForDocuments()
            };
            await Navigation.PushAsync(mission);
        }
        finally
        {
            _navigating = false;
        }
    }
}
