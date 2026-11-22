namespace ZaydRazanChallenge;

public sealed class SplashPage : ContentPage
{
    private readonly IGamePageFactory _pages;
    private bool _navigationStarted;

    public SplashPage(IGamePageFactory pages)
    {
        _pages = pages;
        BackgroundColor = Color.FromArgb("#DDF2FF");
        Content = new Grid
        {
            Children =
            {
                new Image
                {
                    Source = "splash.jpg",
                    Aspect = Aspect.AspectFill,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                }
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_navigationStarted)
            return;

        _navigationStarted = true;
        await Task.Delay(TimeSpan.FromSeconds(3));

        if (Application.Current?.Windows.FirstOrDefault() is Window window)
            window.Page = new NavigationPage(_pages.CreateHomePage())
            {
                BarBackgroundColor = Color.FromArgb("#183153"),
                BarTextColor = Colors.White
            };
    }
}
