namespace ZaydRazanChallenge;

public sealed class PackingMissionPage : ContentPage
{
    private sealed record Item(string Emoji, string French, string Dutch, bool Useful);
    private readonly string _preferenceKey;
    private readonly string _hero;
    private readonly Item[] _items;
    private readonly HashSet<string> _packed = [];
    private readonly Label _instruction = new() { FontSize = 20, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.White };
    private readonly Label _counter = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Colors.White };
    private readonly Label _suitcase = new() { FontSize = 31, HorizontalTextAlignment = TextAlignment.Center, Text = "" };
    private readonly FlexLayout _objects = new() { Wrap = FlexWrap.Wrap, JustifyContent = FlexJustify.Center, AlignItems = FlexAlignItems.Center };
    private bool _busy;

    private PackingMissionPage(string title, string hero, string image, string preferenceKey, Item[] items)
    {
        Title = title;
        _hero = hero;
        _preferenceKey = preferenceKey;
        _items = items;
        BackgroundColor = Color.FromArgb("#172554");
        GameUi.AddHomeButton(this);

        var scene = new Grid
        {
            HeightRequest = 500,
            Children =
            {
                new Image { Source = image, Aspect = Aspect.AspectFill },
                new BoxView { Color = Color.FromArgb("#4410203A") },
                new Border
                {
                    Margin = 12, Padding = new Thickness(14, 10), VerticalOptions = LayoutOptions.Start,
                    BackgroundColor = Color.FromArgb("#D917324D"), StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
                    Content = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Children = { _instruction, _counter } }
                },
                new Border
                {
                    Margin = new Thickness(50, 0, 50, 28), VerticalOptions = LayoutOptions.End, Padding = 8,
                    BackgroundColor = Color.FromArgb("#AAFFFFFF"), Stroke = Colors.White, StrokeThickness = 2,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 }, Content = _suitcase
                }
            }
        };
        Grid.SetColumn(_counter, 1);

        Content = new ScrollView { Content = new VerticalStackLayout { Spacing = 0, Children =
        {
            scene,
            new VerticalStackLayout { Padding = 14, Spacing = 10, Children =
            {
                new Label { Text = "Touche un objet pour le mettre dans la valise", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.White },
                _objects
            }}
        }}};
        BuildObjects();
    }

    public static PackingMissionPage ForZayd() => new("Mission · Valise de Zayd", "Zayd", "mission_pack_zayd.jpg", "home_pack_zayd",
    [
        new("👕", "un tee-shirt", "een T-shirt", true), new("👖", "un pantalon", "een broek", true),
        new("📖", "un livre", "een boek", true), new("🪥", "une brosse à dents", "een tandenborstel", true),
        new("🧦", "des chaussettes", "sokken", true), new("🥤", "une gourde", "een drinkfles", true),
        new("🛩️", "un avion jouet", "een speelgoedvliegtuig", false), new("🥾", "une grosse botte", "een grote laars", false)
    ]);

    public static PackingMissionPage ForRazan() => new("Mission · Valise de Razan", "Razan", "mission_pack_razan.jpg", "home_pack_razan",
    [
        new("👚", "un tee-shirt rose", "een roze T-shirt", true), new("👖", "un pantalon bleu", "een blauwe broek", true),
        new("📕", "un livre sur Paris", "een boek over Parijs", true), new("📷", "un appareil photo", "een fototoestel", true),
        new("👒", "un chapeau", "een hoed", true), new("🪥", "une brosse à dents", "een tandenborstel", true),
        new("🧸", "un gros nounours", "een grote knuffel", false), new("🧥", "un manteau trop lourd", "een te zware jas", false)
    ]);

    public static PackingMissionPage ForDocuments() => new("Mission · Documents", "Zayd et Razan", "scene_home.jpg", "home_documents",
    [
        new("🛂", "les passeports", "de paspoorten", true), new("🎫", "les billets", "de tickets", true),
        new("🗺️", "le plan de Paris", "de kaart van Parijs", true), new("📒", "le carnet de voyage", "het reisboekje", true),
        new("🎮", "la console", "de spelconsole", false), new("⚽", "le ballon", "de bal", false)
    ]);

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(350);
        await SpeakAsync($"Aide {_hero}. Choisis les objets utiles pour le voyage.");
    }

    private void BuildObjects()
    {
        UpdateHud();
        foreach (var item in _items.OrderBy(_ => Random.Shared.Next()))
        {
            var button = new Button
            {
                Text = $"{item.Emoji}  {item.French}\n{item.Dutch}", FontSize = 15, CornerRadius = 17,
                BackgroundColor = Colors.White, TextColor = Color.FromArgb("#172554"), Margin = 5,
                Padding = new Thickness(13, 8), FontAttributes = FontAttributes.Bold
            };
            button.Clicked += async (_, _) => await SelectItem(item, button);
            _objects.Add(button);
        }
    }

    private async Task SelectItem(Item item, Button button)
    {
        if (_busy || _packed.Contains(item.French)) return;
        _busy = true;
        if (!item.Useful)
        {
            button.BackgroundColor = Color.FromArgb("#FCA5A5");
            await GameFeedback.FailureAsync();
            await SpeakAsync($"Non, pas {item.French}. Cherche un objet utile.");
            await button.TranslateTo(-12, 0, 70); await button.TranslateTo(12, 0, 70); await button.TranslateTo(0, 0, 70);
            button.BackgroundColor = Colors.White;
            _busy = false;
            return;
        }

        _packed.Add(item.French);
        button.IsEnabled = false;
        button.BackgroundColor = Color.FromArgb("#86EFAC");
        await GameFeedback.SuccessAsync();
        await SpeakAsync($"Bravo ! {item.French}.");
        await button.ScaleTo(.65, 180, Easing.CubicIn);
        await button.TranslateTo(0, -120, 350, Easing.CubicIn);
        button.IsVisible = false;
        _suitcase.Text += item.Emoji + " ";
        UpdateHud();

        if (_packed.Count == _items.Count(x => x.Useful))
        {
            Preferences.Default.Set(_preferenceKey, true);
            var stars = Preferences.Default.Get("stars", 0) + 1;
            Preferences.Default.Set("stars", stars);
            await SpeakAsync($"Mission réussie ! La valise de {_hero} est prête.");
            await DisplayAlert("⭐ Mission réussie !", $"La valise de {_hero} est prête. Tu gagnes une étoile !", "Continuer");
            await Navigation.PopAsync();
        }
        _busy = false;
    }

    private void UpdateHud()
    {
        var target = _items.Count(x => x.Useful);
        _instruction.Text = $"Prépare la valise de {_hero}";
        _counter.Text = $"{_packed.Count}/{target}";
    }

    private static async Task SpeakAsync(string text)
    {
        try
        {
            var locales = await TextToSpeech.Default.GetLocalesAsync();
            var french = locales.FirstOrDefault(x => x.Language.StartsWith("fr", StringComparison.OrdinalIgnoreCase));
            await TextToSpeech.Default.SpeakAsync(text, new SpeechOptions { Locale = french });
        }
        catch { }
    }
}
