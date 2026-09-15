namespace ZaydRazanChallenge;

public sealed class ParisChapterPage : ContentPage
{
    private sealed record RouteChapter(string Icon, string Title, string Subtitle, string Image);

    private static readonly RouteChapter[] Chapters =
    [
        new("🗺️", "Aankomst in Parijs", "De eerste route door de stad", "paris_journey_map.jpg"),
        new("🏛️", "Oude monumenten", "Volg het spoor van de geschiedenis", "paris_journey_footsteps.jpg"),
        new("🔭", "De stad komt tot leven", "Kijk verder dan de bekende straten", "paris_journey_river.jpg"),
        new("🎨", "Kunst, pleinen en licht", "Verzamel aanwijzingen als echte ontdekkers", "paris_journey_art.jpg"),
        new("🥐", "Bruggen, buurten en smaken", "Dwaal door het dagelijkse Parijs", "paris_journey_neighborhood.jpg"),
        new("⭐", "De laatste ontdekkingen", "Maak het reisdagboek compleet", "paris_journey_finale.jpg")
    ];

    private readonly VerticalStackLayout _levels = new() { Spacing = 8, Padding = new Thickness(14, 12, 14, 24) };
    private bool _opening;

    public ParisChapterPage()
    {
        Title = "Parijs · 30 ontdekkingen";
        BackgroundColor = Color.FromArgb("#17324D");
        GameUi.AddHomeButton(this);
        var header = new Grid { HeightRequest = 225 };
        header.Add(new Image { Source = "paris_journey_map.jpg", Aspect = Aspect.AspectFill });
        header.Add(new Label
        {
            Text = "🧭 Ons avontuur door Parijs\n30 ontdekkingen · één grote reis",
            VerticalOptions = LayoutOptions.End,
            Padding = new Thickness(15, 10),
            FontSize = 22, FontAttributes = FontAttributes.Bold,
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
                : $"{completed}/30 ontdekt · reis naar de volgende etappe",
            TextColor = Colors.White, FontSize = 18, FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 8)
        });
        for (var chapterIndex = 0; chapterIndex < Chapters.Length; chapterIndex++)
        {
            var chapter = Chapters[chapterIndex];
            var firstLevel = chapterIndex * 5;
            var chapterProgress = Math.Clamp(completed - firstLevel, 0, 5);
            var chapterCard = new Border
            {
                Stroke = Color.FromArgb(chapterProgress == 5 ? "#39B98A" : "#D99B45"),
                StrokeThickness = 2,
                BackgroundColor = Color.FromArgb("#213F5C"),
                Padding = 0,
                Margin = new Thickness(0, chapterIndex == 0 ? 4 : 12, 0, 0),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 }
            };
            var chapterLayout = new VerticalStackLayout { Spacing = 8, Padding = new Thickness(12) };
            var chapterHeader = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(new GridLength(56)), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }
            };
            var thumbnail = new Image { Source = chapter.Image, Aspect = Aspect.AspectFill, HeightRequest = 54, WidthRequest = 54 };
            chapterHeader.Add(new Border
            {
                Content = thumbnail, Padding = 0, StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }
            }, 0, 0);
            chapterHeader.Add(new VerticalStackLayout
            {
                Padding = new Thickness(10, 2, 4, 0), Spacing = 1,
                Children =
                {
                    new Label { Text = $"{chapter.Icon}  {chapter.Title}", TextColor = Colors.White, FontSize = 17, FontAttributes = FontAttributes.Bold },
                    new Label { Text = chapter.Subtitle, TextColor = Color.FromArgb("#CBD9E6"), FontSize = 12 }
                }
            }, 1, 0);
            chapterHeader.Add(new Label { Text = $"{chapterProgress}/5", TextColor = Color.FromArgb("#FFD88A"), FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center }, 2, 0);
            chapterLayout.Add(chapterHeader);

            for (var offset = 0; offset < 5; offset++)
            {
                var index = firstLevel + offset;
                var stage = index + ParisTreasureCatalog.FirstStage;
                var unlocked = index <= completed;
                var discovered = index < completed;
                var button = new Button
                {
                    Text = discovered ? $"✓  ETAPPE {index + 1:00}  ·  {ParisTreasureCatalog.Quests[index].Title}" : unlocked ? $"▶  ETAPPE {index + 1:00}  ·  Vertrek" : $"🔒  ETAPPE {index + 1:00}",
                    HorizontalOptions = LayoutOptions.Fill,
                    FontSize = 14, FontAttributes = FontAttributes.Bold,
                    HeightRequest = 50, CornerRadius = 13,
                    TextColor = Colors.White,
                    BackgroundColor = discovered ? Color.FromArgb("#167D61") : unlocked ? Color.FromArgb("#C97920") : Color.FromArgb("#46586A"),
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
                chapterLayout.Add(button);
            }
            chapterCard.Content = chapterLayout;
            _levels.Add(chapterCard);
        }
    }
}
