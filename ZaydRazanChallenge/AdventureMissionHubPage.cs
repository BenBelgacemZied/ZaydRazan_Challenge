namespace ZaydRazanChallenge;

public sealed class AdventureMissionHubPage : ContentPage
{
    private readonly VerticalStackLayout _missions = new() { Spacing = 14 };

    public AdventureMissionHubPage()
    {
        Title = "Hoofdstuk 1 · Thuis";
        BackgroundColor = Color.FromArgb("#FFF7ED");
        GameUi.AddHomeButton(this);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 18,
                Spacing = 14,
                Children =
                {
                    new Label { Text = "🏠 Thuis · À la maison", FontSize = 31, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D") },
                    new Label { Text = "Zayd en Razan bereiden zelf hun reis voor. Voltooi elke missie om de wolken te laten verdwijnen en de weg naar het station te openen.", FontSize = 17, LineHeight = 1.25 },
                    new ProgressBar { ProgressColor = Color.FromArgb("#F59E0B"), BackgroundColor = Color.FromArgb("#FED7AA"), HeightRequest = 10 },
                    _missions
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RenderMissions();
    }

    private void RenderMissions()
    {
        _missions.Clear();
        var zaydDone = Preferences.Default.Get("home_pack_zayd", false);
        var razanDone = Preferences.Default.Get("home_pack_razan", false);
        var documentsDone = Preferences.Default.Get("home_documents", false);

        AddMission("1", "🧳 De koffer van Zayd", "Kies 6 nuttige voorwerpen en leer hun Franse naam.", true, zaydDone,
            () => Navigation.PushAsync(PackingMissionPage.ForZayd()));
        AddMission("2", "🧳 De koffer van Razan", "Maak haar koffer klaar zonder de verkeerde voorwerpen mee te nemen.", zaydDone, razanDone,
            () => Navigation.PushAsync(PackingMissionPage.ForRazan()));
        AddMission("3", "📘 Reisdocumenten", "Zoek de paspoorten, tickets en de kaart van Parijs.", razanDone, documentsDone,
            () => Navigation.PushAsync(PackingMissionPage.ForDocuments()));

        if (zaydDone && razanDone && documentsDone)
        {
            if (Preferences.Default.Get("adventure_stage", 0) < 1)
                Preferences.Default.Set("adventure_stage", 1);
            _missions.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#DCFCE7"), Stroke = Color.FromArgb("#22C55E"), StrokeThickness = 2,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 }, Padding = 18,
                Content = new VerticalStackLayout { Spacing = 8, Children =
                {
                    new Label { Text = "☀️ Hoofdstuk voltooid!", FontSize = 23, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#166534") },
                    new Label { Text = "De koffers zijn dicht. De weg naar het station is nu open.", FontSize = 16 },
                    CreateGoButton()
                }}
            });
        }
    }

    private Button CreateGoButton()
    {
        var button = new Button { Text = "🗺️ Terug naar de kaart", BackgroundColor = Color.FromArgb("#16A34A"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold };
        button.Clicked += async (_, _) => await Navigation.PopAsync();
        return button;
    }

    private void AddMission(string number, string title, string description, bool unlocked, bool completed, Func<Task> open)
    {
        var button = new Button
        {
            Text = completed ? "✓ Opnieuw" : unlocked ? "Spelen" : "🔒 Vergrendeld",
            IsEnabled = unlocked,
            BackgroundColor = completed ? Color.FromArgb("#16A34A") : unlocked ? Color.FromArgb("#2563EB") : Color.FromArgb("#94A3B8"),
            TextColor = Colors.White, FontAttributes = FontAttributes.Bold, WidthRequest = 112
        };
        button.Clicked += async (_, _) => await open();
        var grid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 10 };
        grid.Add(new VerticalStackLayout { Spacing = 5, Children =
        {
            new Label { Text = $"MISSION {number}", FontSize = 13, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#EA580C") },
            new Label { Text = title, FontSize = 21, FontAttributes = FontAttributes.Bold },
            new Label { Text = description, FontSize = 15, TextColor = Color.FromArgb("#475569") }
        }});
        grid.Add(button, 1);
        _missions.Add(new Border { BackgroundColor = Colors.White, Stroke = completed ? Color.FromArgb("#22C55E") : Color.FromArgb("#FDBA74"), StrokeThickness = 2, StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 }, Padding = 16, Content = grid });
    }
}
