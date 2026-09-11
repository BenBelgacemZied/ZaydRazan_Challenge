namespace ZaydRazanChallenge;

public sealed class AdventureMissionHubPage : ContentPage
{
    private readonly VerticalStackLayout _missions = new() { Spacing = 14 };

    public AdventureMissionHubPage()
    {
        Title = "Chapitre 1 · La maison";
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
                    new Label { Text = "🏠 La maison", FontSize = 31, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D") },
                    new Label { Text = "Zayd et Razan préparent eux-mêmes leur voyage. Termine chaque mission pour faire disparaître les nuages et ouvrir la route vers la gare.", FontSize = 17, LineHeight = 1.25 },
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

        AddMission("1", "🧳 Valise de Zayd", "Choisir 6 objets utiles et apprendre leur nom en français.", true, zaydDone,
            () => Navigation.PushAsync(PackingMissionPage.ForZayd()));
        AddMission("2", "🧳 Valise de Razan", "Préparer sa valise sans emporter les objets intrus.", zaydDone, razanDone,
            () => Navigation.PushAsync(PackingMissionPage.ForRazan()));
        AddMission("3", "📘 Documents de voyage", "Retrouver passeports, billets et plan de Paris.", razanDone, documentsDone,
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
                    new Label { Text = "☀️ Chapitre terminé !", FontSize = 23, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#166534") },
                    new Label { Text = "Les valises sont fermées. La route vers la gare est maintenant ouverte.", FontSize = 16 },
                    CreateGoButton()
                }}
            });
        }
    }

    private Button CreateGoButton()
    {
        var button = new Button { Text = "🗺️ Retourner à la carte", BackgroundColor = Color.FromArgb("#16A34A"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold };
        button.Clicked += async (_, _) => await Navigation.PopAsync();
        return button;
    }

    private void AddMission(string number, string title, string description, bool unlocked, bool completed, Func<Task> open)
    {
        var button = new Button
        {
            Text = completed ? "✓ Rejouer" : unlocked ? "Jouer" : "🔒 Verrouillée",
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
