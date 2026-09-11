namespace ZaydRazanChallenge;

public sealed class StationMissionHubPage : ContentPage
{
    private readonly VerticalStackLayout _missions = new() { Spacing = 13 };
    private static readonly string[] Keys =
    [
        "station_find_counter", "station_ask_tickets", "station_pay",
        "station_find_platform", "station_find_wagon"
    ];

    public StationMissionHubPage()
    {
        Title = "Hoofdstuk · Het station";
        BackgroundColor = Color.FromArgb("#EFF6FF");
        GameUi.AddHomeButton(this);
        Content = new ScrollView { Content = new VerticalStackLayout
        {
            Padding = 18, Spacing = 13, Children =
            {
                new Label { Text = "🚉 Het station · La gare", FontSize = 31, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D") },
                new Label { Text = "Zayd en Razan zoeken zelf de weg naar de trein. Elke missie laat een deel van de wolken verdwijnen.", FontSize = 17, LineHeight = 1.25 },
                _missions
            }
        }};
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _missions.Clear();
        Add(0, "🔎 Het loket vinden", "Bekijk de borden en zoek het symbool van de tickets.");
        Add(1, "🎫 Tickets vragen", "Kies de juiste Franse zin.");
        Add(2, "💳 De tickets betalen", "Kies het juiste betaalmiddel.");
        Add(3, "📺 Het perron vinden", "Lees het bord en zoek perron 3.");
        Add(4, "🚄 De wagon vinden", "Stap in wagon 7.");

        if (Keys.All(k => Preferences.Default.Get(k, false)))
        {
            if (Preferences.Default.Get("adventure_stage", 0) < 3)
                Preferences.Default.Set("adventure_stage", 3);
            var done = new Button { Text = "☀️ Station voltooid · Terug naar de kaart", BackgroundColor = Color.FromArgb("#16A34A"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HeightRequest = 58 };
            done.Clicked += async (_, _) => await Navigation.PopAsync();
            _missions.Add(done);
        }
    }

    private void Add(int index, string title, string description)
    {
        var complete = Preferences.Default.Get(Keys[index], false);
        var unlocked = index == 0 || Preferences.Default.Get(Keys[index - 1], false);
        var button = new Button
        {
            Text = complete ? "✓ Opnieuw" : unlocked ? "Spelen" : "🔒",
            IsEnabled = unlocked, WidthRequest = 105, TextColor = Colors.White,
            BackgroundColor = complete ? Color.FromArgb("#16A34A") : unlocked ? Color.FromArgb("#2563EB") : Color.FromArgb("#94A3B8"),
            FontAttributes = FontAttributes.Bold
        };
        button.Clicked += async (_, _) => await Navigation.PushAsync(new StationMiniMissionPage(index));
        var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 10 };
        grid.Add(new VerticalStackLayout { Spacing = 4, Children =
        {
            new Label { Text = $"MISSION {index + 1}", FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#2563EB") },
            new Label { Text = title, FontSize = 20, FontAttributes = FontAttributes.Bold },
            new Label { Text = description, FontSize = 14, TextColor = Color.FromArgb("#475569") }
        }});
        grid.Add(button, 1);
        _missions.Add(new Border
        {
            BackgroundColor = Colors.White, Stroke = complete ? Color.FromArgb("#22C55E") : Color.FromArgb("#93C5FD"),
            StrokeThickness = 2, Padding = 15,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 }, Content = grid
        });
    }
}
