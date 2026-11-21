namespace ZaydRazanChallenge;

public sealed class BrusselsPuzzlePage : ContentPage
{
    private static readonly string[] Solution = ["🚉", "💧", "🧇", "🌍", "⚛️", "🏛️", "🚗", "🌳", "⭐"];
    private readonly string[] _tiles = Solution.OrderBy(_ => Random.Shared.Next()).ToArray();
    private readonly Grid _grid = new() { RowSpacing = 7, ColumnSpacing = 7 };
    private readonly Label _message = new() { FontSize = 17, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Color.FromArgb("#17324D") };
    private int _selected = -1;
    private bool _solved;

    public BrusselsPuzzlePage()
    {
        Title = "Puzzle Bruxelles";
        BackgroundColor = Color.FromArgb("#EAF8FF");
        GameUi.AddHomeButton(this);
        for (var i = 0; i < 3; i++)
        {
            _grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        var instructions = new Label
        {
            Text = "🧩 Remets les étapes du voyage dans le bon ordre.",
            FontSize = 19,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            TextColor = Color.FromArgb("#17324D")
        };
        var layout = new VerticalStackLayout
        {
            Padding = 16,
            Spacing = 14,
            Children = { instructions, _grid, _message }
        };
        Content = layout;
        Render();
    }

    private void Render()
    {
        _grid.Clear();
        for (var i = 0; i < _tiles.Length; i++)
        {
            var index = i;
            var button = new Button
            {
                Text = _tiles[i],
                FontSize = 32,
                BackgroundColor = _selected == index ? Color.FromArgb("#F4B942") : Colors.White,
                TextColor = Color.FromArgb("#17324D"),
                CornerRadius = 18,
                Margin = 2
            };
            button.Clicked += (_, _) => Select(index);
            _grid.Add(button, i % 3, i / 3);
        }
        _message.Text = _solved ? "🏆 Puzzle terminé ! +3 ⭐" : _selected < 0 ? "Choisis deux cases pour les échanger." : "Choisis la deuxième case.";
    }

    private void Select(int index)
    {
        if (_solved) return;
        if (_selected < 0)
        {
            _selected = index;
        }
        else if (_selected == index)
        {
            _selected = -1;
        }
        else
        {
            (_tiles[_selected], _tiles[index]) = (_tiles[index], _tiles[_selected]);
            _selected = -1;
            if (_tiles.SequenceEqual(Solution))
            {
                _solved = true;
                AdventureSave.Set("stars", AdventureSave.Get("stars", 0) + 3);
                _message.Text = "🏆 Puzzle terminé ! +3 ⭐";
            }
        }
        Render();
    }
}
