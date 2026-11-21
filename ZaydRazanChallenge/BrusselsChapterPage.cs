namespace ZaydRazanChallenge;

public sealed class BrusselsChapterPage : ContentPage
{
    private readonly VerticalStackLayout _route = new() { Spacing = 10 };
    private readonly ProgressBar _progress = new()
    {
        ProgressColor = Color.FromArgb("#F4B942"),
        BackgroundColor = Color.FromArgb("#B9DCEB"),
        HeightRequest = 9
    };
    private readonly Label _progressText = new()
    {
        FontSize = 14,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb("#17324D"),
        HorizontalTextAlignment = TextAlignment.End
    };
    private bool _opening;

    public BrusselsChapterPage()
    {
        Title = "Bruxelles · nouvelle aventure";
        BackgroundColor = Color.FromArgb("#EAF8FF");
        GameUi.AddHomeButton(this);

        var hero = new Border
        {
            Margin = new Thickness(12, 12, 12, 0),
            Padding = new Thickness(18, 20),
            Stroke = Color.FromArgb("#D62828"),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 26 },
            Background = new LinearGradientBrush(
                new GradientStopCollection
                {
                    new GradientStop(Color.FromArgb("#D62828"), 0),
                    new GradientStop(Color.FromArgb("#17324D"), 1)
                },
                new Point(0, 0),
                new Point(1, 1)),
            Content = new VerticalStackLayout
            {
                Spacing = 7,
                Children =
                {
                    new Label { Text = "🇧🇪  BRUXELLES", FontSize = 30, FontAttributes = FontAttributes.Bold, TextColor = Colors.White },
                    new Label { Text = "Une nouvelle ville, une nouvelle histoire !", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Colors.White },
                    new Label { Text = "Arrivée, monuments, puzzle et jeux de mots.", FontSize = 15, TextColor = Color.FromArgb("#E6F7FF") }
                }
            }
        };

        var progressPanel = new Border
        {
            Margin = new Thickness(12, 10, 12, 4),
            Padding = new Thickness(14, 10),
            Stroke = Color.FromArgb("#B9DCEB"),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
            BackgroundColor = Colors.White,
            Content = new Grid
            {
                RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) },
                Children =
                {
                    new Label { Text = "🧭 Carnet de voyage Bruxelles", FontSize = 19, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D") },
                    _progress,
                    _progressText
                }
            }
        };
        Grid.SetRow(_progress, 1);
        Grid.SetRow(_progressText, 1);
        Grid.SetColumn(_progressText, 1);

        var miniGames = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) },
            ColumnSpacing = 10,
            Margin = new Thickness(12, 4, 12, 8)
        };
        var puzzle = MakeButton("🧩 Puzzle Bruxelles", "#0077B6");
        puzzle.Clicked += async (_, _) => await Navigation.PushAsync(new BrusselsPuzzlePage());
        var words = MakeButton("🔤 Jeux de mots", "#7C3AED");
        words.Clicked += async (_, _) => await Navigation.PushAsync(new BrusselsWordGamePage());
        miniGames.Add(puzzle);
        miniGames.Add(words, 1, 0);

        var content = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { hero, progressPanel, miniGames, _route }
        };
        Content = new ScrollView { Content = content };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RenderRoute();
    }

    private void RenderRoute()
    {
        _route.Clear();
        var completed = Math.Clamp(AdventureSave.Get("brussels_stage", 0), 0, BrusselsCatalog.Missions.Count);
        _progress.Progress = (double)completed / BrusselsCatalog.Missions.Count;
        _progressText.Text = $"{completed}/{BrusselsCatalog.Missions.Count} monuments découverts";

        if (completed == BrusselsCatalog.Missions.Count)
        {
            _route.Add(new Label
            {
                Text = "🏆 Bruxelles est complètement découverte !",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#15803D"),
                HorizontalTextAlignment = TextAlignment.Center
            });
        }

        for (var index = 0; index < BrusselsCatalog.Missions.Count; index++)
        {
            var mission = BrusselsCatalog.Missions[index];
            var unlocked = index <= completed;
            var discovered = index < completed;
            var button = new Button
            {
                Text = discovered
                    ? $"✅  {mission.Icon}  {index + 1}. {mission.Title}"
                    : unlocked
                        ? $"▶  {mission.Icon}  {index + 1}. {mission.Title}"
                        : $"🔒  {index + 1}. {mission.Title}",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 58,
                CornerRadius = 17,
                TextColor = Colors.White,
                BackgroundColor = discovered
                    ? Color.FromArgb("#15803D")
                    : unlocked ? Color.FromArgb("#D62828") : Color.FromArgb("#94A3B8"),
                IsEnabled = unlocked,
                AutomationId = $"brussels-mission-{index + 1}"
            };
            button.Clicked += async (_, _) =>
            {
                if (_opening) return;
                _opening = true;
                try { await Navigation.PushAsync(new BrusselsMissionPage(index)); }
                finally { _opening = false; }
            };
            _route.Add(button);
        }
    }

    private static Button MakeButton(string text, string color) => new()
    {
        Text = text,
        FontSize = 14,
        FontAttributes = FontAttributes.Bold,
        BackgroundColor = Color.FromArgb(color),
        TextColor = Colors.White,
        CornerRadius = 15,
        MinimumHeightRequest = 58,
        Padding = new Thickness(5, 4)
    };
}
