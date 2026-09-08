namespace ZaydRazanChallenge;

public partial class MainPage : ContentPage
{
    public MainPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StarsLabel.Text = $"⭐ {Preferences.Default.Get("stars", 0)}";
    }

    private async void OnMatchingClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Matching));
    private async void OnListeningClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Listening));
    private async void OnMonumentsClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new GamePage(GameMode.Monuments));
}
