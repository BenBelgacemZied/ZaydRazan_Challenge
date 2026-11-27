namespace ZaydRazanChallenge.Features.Shared;

public sealed class SplashPage : ContentPage
{
    private readonly IGamePageFactory _pages;
    private readonly ILearningLanguageSettings _languageSettings;
    private readonly Border _brandBadge;
    private readonly VerticalStackLayout _languagePanel;
    private readonly Grid _welcomePanel;
    private readonly Image _characters;
    private readonly ISplashImageProvider _splashImageProvider;
    private readonly ProgressBar _progress;
    private bool _languageChosen;
    private bool _navigationStarted;

    public SplashPage(IGamePageFactory pages, ILearningLanguageSettings languageSettings, ISplashImageProvider splashImageProvider)
    {
        _pages = pages;
        _languageSettings = languageSettings;
        _splashImageProvider = splashImageProvider;
        NavigationPage.SetHasNavigationBar(this, false);
        BackgroundColor = Color.FromArgb("#102A43");

        _brandBadge = new Border
        {
            WidthRequest = 86,
            HeightRequest = 86,
            Padding = 0,
            BackgroundColor = Color.FromArgb("#0EA5A8"),
            Stroke = Color.FromArgb("#B7F3EF"),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 28 },
            Content = new Label
            {
                Text = "ZR",
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }
        };

        var brandName = new Label
        {
            Text = "LingoTrip",
            FontSize = 34,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            VerticalTextAlignment = TextAlignment.Center
        };

        var brandRow = new HorizontalStackLayout
        {
            Spacing = 13,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { _brandBadge, brandName }
        };

        var brandSubtitle = new Label
        {
            Text = "🌍  Frans leren op reis  ·  Learn French as you travel",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#C5E7F5"),
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 3, 0, 10)
        };

        var languageTitle = new Label
        {
            Text = "Kies je taal  ·  Choose your language",
            FontSize = 19,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 14, 0, 2)
        };

        var dutchButton = CreateLanguageButton("🇳🇱   Nederlands", "#F59E0B");
        dutchButton.Clicked += async (_, _) => await ChooseLanguageAsync("nl");

        var englishButton = CreateLanguageButton("🇬🇧   English", "#2563EB");
        englishButton.Clicked += async (_, _) => await ChooseLanguageAsync("en");

        _languagePanel = new VerticalStackLayout
        {
            Spacing = 12,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { languageTitle, dutchButton, englishButton }
        };

        var languageContent = new VerticalStackLayout
        {
            Spacing = 12,
            Padding = new Thickness(26, 32),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Children = { brandRow, brandSubtitle, _languagePanel }
        };

        var languageScreen = new Grid
        {
            Children =
            {
                new BoxView
                {
                    Color = Color.FromArgb("#173B59"),
                    WidthRequest = 260,
                    HeightRequest = 260,
                    CornerRadius = 130,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Start,
                    TranslationX = 110,
                    TranslationY = -120,
                    Opacity = .55
                },
                new BoxView
                {
                    Color = Color.FromArgb("#0B7180"),
                    WidthRequest = 220,
                    HeightRequest = 220,
                    CornerRadius = 110,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.End,
                    TranslationX = -100,
                    TranslationY = 100,
                    Opacity = .35
                },
                languageContent
            }
        };

        _characters = new Image
        {
            Source = "splash.jpg",
            Aspect = Aspect.AspectFill,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        _progress = new ProgressBar
        {
            Progress = 0,
            ProgressColor = Color.FromArgb("#FBBF24"),
            BackgroundColor = Color.FromArgb("#38556B"),
            HeightRequest = 12,
            Margin = new Thickness(30, 0, 30, 14)
        };

        _welcomePanel = new Grid
        {
            IsVisible = false,
            Opacity = 0,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            },
            Padding = 0,
            Children = { _characters, _progress }
        };
        Grid.SetRowSpan(_characters, 3);
        Grid.SetRow(_progress, 2);

        var root = new Grid();
        root.Children.Add(languageScreen);
        root.Children.Add(_welcomePanel);
        Content = root;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_languageChosen)
            _ = PulseLogoAsync();
    }

    private static Button CreateLanguageButton(string text, string color) => new()
    {
        Text = text,
        FontSize = 19,
        HeightRequest = 60,
        BackgroundColor = Color.FromArgb(color),
        TextColor = Colors.White,
        CornerRadius = 20,
        BorderColor = Color.FromArgb("#C5E7F5"),
        BorderWidth = 1.5,
        HorizontalOptions = LayoutOptions.Fill
    };

    private async Task PulseLogoAsync()
    {
        while (!_languageChosen)
        {
            await _brandBadge.ScaleTo(1.06, 650, Easing.CubicOut);
            if (_languageChosen) break;
            await _brandBadge.ScaleTo(1, 650, Easing.CubicIn);
        }
    }

    private async Task ChooseLanguageAsync(string languageCode)
    {
        if (_languageChosen) return;

        _languageChosen = true;
        _languageSettings.SourceLanguageCode = languageCode;
        _characters.Source = _splashImageProvider.GetSplashImageAsset(languageCode);
        _welcomePanel.IsVisible = true;
        await _welcomePanel.FadeTo(1, 450, Easing.CubicOut);
        await _progress.ProgressTo(1, 1900, Easing.CubicInOut);
        await Task.Delay(250);

        if (_navigationStarted) return;
        _navigationStarted = true;

        if (Application.Current?.Windows.FirstOrDefault() is Window window)
        {
            window.Page = new NavigationPage(_pages.CreateHomePage())
            {
                BarBackgroundColor = Color.FromArgb("#183153"),
                BarTextColor = Colors.White
            };
        }
    }
}