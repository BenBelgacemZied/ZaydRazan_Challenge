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
            ? "Begin thuis en ontgrendel elke etappe naar Parijs."
            : stage >= total
                ? "Avontuur voltooid! Je kunt opnieuw beginnen."
                : $"Etappe {stage + 1} van {total} is open.";
        AdventureButton.Text = stage == 0 ? "▶  Begin het avontuur"
            : stage >= total ? "↻  Opnieuw" : "▶  Verder";
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
        await Navigation.PushAsync(new PuzzleGalleryPage());
}
