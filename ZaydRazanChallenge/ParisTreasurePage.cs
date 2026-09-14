using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class ParisTreasurePage : ContentPage
{
    private sealed record Clue(string DutchSentence, string Highlight, string FrenchPrompt, string Word, string[] Choices);
    private sealed record Treasure(string Title, string Emoji, string Target, string Intro, Clue[] Clues, string[] Objects);

    private static readonly Treasure[] Quests =
    [
        new("De stadskaart", "🗺️", "la carte", "Zayd en Razan zijn in Parijs. Zoek eerst drie aanwijzingen voor hun kaart.",
            [
                new("Zayd zoekt de kaart.", "kaart", "Zayd cherche ...", "la carte", ["la carte", "le pain", "la porte"]),
                new("De straat leidt naar de toren.", "straat", "La ... mène à la tour.", "rue", ["rue", "gare", "table"]),
                new("Razan ziet de stad.", "stad", "Razan voit la ...", "ville", ["ville", "valise", "porte"])
            ], ["🗺️|la carte", "🥖|la baguette", "🎟️|le billet"]),
        new("La tour Eiffel", "🗼", "la tour Eiffel", "Razan zoekt een hoge toren. Ontdek drie aanwijzingen.",
            [
                new("Het is van ijzer gemaakt.", "ijzer", "C'est fait en ...", "fer", ["bois", "fer", "diamant"]),
                new("De toren is heel hoog.", "hoog", "La tour est très ...", "haute", ["haute", "petite", "rouge"]),
                new("Je ziet de top van de toren.", "top", "Tu vois le ... de la tour.", "sommet", ["sommet", "pain", "quai"])
            ], ["🗼|la tour Eiffel", "🏛️|l'Arc de Triomphe", "🥖|la baguette"]),
        new("Le musée du Louvre", "🖼️", "le Louvre", "Zayd zoekt een beroemd museum. Luister naar de aanwijzingen.",
            [
                new("In het museum zie je kunst.", "museum", "On voit de l'art au ...", "musée", ["musée", "train", "parc"]),
                new("Hier hangt de Mona Lisa.", "Mona Lisa", "En français, c'est ...", "la Joconde", ["la fenêtre", "la Joconde", "la valise"]),
                new("Zayd zoekt een schilderij.", "schilderij", "Zayd cherche un ...", "tableau", ["billet", "tableau", "train"])
            ], ["🖼️|le Louvre", "🗼|la tour Eiffel", "🥖|la baguette"]),
        new("L'Arc de Triomphe", "🏛️", "l'Arc de Triomphe", "Ze zoeken een grote boog in Parijs.",
            [
                new("De grote boog staat aan een plein.", "plein", "Le grand arc est sur une ...", "place", ["place", "table", "porte"]),
                new("De boog is een monument.", "monument", "L'arc est un ...", "monument", ["musée", "monument", "train"]),
                new("Je ziet een grote boog.", "boog", "Tu vois un grand ...", "arc", ["arc", "quai", "pain"])
            ], ["🏛️|l'Arc de Triomphe", "🖼️|le Louvre", "🗺️|la carte"]),
        new("La baguette", "🥖", "la baguette", "Zayd ruikt vers brood. Waar is de baguette?",
            [
                new("De bakker maakt vers brood.", "bakker", "Le ... prépare du pain.", "boulanger", ["boulanger", "musée", "quai"]),
                new("In de bakkerij koop je brood.", "bakkerij", "On achète du pain à la ...", "boulangerie", ["gare", "boulangerie", "tour"]),
                new("Een baguette is lang brood.", "brood", "La baguette est du ...", "pain", ["train", "siège", "pain"])
            ], ["🗺️|la carte", "🥖|la baguette", "🖼️|le Louvre"])
    ];

    private static readonly Rect[] ObjectPlaces = [new(.14, .30, 130, 95), new(.82, .50, 130, 95), new(.25, .78, 130, 95)];

    private readonly int _stage;
    private readonly Treasure _quest;
    private readonly AbsoluteLayout _scene = new();
    private readonly Label _status = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _instruction = new() { FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _dutch = new() { FontSize = 17, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.White };
    private readonly Label _french = new() { FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#FDE68A") };
    private readonly Label _feedback = new() { FontSize = 13, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Grid _choices = new() { ColumnSpacing = 5 };
    private readonly BoxView _fog = new() { Color = Color.FromArgb("#9AA8B8"), Opacity = .70, InputTransparent = true };
    private readonly Label[] _clouds = new Label[3];
    private readonly Button _letter = new() { Text = "✉️ Open de brief", FontSize = 16, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White, CornerRadius = 16, BorderColor = Colors.White, BorderWidth = 2, AutomationId = "paris-open-letter" };
    private readonly VerticalStackLayout _letterPanel = new() { Spacing = 3, Padding = new Thickness(0, 2) };
    private int _clueIndex;
    private bool _letterOpen;
    private bool _busy;
    private bool _finished;

    public ParisTreasurePage(int stage)
    {
        if (stage < 4 || stage > 8) throw new ArgumentOutOfRangeException(nameof(stage));
        _stage = stage;
        _quest = Quests[stage - 4];
        Title = _quest.Title;
        BackgroundColor = Color.FromArgb("#17324D");
        GameUi.AddHomeButton(this);

        // Each hunt has its own Parisian setting instead of reusing the adventure map.
        // The station illustration has different-looking children in it. Use
        // the empty concourse and never draw another child over any scene.
        var scenes = new[] { "station_concourse.jpg", "eiffel_puzzle.jpg", "louvre_puzzle.jpg", "arc_puzzle.jpg", "baguette_puzzle.jpg" };
        var image = new Image { Source = scenes[stage - 4], Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(image, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
        _scene.Add(image);

        AbsoluteLayout.SetLayoutBounds(_fog, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(_fog, AbsoluteLayoutFlags.All);
        _scene.Add(_fog);
        var cloudPositions = new[] { new Rect(.16, .18, 120, 90), new Rect(.79, .40, 120, 90), new Rect(.35, .71, 120, 90) };
        for (var i = 0; i < _clouds.Length; i++)
        {
            var cloud = new Label { Text = "☁️", FontSize = 88, HorizontalTextAlignment = TextAlignment.Center, InputTransparent = true };
            _clouds[i] = cloud;
            AbsoluteLayout.SetLayoutBounds(cloud, cloudPositions[i]);
            AbsoluteLayout.SetLayoutFlags(cloud, AbsoluteLayoutFlags.PositionProportional);
            _scene.Add(cloud);
        }

        _letter.Clicked += async (_, _) => await OpenLetterAsync();
        AbsoluteLayout.SetLayoutBounds(_letter, new Rect(.54, .75, 158, 52));
        AbsoluteLayout.SetLayoutFlags(_letter, AbsoluteLayoutFlags.PositionProportional);
        _scene.Add(_letter);

        var hear = new Button { Text = "🔊", FontSize = 21, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White, CornerRadius = 24, WidthRequest = 46, HeightRequest = 46 };
        hear.Clicked += async (_, _) => await SpeakCurrentAsync();
        var top = new Grid { Padding = new Thickness(12, 3), VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.FromArgb("#C817324D"), ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        top.Add(_status);
        top.Add(hear, 1, 0);

        _choices.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _choices.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _choices.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _letterPanel.Children.Add(new Label { Text = (stage % 2 == 0 ? "Razan" : "Zayd") + " opent de brief", FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#FDE68A") });
        _letterPanel.Children.Add(_dutch);
        _letterPanel.Children.Add(_french);
        _letterPanel.Children.Add(_choices);
        _letterPanel.Children.Add(_feedback);
        _letterPanel.IsVisible = false;
        var bottom = new VerticalStackLayout { Padding = new Thickness(12, 7), Spacing = 2, VerticalOptions = LayoutOptions.End, BackgroundColor = Color.FromArgb("#E817324D"), Children = { _instruction, _letterPanel } };
        var layout = new Grid();
        layout.Add(_scene);
        layout.Add(top);
        layout.Add(bottom);
        Content = layout;
        _instruction.Text = "☁️ Open de brief en onthul de foto.";
        _status.Text = $"{_quest.Emoji} {_quest.Title}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SpeakCurrentAsync();
    }

    private void Render()
    {
        _status.Text = $"{_quest.Emoji} {_quest.Title}  ·  {_clueIndex}/3";
        _choices.Children.Clear();
        _feedback.Text = "";
        var clue = _quest.Clues[_clueIndex];
        var start = clue.DutchSentence.IndexOf(clue.Highlight, StringComparison.OrdinalIgnoreCase);
        if (start < 0) throw new InvalidOperationException($"Missing highlighted word: {clue.Highlight}");
        _dutch.FormattedText = new FormattedString
        {
            Spans =
            {
                new Span { Text = clue.DutchSentence[..start] },
                new Span { Text = clue.DutchSentence.Substring(start, clue.Highlight.Length), TextColor = Color.FromArgb("#FDE047"), FontAttributes = FontAttributes.Bold },
                new Span { Text = clue.DutchSentence[(start + clue.Highlight.Length)..] }
            }
        };
        _french.Text = "🇫🇷 " + clue.FrenchPrompt;
        foreach (var (answer, index) in clue.Choices.OrderBy(_ => Random.Shared.Next()).Select((value, index) => (value, index)))
        {
            var choice = answer;
            var button = new Button
            {
                Text = choice, FontSize = 14, FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#2563EB"), TextColor = Colors.White,
                CornerRadius = 12, HeightRequest = 42, Padding = new Thickness(3, 0),
                AutomationId = "paris-answer-" + choice
            };
            button.Clicked += async (_, _) => await AnswerAsync(choice, button);
            _choices.Add(button, index, 0);
        }
    }

    private async Task OpenLetterAsync()
    {
        if (_busy || _letterOpen) return;
        _busy = true;
        _letterOpen = true;
        _letter.Text = "📜";
        _letter.IsVisible = false;
        _letterPanel.IsVisible = true;
        _instruction.Text = "Kies het Franse woord.";
        Render();
        await SpeakCurrentAsync();
        _busy = false;
    }

    private async Task AnswerAsync(string answer, Button button)
    {
        if (_busy || !_letterOpen) return;
        _busy = true;
        var clue = _quest.Clues[_clueIndex];
        if (answer != clue.Word)
        {
            button.BackgroundColor = Color.FromArgb("#DC2626");
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            _feedback.TextColor = Color.FromArgb("#FCA5A5");
            _feedback.Text = "Probeer opnieuw. Luister naar het gekleurde woord.";
            await GameFeedback.FailureAsync();
            await SpeakDutchAsync("Probeer opnieuw.");
            button.BackgroundColor = Color.FromArgb("#2563EB");
            _busy = false;
            return;
        }

        foreach (var child in _choices.Children)
            if (child is Button choice) choice.IsEnabled = false;
        button.BackgroundColor = Color.FromArgb("#16A34A");
        _feedback.TextColor = Color.FromArgb("#86EFAC");
        _feedback.Text = $"Goed zo! {clue.Highlight} = {clue.Word}  ☀️";
        await GameFeedback.SuccessAsync();
        await SpeakFrenchAsync(clue.Word);
        await Task.WhenAll(_clouds[_clueIndex].FadeTo(0, 500),
            _fog.FadeTo(Math.Max(0, .70 - (_clueIndex + 1) * .24), 500));
        _clueIndex++;
        await Task.Delay(650);
        if (_clueIndex == _quest.Clues.Length)
        {
            _letterPanel.IsVisible = false;
            _instruction.Text = "🎯 De wolken zijn weg! Tik op de schat in het decor.";
            _status.Text = $"{_quest.Emoji} {_quest.Title}  ·  🎯";
            ShowObjects();
            await SpeakCurrentAsync();
        }
        else
        {
            Render();
            await SpeakCurrentAsync();
        }
        _busy = false;
    }

    private void ShowObjects()
    {
        foreach (var entry in _quest.Objects.Select((value, index) => (value, index)))
        {
            var parts = entry.value.Split('|');
            var target = parts[1];
            var button = new Button { Text = parts[0], FontSize = 32, FontAttributes = FontAttributes.Bold, BackgroundColor = Color.FromArgb("#EAFBF8"), TextColor = Color.FromArgb("#17324D"), BorderColor = Color.FromArgb("#F59E0B"), BorderWidth = 3, CornerRadius = 19, AutomationId = $"paris-object-{entry.index}" };
            button.Clicked += async (_, _) => await FindAsync(target, button);
            AbsoluteLayout.SetLayoutBounds(button, ObjectPlaces[entry.index]);
            AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
            _scene.Add(button);
        }
    }

    private async Task FindAsync(string name, Button button)
    {
        if (_busy) return;
        _busy = true;
        if (name != _quest.Target)
        {
            button.BackgroundColor = Color.FromArgb("#DC2626");
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            await GameFeedback.FailureAsync();
            await SpeakDutchAsync("Dat is het niet. Zoek verder.");
            button.BackgroundColor = Color.FromArgb("#EAFBF8");
            _busy = false;
            return;
        }

        _finished = true;
        button.BackgroundColor = Color.FromArgb("#16A34A");
        await GameFeedback.SuccessAsync();
        await SpeakFrenchAsync(name);
        AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
        if (!AdventureSave.IsTestMode)
            AdventureSave.Set("adventure_stage", Math.Max(_stage + 1, AdventureSave.Get("adventure_stage", 0)));
        _instruction.Text = $"⭐ Goed gevonden! {name}";
        await SpeakDutchAsync("Goed gevonden! Je hebt de schat ontdekt.");
        await Task.Delay(450);
        await Navigation.PopAsync();
    }

    private async Task SpeakCurrentAsync()
    {
        if (_finished) return;
        if (!_letterOpen)
            await SpeakDutchAsync(_quest.Intro + " Open de brief bij Zayd en Razan.");
        else if (_clueIndex < _quest.Clues.Length)
            await SpeakDutchAsync(_quest.Clues[_clueIndex].DutchSentence);
        else
            await SpeakDutchAsync($"Zoek {_quest.Title}. Tik op het juiste voorwerp.");
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try
        {
            var languages = await TextToSpeech.Default.GetLocalesAsync();
            var dutch = languages.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = dutch });
        }
        catch { }
    }

    private static async Task SpeakFrenchAsync(string text)
    {
        try
        {
            var languages = await TextToSpeech.Default.GetLocalesAsync();
            var french = languages.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = french });
        }
        catch { }
    }
}
