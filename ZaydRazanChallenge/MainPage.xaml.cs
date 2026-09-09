namespace ZaydRazanChallenge;

public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StarsLabel.Text = $"⭐ {Preferences.Default.Get("stars", 0)}";
        var stage = Preferences.Default.Get("adventure_stage", 0);
        var total = AdventurePage.StageCount;
        AdventureProgressBar.Progress = Math.Min(1d, (double)stage / total);
        AdventureProgressLabel.Text = stage == 0
            ? "Begin thuis en ontgrendel stap voor stap de reis naar Parijs."
            : stage >= total
                ? "Avontuur voltooid! Je kunt opnieuw spelen."
                : $"Etappe {stage + 1} van {total} is ontgrendeld.";
        AdventureButton.Text = stage == 0 ? "▶  Start het avontuur"
            : stage >= total ? "↻  Speel opnieuw" : "▶  Ga verder";
    }

    private async void OnAdventureClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new AdventurePage());

    private async void OnMatchingClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Matching));
    private async void OnListeningClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Listening));
    private async void OnMonumentsClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Monuments));
    private async void OnPuzzleClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new PuzzlePage());
}
