using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class BrusselsAdventurePage : ContentPage
{
    private sealed record Mission(
        string Place,
        string Story,
        string Question,
        string FrenchPhrase,
        string[] Answers,
        int CorrectIndex,
        string Success);

    private static readonly Mission[] Missions =
    [
        new("Bruxelles-Midi", "Zayd et Razan arrivent à Bruxelles avec leur famille.",
            "Hoe zeg je « het station » in het Frans?", "la gare",
            ["la gare", "la rue", "le train"], 0, "Nous sommes à Bruxelles-Midi !"),
        new("Le guichet", "Papa veut acheter les billets pour la suite du voyage.",
            "Welke Franse zin vraag je aan het loket?", "Deux billets, s'il vous plaît",
            ["Merci beaucoup", "Deux billets, s'il vous plaît", "Au revoir"], 1, "Deux billets, s'il vous plaît !"),
        new("Le quai", "La famille cherche le bon quai à la gare.",
            "Hoe zeg je « het perron » in het Frans?", "le quai",
            ["le quai", "la valise", "la carte"], 0, "Le quai est ici !"),
        new("Le train", "Ils montent dans le train et cherchent leurs places.",
            "Welke Franse zin betekent « wij reizen met de trein »?", "Nous voyageons en train",
            ["Nous mangeons une gaufre", "Nous voyageons en train", "Nous visitons Paris"], 1, "Nous voyageons en train !"),
        new("Le tram", "Dans la ville, Zayd et Razan prennent le tram.",
            "Hoe zeg je « de tram » in het Frans?", "le tram",
            ["le métro", "le tram", "le bateau"], 1, "Prenons le tram !"),
        new("Manneken Pis", "Zayd et Razan découvrent la petite statue célèbre de Bruxelles.",
            "Hoe zeg je « het standbeeld » in het Frans?", "la statue",
            ["la statue", "la fenêtre", "la place"], 0, "Voici la statue !"),
        new("La gaufre", "De kinderen ruiken heerlijke Belgische wafels.",
            "Welke Franse zin vraag je in de winkel?", "Une gaufre, s'il vous plaît",
            ["Une gaufre, s'il vous plaît", "Un billet, s'il vous plaît", "Une carte, s'il vous plaît"], 0, "Une gaufre, s'il vous plaît !"),
        new("Mini-Europe", "Ils visitent Mini-Europe et observent les monuments miniatures.",
            "Hoe zeg je « klein » in het Frans?", "petit",
            ["grand", "petit", "haut"], 1, "Les monuments sont petits !"),
        new("L'Atomium", "De kinderen kijken naar de grote bollen van het Atomium.",
            "Welke Franse zin betekent « het Atomium is groot »?", "L'Atomium est grand",
            ["L'Atomium est grand", "L'Atomium est petit", "L'Atomium est fermé"], 0, "L'Atomium est grand !"),
        new("Le métro Heizel", "La famille prend le métro près de l'Atomium.",
            "Hoe zeg je « de metro » in het Frans?", "le métro",
            ["le métro", "le vélo", "le taxi"], 0, "Prenons le métro !"),
        new("La Grand-Place", "Zayd et Razan arrivent sur la magnifique Grand-Place.",
            "Hoe zeg je « het plein » in het Frans?", "la place",
            ["la gare", "la place", "la maison"], 1, "La Grand-Place est magnifique !"),
        new("Mission finale", "La journée à Bruxelles se termine. Zayd et Razan racontent leur voyage.",
            "Welke Franse zin betekent « tot ziens, Brussel »?", "Au revoir, Bruxelles",
            ["Bonjour, Paris", "Au revoir, Bruxelles", "Merci, Londres"], 1, "Au revoir, Bruxelles !")
    ];

    private readonly Grid _root = new();
    private readonly Label _counter = new();
    private readonly Label _place = new();
    private readonly Label _story = new();
    private readonly Label _question = new();
    private readonly Label _feedback = new();
    private readonly VerticalStackLayout _answers = new() { Spacing = 10 };
    private readonly Label _successOverlay = new()
    {
        Text = "🎈 Bravo !",
        FontSize = 42,
        FontAttributes = FontAttributes.Bold,
        TextColor = Colors.White,
        BackgroundColor = Color.FromArgb("#DD16A34A"),
        HorizontalTextAlignment = TextAlignment.Center,
        VerticalTextAlignment = TextAlignment.Center,
        IsVisible = false,
        Opacity = 0,
        Scale = .4
    };
    private int _missionIndex;
    private bool _busy;

    public BrusselsAdventurePage()
    {
        Title = "Avontuur in Brussel";
        BackgroundColor = Color.FromArgb("#E0F2FE");
        GameUi.AddHomeButton(this);

        var background = new Image
        {
            Source = "adventure_map.jpg",
            Aspect = Aspect.AspectFill,
            InputTransparent = true
        };
        Grid.SetRowSpan(background, 2);
        _root.Add(background);

        var header = new Border
        {
            Margin = 12,
            Padding = new Thickness(14, 10),
            BackgroundColor = Color.FromArgb("#CC17324D"),
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 22 },
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
                        Text = "🇧🇪 Bruxelles",
                        FontSize = 22,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White
                    },
                    _counter
                }
            }
        };
        Grid.SetColumn(_counter, 1);

        var panel = new Border
        {
            Margin = new Thickness(14, 0, 14, 18),
            Padding = 18,
            BackgroundColor = Color.FromArgb("#F8FFFFFF"),
            Stroke = Color.FromArgb("#F59E0B"),
            StrokeThickness = 2,
            StrokeShape = new RoundRectangle { CornerRadius = 24 },
            VerticalOptions = LayoutOptions.End,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    _place, _story, _question, _answers, _feedback
                }
            }
        };

        _root.Add(header);
        _root.Add(panel);
        _root.Add(_successOverlay);
        Grid.SetRow(_successOverlay, 0);
        Grid.SetRowSpan(_successOverlay, 2);
        _successOverlay.InputTransparent = true;
        Content = new ScrollView { Content = _root };

        _missionIndex = Math.Clamp(AdventureSave.Get("brussels_stage", 0), 0, Missions.Length);
        RenderMission();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_missionIndex < Missions.Length)
            await ReadQuestionAsync();
    }

    private void RenderMission()
    {
        _answers.Clear();

        if (_missionIndex >= Missions.Length)
        {
            _counter.Text = "12/12";
            _place.Text = "🎉 Bruxelles terminé !";
            _story.Text = "Zayd et Razan ont découvert Bruxelles et appris de nouveaux mots français.";
            _question.Text = "Bravo pour cette nouvelle aventure !";
            _feedback.Text = "Tu peux réinitialiser l'application pour recommencer.";
            return;
        }

        var mission = Missions[_missionIndex];
        _counter.Text = $"{_missionIndex + 1}/{Missions.Length}";
        _place.Text = $"📍 {mission.Place}";
        _story.Text = mission.Story;
        _question.Text = mission.Question;
        _feedback.Text = "Écoute la question avant de choisir.";
        _feedback.TextColor = Color.FromArgb("#64748B");

        for (var i = 0; i < mission.Answers.Length; i++)
        {
            var index = i;
            var answer = new Button
            {
                Text = mission.Answers[i],
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#2563EB"),
                TextColor = Colors.White,
                CornerRadius = 18,
                MinimumHeightRequest = 52,
                IsEnabled = false
            };
            answer.Clicked += async (_, _) => await AnswerAsync(index, answer);
            _answers.Add(answer);
        }
    }

    private async Task ReadQuestionAsync()
    {
        if (_missionIndex >= Missions.Length) return;
        _feedback.Text = "🔊 Écoute la question...";
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = false;

        await SpeakDutchAsync(_question.Text);

        if (_busy || _missionIndex >= Missions.Length) return;
        _feedback.Text = "Choisis la bonne réponse en français.";
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = true;
    }

    private async Task AnswerAsync(int index, Button selected)
    {
        if (_busy || _missionIndex >= Missions.Length) return;
        _busy = true;
        foreach (var child in _answers.Children)
            if (child is Button button) button.IsEnabled = false;

        var mission = Missions[_missionIndex];
        await SpeakFrenchAsync(mission.Answers[index]);

        if (index != mission.CorrectIndex)
        {
            selected.BackgroundColor = Color.FromArgb("#DC2626");
            _feedback.Text = "Ce n'est pas la bonne réponse. Réécoute et essaie encore.";
            await GameFeedback.FailureAsync();
            await Task.Delay(500);
            selected.BackgroundColor = Color.FromArgb("#2563EB");
            _busy = false;
            await ReadQuestionAsync();
            return;
        }

        selected.BackgroundColor = Color.FromArgb("#16A34A");
        _feedback.Text = $"Bravo ! {mission.FrenchPhrase}";
        AdventureSave.Set("brussels_stage", _missionIndex + 1);
        AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
        await GameFeedback.SuccessAsync();
        await ShowSuccessAsync(mission.Success);
        _missionIndex++;
        RenderMission();
        _busy = false;

        if (_missionIndex < Missions.Length)
            await ReadQuestionAsync();
    }

    private async Task ShowSuccessAsync(string message)
    {
        _successOverlay.Text = $"🎈 Bravo !\\n{message}";
        _successOverlay.IsVisible = true;
        await _successOverlay.ScaleTo(1, 220, Easing.CubicOut);
        await _successOverlay.FadeTo(1, 180);
        await Task.Delay(500);
        await _successOverlay.ScaleTo(1.12, 140, Easing.CubicInOut);
        await _successOverlay.FadeTo(0, 220);
        _successOverlay.IsVisible = false;
        _successOverlay.Scale = .4;
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var locale = locales.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            if (locale is not null)
                await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale });
        }
        catch { }
    }

    private static async Task SpeakFrenchAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var locale = locales.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            if (locale is not null)
                await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = locale });
        }
        catch { }
    }
}
