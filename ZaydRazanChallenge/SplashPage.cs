namespace ZaydRazanChallenge;

public sealed class SplashPage : ContentPage
{
    private bool _navigationStarted;

    public SplashPage()
    {
        BackgroundColor = Color.FromArgb("#DDF2FF");
        Content = new Grid
        {
            Children =
            {
                new Image
                {
                    Source = "splash.jpg",
                    Aspect = Aspect.AspectFit,
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
            window.Page = App.CreateHomePage();
    }
}
