namespace ZaydRazanChallenge.Features.Adventure.Pages;

public sealed class AdventureTestMenuPage : ContentPage
{
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private readonly IDestinationContent _destination;
    private readonly IGamePageFactory _pages;
    private bool IsEnglish => _destination.LanguagePair.SourceLanguage.Value == "en";
    private bool _opening;

    public AdventureTestMenuPage(IDestinationContent destination, IGamePageFactory pages, IGameProgressStore progress, ISpeechService speech)
    {
        _speechService = speech;
        _gameProgress = progress;
        _destination = destination;
        _pages = pages;
        Title = IsEnglish ? "Test mode" : "Testmodus";
        BackgroundColor = Color.FromArgb("#EFF6FF");
        GameUi.AddHomeButton(this);
        _gameProgress.BeginTest();

        var items = new VerticalStackLayout { Padding = 18, Spacing = 10 };
        items.Add(new Label
        {
            Text = IsEnglish ? "🧪 Choose a scene to test" : "🧪 Kies een scène om te testen",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#17324D")
        });
        items.Add(new Label
        {
            Text = IsEnglish ? "Test as much as you like. Your stars and progress in game mode will not change." : "Test zoveel je wilt. Je sterren en voortgang in Speelmodus veranderen niet.",
            FontSize = 15,
            TextColor = Color.FromArgb("#334155")
        });

        AddSection(items, IsEnglish ? "🏠 Home" : "🏠 Thuis");
        AddOption(items, IsEnglish ? "The whole home (3 missions)" : "Het hele huis (3 opdrachten)", () => new AdventureMissionHubPage(_gameProgress, _speechService, _destination));
        AddOption(items, IsEnglish ? "Zayd’s suitcase" : "Zayds koffer", () => PackingMissionPage.ForZayd(_gameProgress, _speechService, _destination));
        AddOption(items, IsEnglish ? "Razan’s suitcase" : "Razans koffer", () => PackingMissionPage.ForRazan(_gameProgress, _speechService, _destination));
        AddOption(items, IsEnglish ? "Travel documents" : "Reispapieren", () => PackingMissionPage.ForDocuments(_gameProgress, _speechService, _destination));

        AddSection(items, IsEnglish ? "🚉 At the station" : "🚉 In het station");
        AddOption(items, IsEnglish ? "The whole station (5 questions)" : "Het hele station (5 vragen)", () => new StationMissionHubPage(_gameProgress, _speechService, _destination));
        var station = IsEnglish ? new[] { "Find the ticket counter", "Ask for two tickets", "Pay for the tickets", "Find platform 3", "Find carriage 7" } : new[] { "Zoek het loket", "Vraag twee tickets", "Betaal de tickets", "Zoek spoor 3", "Zoek wagon 7" };
        for (var i = 0; i < station.Length; i++)
        {
            var index = i;
            AddOption(items, station[i], () => new StationMiniMissionPage(index, _gameProgress, _speechService, _destination));
        }

        AddSection(items, IsEnglish ? "🚄 On the train" : "🚄 In de trein");
        AddOption(items, IsEnglish ? "The whole train (5 items)" : "De hele trein (5 voorwerpen)", () => new TrainMissionHubPage(_gameProgress, _speechService, _destination));
        var train = new[] { "la porte", "le siège", "le porte-bagages", "la fenêtre", "la table" };
        for (var i = 0; i < train.Length; i++)
        {
            var index = i;
            AddOption(items, train[i], () => new TrainMissionHubPage(_gameProgress, _speechService, _destination, index));
        }

        for (var i = 0; i < _destination.Count; i++)
        {
            if (i % 10 == 0) AddSection(items, IsEnglish ? $"☁️ {_destination.Profile.DisplayName} · discoveries {i + 1}–{Math.Min(i + 10, _destination.Count)}" : $"☁️ {_destination.Profile.DisplayName} · ontdekking {i + 1}–{Math.Min(i + 10, _destination.Count)}");
            var stage = i + _destination.FirstStage;
            AddOption(items, $"{i + 1:00} · {_destination.Quests[i].Title}", () => _pages.CreateParisTreasurePage(stage));
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
                _gameProgress.ResetTest();
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
