namespace ZaydRazanChallenge;

public sealed class ParisChapterPage : ContentPage
{
    private readonly VerticalStackLayout _levels = new() { Spacing = 8, Padding = new Thickness(14, 12, 14, 24) };
    private bool _opening;

    public ParisChapterPage()
    {
        Title = "Parijs · 30 ontdekkingen";
        BackgroundColor = Color.FromArgb("#17324D");
        GameUi.AddHomeButton(this);
        var header = new Grid { HeightRequest = 185 };
        header.Add(new Image { Source = "paris_letter_scene.jpg", Aspect = Aspect.AspectFill });
        header.Add(new Label
        {
            Text = "☁️ 30 schatten in Parijs",
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(15, 10),
            FontSize = 23, FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#C017324D")
        });
        var content = new VerticalStackLayout { Spacing = 0 };
        content.Add(header);
        content.Add(_levels);
        Content = new ScrollView { Content = content };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _opening = false;
        RenderLevels();
    }

    private void RenderLevels()
    {
        _levels.Children.Clear();
        var saved = AdventureSave.Get("adventure_stage", ParisTreasureCatalog.FirstStage);
        var completed = Math.Clamp(saved - ParisTreasureCatalog.FirstStage, 0, ParisTreasureCatalog.Count);
        _levels.Add(new Label
        {
            Text = completed == ParisTreasureCatalog.Count
                ? "🏆 Je hebt alle 30 schatten ontdekt!"
                : $"{completed}/30 gevonden · open de volgende brief",
            TextColor = Colors.White, FontSize = 18, FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 8)
        });
        for (var i = 0; i < ParisTreasureCatalog.Count; i++)
        {
            var index = i;
            var stage = i + ParisTreasureCatalog.FirstStage;
            var unlocked = i <= completed;
            var discovered = i < completed;
            var button = new Button
            {
                Text = discovered ? $"✓ {i + 1:00}  {ParisTreasureCatalog.Quests[i].Title}" : $"✉️ {i + 1:00}  " + (unlocked ? "Open de brief" : "Nog niet ontdekt"),
                HorizontalOptions = LayoutOptions.Fill,
                FontSize = 16, FontAttributes = FontAttributes.Bold,
                HeightRequest = 58, CornerRadius = 14,
                TextColor = Colors.White,
                BackgroundColor = discovered ? Color.FromArgb("#167D61") : unlocked ? Color.FromArgb("#C97920") : Color.FromArgb("#536477"),
                IsEnabled = unlocked,
                AutomationId = $"paris-level-{index + 1}"
            };
            button.Clicked += async (_, _) =>
            {
                if (_opening) return;
                _opening = true;
                try { await Navigation.PushAsync(new ParisTreasurePage(stage)); }
                finally { _opening = false; }
            };
            _levels.Add(button);
        }
    }
}
