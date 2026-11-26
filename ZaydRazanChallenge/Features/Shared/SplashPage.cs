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
    private readonly Label _sloganMidDepth;
    private readonly Label _sloganDeepDepth;
    private readonly Label _names;
    private readonly Label _namesMidDepth;
    private readonly Label _namesDeepDepth;
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

        _sloganDeepDepth = CreateTitleLabel(Color.FromArgb("#074078"), 31, 6);
        _sloganMidDepth = CreateTitleLabel(Color.FromArgb("#0A62A8"), 31, 3);
        _slogan = CreateTitleLabel(Colors.White, 31, 0);
        _slogan.Text = "Op reis met";
        _slogan.Shadow = new Shadow { Brush = Color.FromArgb("#063B70"), Offset = new Point(0, 2), Radius = 2, Opacity = .85f };

        _namesDeepDepth = CreateTitleLabel(Colors.White, 38, 6);
        _namesMidDepth = CreateTitleLabel(Colors.White, 38, 3);
        _names = CreateTitleLabel(Colors.White, 38, 0);
        _namesDeepDepth.FormattedText = CreateNamesTitle(false, "#974700", "#064878", "#90103E");
        _namesMidDepth.FormattedText = CreateNamesTitle(false, "#D27A00", "#0A65AA", "#BF164E");
        _names.FormattedText = CreateNamesTitle(false, "#FFAA00", "#1685E8", "#E91E63");
        _names.Shadow = new Shadow { Brush = Color.FromArgb("#55233B"), Offset = new Point(0, 2), Radius = 2, Opacity = .8f };

        var globe = new Label
        {
            Text = "🌍",
            FontSize = 32,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            HeightRequest = 38,
            Shadow = new Shadow { Brush = Color.FromArgb("#66452D"), Offset = new Point(0, 3), Radius = 3, Opacity = .65f }
        };

        var titlePanel = new Grid
        {
            HeightRequest = 194,
            Margin = new Thickness(0, 4, 0, 0),
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(88) },
                new RowDefinition { Height = new GridLength(58) },
                new RowDefinition { Height = new GridLength(42) }
            }
        };

        var whiteCloudDepth = CreateCloudShape(Color.FromArgb("#D5E7F2"), Color.FromArgb("#A9C8DB"));
        whiteCloudDepth.TranslationY = 8;
        titlePanel.Children.Add(whiteCloudDepth);
        Grid.SetRowSpan(whiteCloudDepth, 3);

        var whiteCloudBackground = CreateCloudShape(Color.FromArgb("#FFFCF7"), Colors.White);
        titlePanel.Children.Add(whiteCloudBackground);
        Grid.SetRowSpan(whiteCloudBackground, 3);

        var blueCloudHeader = new Grid
        {
            Margin = new Thickness(24, 0, 24, 0)
        };
        var blueCloudDepth = CreateCloudShape(Color.FromArgb("#0754A6"), Color.FromArgb("#06478B"));
        blueCloudDepth.TranslationY = 7;
        blueCloudHeader.Children.Add(blueCloudDepth);
        blueCloudHeader.Children.Add(CreateCloudShape(Color.FromArgb("#0879D5"), Color.FromArgb("#A6E8FF")));
        blueCloudHeader.Children.Add(_sloganDeepDepth);
        blueCloudHeader.Children.Add(_sloganMidDepth);
        blueCloudHeader.Children.Add(_slogan);
        titlePanel.Children.Add(blueCloudHeader);
        Grid.SetRow(blueCloudHeader, 0);

        titlePanel.Children.Add(_namesDeepDepth);
        Grid.SetRow(_namesDeepDepth, 1);
        titlePanel.Children.Add(_namesMidDepth);
        Grid.SetRow(_namesMidDepth, 1);
        titlePanel.Children.Add(_names);
        Grid.SetRow(_names, 1);

        titlePanel.Children.Add(globe);
        Grid.SetRow(globe, 2);

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

    private static Label CreateTitleLabel(Color color, double fontSize, double depth) => new()
    {
        FontFamily = "sans-serif-rounded",
        FontSize = fontSize,
        FontAttributes = FontAttributes.Bold,
        TextColor = color,
        HorizontalTextAlignment = TextAlignment.Center,
        VerticalTextAlignment = TextAlignment.Center,
        LineBreakMode = LineBreakMode.NoWrap,
        TranslationY = depth
    };

    private static GraphicsView CreateCloudShape(Color fillColor, Color outlineColor) => new()
    {
        Drawable = new CloudDrawable(fillColor, outlineColor),
        InputTransparent = true,
        Shadow = new Shadow { Brush = Color.FromArgb("#554C6A80"), Offset = new Point(0, 4), Radius = 5, Opacity = .45f }
    };

    private sealed class CloudDrawable(Color fillColor, Color outlineColor) : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var w = dirtyRect.Width;
            var h = dirtyRect.Height;
            var path = new PathF();

            path.MoveTo(w * .12f, h * .83f);
            path.CurveTo(w * .02f, h * .81f, w * .01f, h * .68f, w * .07f, h * .57f);
            path.CurveTo(w * .01f, h * .43f, w * .10f, h * .28f, w * .24f, h * .34f);
            path.CurveTo(w * .23f, h * .13f, w * .43f, h * .08f, w * .51f, h * .29f);
            path.CurveTo(w * .59f, h * .04f, w * .80f, h * .10f, w * .79f, h * .33f);
            path.CurveTo(w * .94f, h * .22f, w * 1.02f, h * .39f, w * .93f, h * .54f);
            path.CurveTo(w * 1.02f, h * .69f, w * .94f, h * .86f, w * .80f, h * .85f);
            path.CurveTo(w * .69f, h * .98f, w * .56f, h * .94f, w * .49f, h * .82f);
            path.CurveTo(w * .37f, h * .99f, w * .21f, h * .96f, w * .20f, h * .82f);
            path.CurveTo(w * .17f, h * .84f, w * .14f, h * .84f, w * .12f, h * .83f);
            path.Close();

            canvas.FillColor = fillColor;
            canvas.FillPath(path);
            canvas.StrokeColor = outlineColor;
            canvas.StrokeSize = 4;
            canvas.DrawPath(path);
        }
    }

    private static FormattedString CreateNamesTitle(bool isEnglish, string zaydColor, string linkColor, string razanColor) => new()
    {
        Spans =
        {
            new Span { Text = "Zayd", TextColor = Color.FromArgb(zaydColor), FontAttributes = FontAttributes.Bold },
            new Span { Text = isEnglish ? " & " : " en ", TextColor = Color.FromArgb(linkColor), FontAttributes = FontAttributes.Bold },
            new Span { Text = "Razan", TextColor = Color.FromArgb(razanColor), FontAttributes = FontAttributes.Bold }
        }
    };

    private async Task ChooseLanguageAsync(string languageCode)
    {
        if (_languageChosen) return;

        _languageChosen = true;
        _languageSettings.SourceLanguageCode = languageCode;
        var english = languageCode == "en";

        _slogan.Text = english ? "Travel with" : "Op reis met";
        _sloganMidDepth.Text = _slogan.Text;
        _sloganDeepDepth.Text = _slogan.Text;
        _namesDeepDepth.FormattedText = CreateNamesTitle(english, "#974700", "#064878", "#90103E");
        _namesMidDepth.FormattedText = CreateNamesTitle(english, "#D27A00", "#0A65AA", "#BF164E");
        _names.FormattedText = CreateNamesTitle(english, "#FFAA00", "#1685E8", "#E91E63");
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