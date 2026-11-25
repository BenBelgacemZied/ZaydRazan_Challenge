namespace ZaydRazanChallenge.Features.Home.Pages;

public partial class MainPage : ContentPage
{
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private readonly IDestinationContent _destination;
    private readonly IGamePageFactory _pages;
    private readonly ILearningLanguageSettings _languageSettings;
    private bool _settingPicker;

    public MainPage(IDestinationContent destination, IGamePageFactory pages, IGameProgressStore progress, ISpeechService speech, ILearningLanguageSettings languageSettings)
    {
        _speechService = speech;
        _gameProgress = progress;
        InitializeComponent();
        _destination = destination;
        _pages = pages;
        _languageSettings = languageSettings;
        _settingPicker = true;
        SourceLanguagePicker.ItemsSource = new[] { "Nederlands", "English" };
        ApplyLanguage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _gameProgress.EndTest();
        ApplyLanguage();
        StarsLabel.Text = $"⭐ {_gameProgress.Get("stars", 0)}";
        var stage = _gameProgress.Get("adventure_stage", 0);
        var total = _destination.FirstStage + _destination.Count;
        AdventureProgressBar.Progress = Math.Min(1d, (double)stage / total);
        UpdateAdventureProgress(stage, total);
    }

    private void OnSourceLanguageChanged(object sender, EventArgs e)
    {
        if (_settingPicker || SourceLanguagePicker.SelectedIndex < 0) return;
        _languageSettings.SourceLanguageCode = SourceLanguagePicker.SelectedIndex == 1 ? "en" : "nl";
        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        var english = _languageSettings.SourceLanguageCode == "en";
        _settingPicker = true;
        SourceLanguagePicker.SelectedIndex = english ? 1 : 0;
        SourceLanguagePicker.Title = english ? "Source language" : "Brontaal";
        _settingPicker = false;

        SubtitleLabel.Text = english ? "Paris · Learn French" : "Parijs · Frans leren";
        AdventureTitleLabel.Text = english ? "🎒 Main adventure" : "🎒 Hoofdavontuur";
        MatchingLabel.Text = english ? "Words" : "Woorden";
        ListeningLabel.Text = english ? "Listen" : "Luisteren";
        PuzzleLabel.Text = english ? "Puzzle" : "Puzzel";
        ResetButton.Text = english ? "↻  Reset the app" : "↻  App resetten";

        var stage = _gameProgress.Get("adventure_stage", 0);
        UpdateAdventureProgress(stage, _destination.FirstStage + _destination.Count);
    }

    private void UpdateAdventureProgress(int stage, int total)
    {
        AdventureProgressBar.Progress = Math.Min(1d, (double)stage / total);
        var english = _languageSettings.SourceLanguageCode == "en";
        AdventureProgressLabel.Text = stage == 0
            ? (english ? "Start at home and unlock each stage on the way to Paris." : "Begin thuis en ontgrendel elke etappe naar Parijs.")
            : stage >= total
                ? (english ? "Adventure complete! You can start again." : "Avontuur voltooid! Je kunt opnieuw beginnen.")
                : (english ? $"Stage {stage + 1} of {total} is open." : $"Etappe {stage + 1} van {total} is open.");
        AdventureButton.Text = stage == 0
            ? (english ? "▶  Start the adventure" : "▶  Begin het avontuur")
            : stage >= total
                ? (english ? "↻  Play again" : "↻  Opnieuw")
                : (english ? "▶  Continue" : "▶  Verder");
    }

    private async void OnAdventureClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(_pages.CreateAdventurePage());

    private async void OnResetClicked(object sender, EventArgs e)
    {
        var english = _languageSettings.SourceLanguageCode == "en";
        var reset = await DisplayAlert(
            english ? "Reset the app" : "App resetten",
            english ? "All stars, progress and saved answers will be deleted." : "Alle sterren, voortgang en opgeslagen antwoorden worden verwijderd.",
            english ? "Reset" : "Resetten",
            english ? "Cancel" : "Annuleren");
        if (!reset) return;

        _gameProgress.EndTest();
        _gameProgress.Clear();
        await DisplayAlert(
            _languageSettings.SourceLanguageCode == "en" ? "App reset" : "App gereset",
            _languageSettings.SourceLanguageCode == "en" ? "The adventure starts again at level 1." : "Het avontuur begint opnieuw bij niveau 1.",
            "OK");
        OnAppearing();
    }

    private async void OnMatchingClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(_pages.CreateWordGamesHubPage());
    private async void OnListeningClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(_pages.CreateWordGamePage(WordGameMode.Listening));
    private async void OnMonumentsClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Monuments, _gameProgress, _speechService, _destination));
    private async void OnPuzzleClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(_pages.CreatePuzzleGalleryPage());
}
