using Microsoft.Maui.Layouts;

namespace ZaydRazanChallenge;

public sealed class ParisTreasurePage : ContentPage
{
    private sealed record AdventureMoment(string Icon, string Action, string Intro, string Discovery);

    private static readonly AdventureMoment[] AdventureMoments =
    [
        new("🗺️", "Bekijk de route", "De reis gaat verder. Bekijk samen de route door Parijs.", "Op de kaart verschijnt de eerste aanwijzing."),
        new("🧭", "Gebruik het kompas", "Het kompas wijst naar een nieuwe plek. Tik om de richting te volgen.", "Het kompas leidt naar de eerste aanwijzing."),
        new("📓", "Open het reisdagboek", "Er wacht een nieuwe bladzijde in het reisdagboek.", "In het reisdagboek staat de eerste aanwijzing."),
        new("📷", "Bekijk de foto", "Razan heeft onderweg een mysterieuze foto gemaakt. Bekijk hem goed.", "Op de foto ontdekken ze de eerste aanwijzing."),
        new("🔭", "Kijk door de verrekijker", "In de verte is iets bijzonders te zien. Kijk door de verrekijker.", "Aan de horizon vinden ze de eerste aanwijzing."),
        new("👣", "Volg het spoor", "Glanzende voetstappen wijzen de weg door Parijs. Volg het spoor.", "Het spoor eindigt bij de eerste aanwijzing.")
    ];

    private static readonly string[] ChapterImages =
    [
        "paris_journey_map.jpg", "paris_journey_footsteps.jpg", "paris_journey_river.jpg",
        "paris_journey_art.jpg", "paris_journey_neighborhood.jpg", "paris_journey_finale.jpg"
    ];

    private readonly int _stage;
    private readonly ParisQuest _quest;
    private readonly Grid _root = new();
    private readonly AbsoluteLayout _scene = new();
    private readonly Label _status = new() { FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _instruction = new() { FontSize = 16, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _question = new() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Label _feedback = new() { FontSize = 14, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center };
    private readonly Grid _choices = new() { ColumnSpacing = 5 };
    private readonly BoxView _fog = new() { Color = Color.FromArgb("#65768F"), Opacity = .40, InputTransparent = true };
    private readonly AdventureMoment _moment;
    private readonly Button _startAction = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, BackgroundColor = Color.FromArgb("#C97920"), TextColor = Colors.White, CornerRadius = 18, HeightRequest = 58, AutomationId = "paris-start-exploration" };
    private readonly VerticalStackLayout _missionPanel = new() { Spacing = 9, Padding = new Thickness(14, 14, 14, 18), BackgroundColor = Color.FromArgb("#E817324D"), VerticalOptions = LayoutOptions.End };
    private int _clueIndex;
    private bool _missionStarted;
    private bool _busy;
    private bool _questionReady;
    private bool _finished;

    public ParisTreasurePage(int stage)
    {
        if (stage < ParisTreasureCatalog.FirstStage || stage >= ParisTreasureCatalog.FirstStage + ParisTreasureCatalog.Count)
            throw new ArgumentOutOfRangeException(nameof(stage));
        _stage = stage;
        var questIndex = stage - ParisTreasureCatalog.FirstStage;
        _quest = ParisTreasureCatalog.Quests[questIndex];
        _moment = AdventureMoments[questIndex % AdventureMoments.Length];
        Title = "Schattenjacht in Parijs";
        BackgroundColor = Color.FromArgb("#17324D");
        GameUi.AddHomeButton(this);

        var image = new Image { Source = ChapterImages[questIndex / 5], Aspect = Aspect.AspectFill };
        AbsoluteLayout.SetLayoutBounds(image, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
        _scene.Add(image);
        AbsoluteLayout.SetLayoutBounds(_fog, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(_fog, AbsoluteLayoutFlags.All);
        _scene.Add(_fog);

        _startAction.Text = $"{_moment.Icon}  {_moment.Action}";
        _startAction.Clicked += async (_, _) => await StartMissionAsync();
        AbsoluteLayout.SetLayoutBounds(_startAction, new Rect(.5, .91, .88, 58));
        AbsoluteLayout.SetLayoutFlags(_startAction, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
        _scene.Add(_startAction);

        var hear = new Button { Text = "🔊", FontSize = 21, BackgroundColor = Color.FromArgb("#C97920"), TextColor = Colors.White, CornerRadius = 24, WidthRequest = 46, HeightRequest = 46 };
        hear.Clicked += async (_, _) => await SpeakCurrentAsync();
        var top = new Grid { Padding = new Thickness(12, 4), VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.FromArgb("#BE17324D"), ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        top.Add(_status);
        top.Add(hear, 1, 0);
        _status.Text = $"{_moment.Icon} Parijs {questIndex + 1}/30 · onderweg";

        for (var i = 0; i < 3; i++) _choices.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        _missionPanel.Children.Add(_instruction);
        _missionPanel.Children.Add(_question);
        _missionPanel.Children.Add(_choices);
        _missionPanel.Children.Add(_feedback);
        _missionPanel.IsVisible = false;
        _root.Add(_scene);
        _root.Add(top);
        _root.Add(_missionPanel);
        Content = _root;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_missionStarted && !_finished)
            await SpeakDutchAsync(_moment.Intro);
    }

    private void RenderClue()
    {
        _questionReady = false;
        _status.Text = $"☁️ Parijs {(_stage - 3)}/30 · aanwijzing {_clueIndex + 1}/3";
        _instruction.Text = _clueIndex == 0
            ? _moment.Discovery
            : ((_stage + _clueIndex) % 2 == 0 ? "Razan ontdekt de volgende aanwijzing:" : "Zayd ontdekt de volgende aanwijzing:");
        _feedback.Text = "";
        _choices.Children.Clear();
        var clue = _quest.Clues[_clueIndex];
        _question.Text = clue.DutchSentence + " Welk Frans woord betekent ‘" + clue.Highlight + "’?";
        foreach (var (answer, index) in clue.Choices.OrderBy(_ => Random.Shared.Next()).Select((value, index) => (value, index)))
        {
            var choice = answer;
            var button = new Button
            {
                Text = choice, FontSize = 14, FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#2051A3"), TextColor = Colors.White,
                CornerRadius = 12, MinimumHeightRequest = 52, Padding = new Thickness(3, 2),
                IsEnabled = false, AutomationId = "paris-answer-" + choice
            };
            button.Clicked += async (_, _) => await AnswerAsync(choice, button);
            _choices.Add(button, index, 0);
        }
    }

    private async Task ReadQuestionAsync()
    {
        await SpeakDutchAsync(_question.Text);
        if (_finished || !_missionStarted || _busy) return;
        _questionReady = true;
        foreach (var child in _choices.Children)
            if (child is Button button) button.IsEnabled = true;
    }

    private async Task StartMissionAsync()
    {
        if (_busy || _missionStarted) return;
        _missionStarted = true;
        _startAction.IsVisible = false;
        _missionPanel.IsVisible = true;
        RenderClue();
        await ReadQuestionAsync();
    }

    private async Task AnswerAsync(string answer, Button button)
    {
        if (_busy || !_questionReady || !_missionStarted) return;
        _busy = true;
        _questionReady = false;
        foreach (var child in _choices.Children)
            if (child is Button choice) choice.IsEnabled = false;
        await SpeakFrenchAsync(answer);
        var clue = _quest.Clues[_clueIndex];
        if (answer != clue.Word)
        {
            button.BackgroundColor = Color.FromArgb("#B92E3D");
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            _feedback.Text = "Probeer opnieuw. Luister naar de vraag.";
            await GameFeedback.FailureAsync();
            await SpeakDutchAsync("Probeer opnieuw.");
            button.BackgroundColor = Color.FromArgb("#2051A3");
            _busy = false;
            _questionReady = true;
            foreach (var child in _choices.Children)
                if (child is Button choice) choice.IsEnabled = true;
            return;
        }

        button.BackgroundColor = Color.FromArgb("#15803D");
        _feedback.Text = $"Goed zo! {clue.Highlight} = {clue.Word}";
        await GameFeedback.SuccessAsync();
        _clueIndex++;
        await _fog.FadeTo(Math.Max(0, .40 - _clueIndex * .13), 400);
        await Task.Delay(450);
        if (_clueIndex == _quest.Clues.Length)
            ShowPhotoChoices();
        else
        {
            RenderClue();
            _busy = false;
            await ReadQuestionAsync();
            return;
        }
        _busy = false;
    }

    private void ShowPhotoChoices()
    {
        _status.Text = "🎯 Parijs · welke foto is de schat?";
        _instruction.Text = "Razan en Zayd hebben alle aanwijzingen gevonden.";
        _question.Text = "Welke foto past bij alle aanwijzingen?";
        _feedback.Text = "Kies een van de drie foto's.";
        _choices.Children.Clear();
        var photos = ParisTreasureCatalog.PhotosFor(_stage - ParisTreasureCatalog.FirstStage);
        for (var i = 0; i < photos.Length; i++)
        {
            var index = i;
            var tile = new ImageButton
            {
                Source = photos[i], Aspect = Aspect.AspectFill,
                BackgroundColor = Colors.Transparent, BorderWidth = 0,
                Padding = 0, HeightRequest = 155, AutomationId = $"paris-object-{index}"
            };
            tile.Clicked += async (_, _) => await FindAsync(index, tile);
            _choices.Add(tile, i, 0);
        }
        _ = SpeakDutchAsync(_question.Text);
    }

    private async Task FindAsync(int index, ImageButton tile)
    {
        if (_busy || _finished) return;
        _busy = true;
        if (index != (_stage - ParisTreasureCatalog.FirstStage) % 3)
        {
            AdventureSave.Set("stars", Math.Max(0, AdventureSave.Get("stars", 0) - 1));
            await GameFeedback.FailureAsync();
            await SpeakDutchAsync("Dat is het niet. Kijk nog eens goed.");
            _busy = false;
            return;
        }

        _finished = true;
        foreach (var child in _choices.Children)
            if (child is ImageButton choice) choice.IsEnabled = false;
        await GameFeedback.SuccessAsync();
        await SpeakFrenchAsync(_quest.Target);
        var completionKey = $"paris_complete_{_stage}";
        if (!AdventureSave.Get(completionKey, false))
        {
            AdventureSave.Set(completionKey, true);
            AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 1);
        }
        if (!AdventureSave.IsTestMode)
            AdventureSave.Set("adventure_stage", Math.Max(_stage + 1, AdventureSave.Get("adventure_stage", 0)));
        ShowDiscovery();
    }

    private void ShowDiscovery()
    {
        _root.Children.Clear();
        var discovered = new Grid { RowDefinitions = { new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) }, BackgroundColor = Color.FromArgb("#17324D") };
        var photo = new Image { Source = _quest.Photo, Aspect = Aspect.AspectFit, Margin = new Thickness(5), AutomationId = "paris-discovered-photo" };
        discovered.Add(photo, 0, 0);
        var story = new VerticalStackLayout { Padding = 16, Spacing = 12, BackgroundColor = Color.FromArgb("#17324D") };
        story.Children.Add(new Label { Text = "⭐ " + _quest.Title, FontSize = 23, FontAttributes = FontAttributes.Bold, TextColor = Colors.White });
        story.Children.Add(new Label { Text = _quest.Story, FontSize = 17, TextColor = Colors.White });
        var listen = new Button { Text = "🔊 Luister naar het verhaal", BackgroundColor = Color.FromArgb("#2051A3"), TextColor = Colors.White };
        listen.Clicked += async (_, _) => await TellStoryAsync();
        story.Children.Add(listen);
        var next = new Button { Text = "Verder", BackgroundColor = Color.FromArgb("#C97920"), TextColor = Colors.White };
        next.Clicked += async (_, _) => await Navigation.PopAsync();
        story.Children.Add(next);
        discovered.Add(story, 0, 1);
        _root.Add(discovered);
        _ = TellStoryAsync();
    }

    private async Task TellStoryAsync()
    {
        await SpeakFrenchAsync(_quest.Target);
        await SpeakDutchAsync(_quest.Story);
    }

    private async Task SpeakCurrentAsync()
    {
        if (_finished) { await TellStoryAsync(); return; }
        if (!_missionStarted) await SpeakDutchAsync(_moment.Intro);
        else if (_clueIndex < _quest.Clues.Length) await ReadQuestionAsync();
        else await SpeakDutchAsync(_question.Text);
    }

    private static async Task SpeakDutchAsync(string text)
    {
        try
        {
            var languages = await TextToSpeech.Default.GetLocalesAsync();
            var dutch = languages.FirstOrDefault(x => x.Language.StartsWith("nl", StringComparison.OrdinalIgnoreCase));
            if (dutch is not null) await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = dutch });
        }
        catch { }
    }

    private static async Task SpeakFrenchAsync(string text)
    {
        try
        {
            var languages = await TextToSpeech.Default.GetLocalesAsync();
            var french = languages.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            if (french is not null) await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = french });
        }
        catch { }
    }
}
