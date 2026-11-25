namespace ZaydRazanChallenge.Features.Shared;

public sealed class SplashPage : ContentPage
{
    private readonly IGamePageFactory _pages;
    private readonly ILearningLanguageSettings _languageSettings;
    private readonly Border _brandBadge;
    private readonly VerticalStackLayout _languagePanel;
    private readonly VerticalStackLayout _welcomePanel;
    private readonly Image _characters;
    private readonly Label _slogan;
    private readonly Label _loadingMessage;
    private readonly ProgressBar _progress;
    private bool _languageChosen;
    private bool _navigationStarted;

    public SplashPage(IGamePageFactory pages, ILearningLanguageSettings languageSettings)
    {
        _pages = pages;
        _languageSettings = languageSettings;
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

        _slogan = new Label
        {
            FontSize = 27,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.WordWrap
        };

        _characters = new Image
        {
            Source = "zayd_razan_official.png",
            Aspect = Aspect.AspectFit,
            HeightRequest = 300,
            HorizontalOptions = LayoutOptions.Fill,
            Scale = .82
        };

        _loadingMessage = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#D7EEF8"),
            HorizontalTextAlignment = TextAlignment.Center
        };

        _progress = new ProgressBar
        {
            Progress = 0,
            ProgressColor = Color.FromArgb("#FBBF24"),
            BackgroundColor = Color.FromArgb("#38556B"),
            HeightRequest = 10,
            Margin = new Thickness(18, 0)
        };

        _welcomePanel = new VerticalStackLayout
        {
            IsVisible = false,
            Opacity = 0,
            Spacing = 12,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { _slogan, _characters, _loadingMessage, _progress }
        };

        var content = new VerticalStackLayout
        {
            Spacing = 12,
            Padding = new Thickness(26, 32),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Children = { brandRow, brandSubtitle, _languagePanel, _welcomePanel }
        };

        var background = new Grid
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
                content
            }
        };

        Content = background;
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
        var english = languageCode == "en";

        _languagePanel.IsVisible = false;
        _slogan.Text = english ? "Travel with Zayd & Razan" : "Op reis met Zayd en Razan";
        _loadingMessage.Text = english ? "Getting the adventure ready…" : "We maken de reis klaar…";
        _welcomePanel.IsVisible = true;

        await _welcomePanel.FadeTo(1, 450, Easing.CubicOut);
        await _characters.ScaleTo(1, 700, Easing.CubicOut);
        await _progress.ProgressTo(1, 1700, Easing.CubicInOut);
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
