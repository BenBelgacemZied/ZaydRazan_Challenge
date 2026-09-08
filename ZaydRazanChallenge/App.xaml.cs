namespace ZaydRazanChallenge;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new SplashPage();
    }

    public static NavigationPage CreateHomePage() => new(new MainPage())
    {
        BarBackgroundColor = Color.FromArgb("#183153"),
        BarTextColor = Colors.White
    };
}
