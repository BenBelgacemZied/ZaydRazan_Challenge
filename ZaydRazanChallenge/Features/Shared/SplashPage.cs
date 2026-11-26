namespace ZaydRazanChallenge.Features.Shared;

public sealed class SplashPage : ContentPage
{
    private readonly IGamePageFactory _pages;
    private readonly ILearningLanguageSettings _languageSettings;
    private readonly Border _brandBadge;
    private readonly VerticalStackLayout _languagePanel;
    private readonly Grid _welcomePanel;
    private readonly Image _characters;
    private readonly Label _slogan;
    private readonly Label _names;
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

        _slogan = new Label
        {
            Text = "Op reis met",
            FontSize = 31,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.WordWrap,
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 3), Radius = 5, Opacity = .75f }
        };

        var firstTitleLine = new Border
        {
            BackgroundColor = Color.FromArgb("#0879D5"),
            Stroke = Color.FromArgb("#8EDBFF"),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 34 },
            Padding = new Thickness(18, 8),
            Margin = new Thickness(28, 12, 28, 0),
            HorizontalOptions = LayoutOptions.Fill,
            Content = _slogan,
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Radius = 8, Opacity = .4f }
        };

        _names = new Label
        {
            FontSize = 36,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap,
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 3), Radius = 4, Opacity = .3f }
        };

        _names.FormattedText = CreateNamesTitle(isEnglish: false);

        var globe = new Label
        {
            Text = "🌍",
            FontSize = 30,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            HeightRequest = 38,
            Margin = new Thickness(0, -5, 0, 0)
        };

        var namesAndGlobe = new VerticalStackLayout
        {
            Spacing = -5,
            Children = { _names, globe }
        };

        var namesCloud = new Border
        {
            BackgroundColor = Color.FromArgb("#FFFCF7"),
            Stroke = Colors.White,
            StrokeThickness = 3,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 42 },
            Padding = new Thickness(8, 5),
            Margin = new Thickness(8, -4, 8, 0),
            HorizontalOptions = LayoutOptions.Fill,
            Content = namesAndGlobe,
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 5), Radius = 8, Opacity = .35f }
        };

        var titlePanel = new VerticalStackLayout
        {
            Spacing = 0,
            Margin = new Thickness(8, 0, 8, 0),
            Children = { firstTitleLine, namesCloud }
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
            Children = { _characters, titlePanel, _progress }
        };
        Grid.SetRowSpan(_characters, 3);
        Grid.SetRow(titlePanel, 0);
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

    private static FormattedString CreateNamesTitle(bool isEnglish) => new()
    {
        Spans =
        {
            new Span { Text = "Zayd", TextColor = Color.FromArgb("#FFAA00"), FontAttributes = FontAttributes.Bold },
            new Span { Text = isEnglish ? " & " : " en ", TextColor = Color.FromArgb("#1685E8"), FontAttributes = FontAttributes.Bold },
            new Span { Text = "Razan", TextColor = Color.FromArgb("#E91E63"), FontAttributes = FontAttributes.Bold }
        }
    };

    private async Task ChooseLanguageAsync(string languageCode)
    {
        if (_languageChosen) return;

        _languageChosen = true;
        _languageSettings.SourceLanguageCode = languageCode;
        var english = languageCode == "en";

        _slogan.Text = english ? "Travel with" : "Op reis met";
        _names.FormattedText = CreateNamesTitle(english);
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