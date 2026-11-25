namespace ZaydRazanChallenge.Features.Home.Pages;

public partial class MainPage : ContentPage
{
    private readonly ISpeechService _speechService;
    private readonly IGameProgressStore _gameProgress;
    private readonly IDestinationContent _destination;
    private readonly IGamePageFactory _pages;

    public MainPage(IDestinationContent destination, IGamePageFactory pages, IGameProgressStore progress, ISpeechService speech)
    {
        _speechService = speech;
        _gameProgress = progress;
        InitializeComponent();
        _destination = destination;
        _pages = pages;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _gameProgress.EndTest();
        StarsLabel.Text = $"⭐ {_gameProgress.Get("stars", 0)}";
        var stage = _gameProgress.Get("adventure_stage", 0);
        var total = _destination.FirstStage + _destination.Count;
        AdventureProgressBar.Progress = Math.Min(1d, (double)stage / total);
        AdventureProgressLabel.Text = stage == 0
            ? "Begin thuis en ontgrendel elke etappe naar Parijs."
            : stage >= total
                ? "Avontuur voltooid! Je kunt opnieuw beginnen."
                : $"Etappe {stage + 1} van {total} is open.";
        AdventureButton.Text = stage == 0 ? "▶  Begin het avontuur"
            : stage >= total ? "↻  Opnieuw" : "▶  Verder";
    }

    private async void OnAdventureClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(_pages.CreateAdventurePage());

    private async void OnResetClicked(object sender, EventArgs e)
    {
        var reset = await DisplayAlert(
            "Réinitialiser l'application",
            "Toutes les étoiles, la progression et les réponses enregistrées seront supprimées.",
            "Réinitialiser",
            "Annuler");
        if (!reset) return;

        _gameProgress.EndTest();
        _gameProgress.Clear();
        await DisplayAlert("Application réinitialisée", "L'aventure recommence au niveau 1.", "OK");
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
