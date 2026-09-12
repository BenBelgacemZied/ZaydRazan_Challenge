namespace ZaydRazanChallenge;

public sealed class SplashPage : ContentPage
{
    private bool _navigationStarted;

    public SplashPage()
    {
        Background = new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(Color.FromArgb("#38BDF8"), 0),
                new GradientStop(Color.FromArgb("#E0F2FE"), 1)
            }, new Point(0, 0), new Point(0, 1));
        var title = new VerticalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "ZR LingoTrip", FontSize = 42, FontAttributes = FontAttributes.Bold, TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Center },
                new Label { Text = "Op reis met Zayd en Razan", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center }
            }
        };
        var characters = GameUi.OfficialCharacters(390);
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(new GridLength(1, GridUnitType.Star)),
                new RowDefinition(new GridLength(2.2, GridUnitType.Star))
            }
        };
        grid.Add(title, 0, 0);
        grid.Add(characters, 0, 1);
        Content = grid;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_navigationStarted)
            return;

        _navigationStarted = true;
        await Task.Delay(TimeSpan.FromSeconds(3));

        if (Application.Current?.Windows.FirstOrDefault() is Window window)
            window.Page = App.CreateHomePage();
    }
}
