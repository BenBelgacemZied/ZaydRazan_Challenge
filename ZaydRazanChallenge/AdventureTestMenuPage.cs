namespace ZaydRazanChallenge;

public sealed class AdventureTestMenuPage : ContentPage
{
    private bool _opening;

    public AdventureTestMenuPage()
    {
        Title = "Testmodus";
        BackgroundColor = Color.FromArgb("#EFF6FF");
        GameUi.AddHomeButton(this);
        AdventureSave.BeginTest();

        var items = new VerticalStackLayout { Padding = 18, Spacing = 10 };
        items.Add(new Label
        {
            Text = "🧪 Kies een scène om te testen",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#17324D")
        });
        items.Add(new Label
        {
            Text = "Test zoveel je wilt. Je sterren en voortgang in Speelmodus veranderen niet.",
            FontSize = 15,
            TextColor = Color.FromArgb("#334155")
        });

        AddSection(items, "🏠 Thuis");
        AddOption(items, "Het hele huis (3 opdrachten)", () => new AdventureMissionHubPage());
        AddOption(items, "Zayds koffer", PackingMissionPage.ForZayd);
        AddOption(items, "Razans koffer", PackingMissionPage.ForRazan);
        AddOption(items, "Reispapieren", PackingMissionPage.ForDocuments);

        AddSection(items, "🚉 In het station");
        AddOption(items, "Het hele station (5 vragen)", () => new StationMissionHubPage());
        var station = new[] { "Zoek het loket", "Vraag twee tickets", "Betaal de tickets", "Zoek spoor 3", "Zoek wagon 7" };
        for (var i = 0; i < station.Length; i++)
        {
            var index = i;
            AddOption(items, station[i], () => new StationMiniMissionPage(index));
        }

        AddSection(items, "🚄 In de trein");
        AddOption(items, "De hele trein (5 voorwerpen)", () => new TrainMissionHubPage());
        var train = new[] { "la porte", "le siège", "le porte-bagages", "la fenêtre", "la table" };
        for (var i = 0; i < train.Length; i++)
        {
            var index = i;
            AddOption(items, train[i], () => new TrainMissionHubPage(index));
        }

        for (var i = 0; i < ParisTreasureCatalog.Count; i++)
        {
            if (i % 10 == 0) AddSection(items, $"☁️ Parijs · ontdekking {i + 1}–{Math.Min(i + 10, ParisTreasureCatalog.Count)}");
            var stage = i + ParisTreasureCatalog.FirstStage;
            AddOption(items, $"{i + 1:00} · {ParisTreasureCatalog.Quests[i].Title}", () => new ParisTreasurePage(stage));
        }

        Content = new ScrollView { Content = items };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _opening = false;
    }

    private void AddOption(VerticalStackLayout items, string title, Func<Page> create)
    {
        var button = new Button
        {
            Text = "▶ " + title,
            BackgroundColor = Color.FromArgb("#2563EB"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 16,
            HeightRequest = 56
        };
        button.Clicked += async (_, _) =>
        {
            if (_opening) return;
            _opening = true;
            try
            {
                AdventureSave.ResetTest();
                await Navigation.PushAsync(create());
            }
            catch
            {
                _opening = false;
                throw;
            }
        };
        items.Add(button);
    }

    private static void AddSection(VerticalStackLayout items, string title) =>
        items.Add(new Label
        {
            Text = title,
            FontSize = 19,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#17324D"),
            Margin = new Thickness(0, 12, 0, 0)
        });
}
