using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class ParisTreasurePage : ContentPage
{
    private sealed record Clue(string Hint, string Question, string Word, string[] Choices);
    private sealed record Treasure(string Title, string Emoji, string Target, string Intro, Clue[] Clues, string[] Objects);

    private static readonly Treasure[] Quests =
    [
        new("De stadskaart", "🗺️", "la carte", "Zayd en Razan zijn in Parijs. Zoek eerst drie aanwijzingen voor hun kaart.",
            [
                new("Een kaart toont de straten.", "Hoe zeg je 'de kaart' in het Frans?", "la carte", ["la carte", "le pain", "la porte"]),
                new("Je ziet de weg op de kaart.", "Hoe zeg je 'de straat' in het Frans?", "la rue", ["le train", "la rue", "le siège"]),
                new("Samen zoeken ze Parijs.", "Hoe zeg je 'de stad' in het Frans?", "la ville", ["la ville", "la valise", "la table"])
            ], ["🗺️|la carte", "🥖|la baguette", "🎟️|le billet"]),
        new("La tour Eiffel", "🗼", "la tour Eiffel", "Razan zoekt een hoge toren. Ontdek drie aanwijzingen.",
            [
                new("De toren is heel hoog.", "Hoe zeg je 'hoog' in het Frans?", "haute", ["haute", "petite", "rouge"]),
                new("Je kunt naar de top kijken.", "Hoe zeg je 'de top' in het Frans?", "le sommet", ["le sommet", "le pain", "le quai"]),
                new("De toren staat in Parijs.", "Hoe zeg je 'de toren' in het Frans?", "la tour", ["la tour", "la rue", "la porte"])
            ], ["🗼|la tour Eiffel", "🏛️|l'Arc de Triomphe", "🥖|la baguette"]),
        new("Le musée du Louvre", "🖼️", "le Louvre", "Zayd zoekt een beroemd museum. Luister naar de aanwijzingen.",
            [
                new("In een museum zie je kunst.", "Hoe zeg je 'het museum' in het Frans?", "le musée", ["le musée", "le train", "le pain"]),
                new("Hier hangt de Mona Lisa.", "Hoe heet de Mona Lisa in het Frans?", "la Joconde", ["la fenêtre", "la Joconde", "la valise"]),
                new("Zayd zoekt een schilderij.", "Hoe zeg je 'het schilderij' in het Frans?", "le tableau", ["le billet", "la rue", "le tableau"])
            ], ["🖼️|le Louvre", "🗼|la tour Eiffel", "🥖|la baguette"]),
        new("L'Arc de Triomphe", "🏛️", "l'Arc de Triomphe", "Ze zoeken een grote boog in Parijs.",
            [
                new("De grote boog staat aan een plein.", "Hoe zeg je 'het plein' in het Frans?", "la place", ["la place", "la table", "la porte"]),
                new("De boog is een monument.", "Hoe zeg je 'het monument' in het Frans?", "le monument", ["le musée", "le monument", "le train"]),
                new("Je ziet een grote boog.", "Hoe zeg je 'de boog' in het Frans?", "l'arc", ["la rue", "le quai", "l'arc"])
            ], ["🏛️|l'Arc de Triomphe", "🖼️|le Louvre", "🗺️|la carte"]),
        new("La baguette", "🥖", "la baguette", "Zayd ruikt vers brood. Waar is de baguette?",
            [
                new("De bakker maakt vers brood.", "Hoe zeg je 'de bakker' in het Frans?", "le boulanger", ["le boulanger", "le musée", "le quai"]),
                new("In de bakkerij koop je brood.", "Hoe zeg je 'de bakkerij' in het Frans?", "la boulangerie", ["la gare", "la boulangerie", "la tour"]),
                new("Een baguette is lang brood.", "Hoe zeg je 'het brood' in het Frans?", "le pain", ["le train", "le siège", "le pain"])
            ], ["🗺️|la carte", "🥖|la baguette", "🖼️|le Louvre"])
    ];

    private static readonly Rect[] HintPlaces = [new(.12, .15, 74, 74), new(.85, .43, 74, 74), new(.18, .72, 74, 74)];
    private static readonly Rect[] ObjectPlaces = [new(.14, .30, 130, 95), new(.82, .50, 130, 95), new(.25, .78, 130, 95)];

    private readonly int _stage;
    private readonly Treasure _quest;
    private readonly AbsoluteLayout _scene = new();
    private readonly Label _status = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _instruction = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center };
    private readonly VerticalStackLayout _answers = new() { Spacing = 6 };
    private int _clueIndex;
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

        var image = new Image { Source = "adventure_map.jpg", Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(image, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
        _scene.Add(image);

        var hear = new Button { Text = "🔊", FontSize = 24, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White, CornerRadius = 30, WidthRequest = 58, HeightRequest = 58 };
        hear.Clicked += async (_, _) => await SpeakCurrentAsync();
        var top = new Grid { Padding = new Thickness(16, 6), ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        top.Add(_status);
        top.Add(hear, 1, 0);

        var bottom = new VerticalStackLayout { Padding = new Thickness(15, 8), Spacing = 7, BackgroundColor = Color.FromArgb("#F8FAFC"), Children = { _instruction, _answers } };
        var layout = new Grid { RowDefinitions = { new RowDefinition(new GridLength(70)), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) } };
        layout.Add(top, 0, 0);
        layout.Add(_scene, 0, 1);
        layout.Add(bottom, 0, 2);
        Content = layout;
        Render();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SpeakCurrentAsync();
    }

    private void Render()
    {
        foreach (var child in _scene.Children.Skip(1).ToArray()) _scene.Remove(child);
        _answers.Clear();
        if (_finished) return;
        if (_clueIndex < _quest.Clues.Length)
        {
            var clue = _quest.Clues[_clueIndex];
            _status.Text = $"{_quest.Emoji}  {_quest.Title}  ·  {_clueIndex + 1}/3";
            _instruction.Text = $"🔎 {clue.Question}";
            var spot = new Button { Text = "✉️", FontSize = 28, BackgroundColor = Color.FromArgb("#F59E0B"), TextColor = Colors.White, BorderColor = Colors.White, BorderWidth = 3, CornerRadius = 38 };
            spot.Clicked += async (_, _) => await SpeakDutchAsync(clue.Hint + " " + clue.Question);
            AbsoluteLayout.SetLayoutBounds(spot, HintPlaces[_clueIndex]);
            AbsoluteLayout.SetLayoutFlags(spot, AbsoluteLayoutFlags.PositionProportional);
            _scene.Add(spot);
            foreach (var answer in clue.Choices.OrderBy(_ => Random.Shared.Next()))
            {
                var selected = answer;
                var button = new Button { Text = selected, FontSize = 17, HeightRequest = 48, BackgroundColor = Color.FromArgb("#2563EB"), TextColor = Colors.White, CornerRadius = 14 };
                button.Clicked += async (_, _) => await AnswerAsync(selected, button);
                _answers.Add(button);
            }
        }
        else
        {
            _status.Text = $"{_quest.Emoji}  {_quest.Title}  ·  🎯";
            _instruction.Text = "Luister en tik het juiste voorwerp aan op de kaart.";
            foreach (var entry in _quest.Objects.Select((value, index) => (value, index)))
            {
                var parts = entry.value.Split('|');
                var target = parts[1];
                var button = new Button { Text = $"{parts[0]}\n{target}", FontSize = 15, FontAttributes = FontAttributes.Bold, BackgroundColor = Color.FromArgb("#EAFBF8"), TextColor = Color.FromArgb("#17324D"), BorderColor = Color.FromArgb("#F59E0B"), BorderWidth = 3, CornerRadius = 19 };
                button.Clicked += async (_, _) => await FindAsync(target, button);
                AbsoluteLayout.SetLayoutBounds(button, ObjectPlaces[entry.index]);
                AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
                _scene.Add(button);
            }
        }
    }

    private async Task AnswerAsync(string answer, Button button)
    {
        if (_busy) return;
        _busy = true;
        var clue = _quest.Clues[_clueIndex];
        if (answer != clue.Word)
        {
            button.BackgroundColor = Color.FromArgb("#DC2626");
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            await GameFeedback.FailureAsync();
            await SpeakDutchAsync("Probeer opnieuw. Luister naar de aanwijzing.");
            button.BackgroundColor = Color.FromArgb("#2563EB");
            _busy = false;
            return;
        }
        button.BackgroundColor = Color.FromArgb("#16A34A");
        await GameFeedback.SuccessAsync();
        await SpeakFrenchAsync(clue.Word);
        _clueIndex++;
        _busy = false;
        Render();
        await SpeakCurrentAsync();
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
        if (_clueIndex < _quest.Clues.Length)
        {
            var clue = _quest.Clues[_clueIndex];
            await SpeakDutchAsync(clue.Hint + " " + clue.Question);
        }
        else await SpeakDutchAsync($"Zoek {_quest.Title}. Tik op het juiste voorwerp.");
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
