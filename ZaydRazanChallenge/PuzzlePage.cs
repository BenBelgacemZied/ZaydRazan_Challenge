namespace ZaydRazanChallenge;

public sealed class PuzzlePage : ContentPage
{
    private readonly PuzzleDefinition _puzzle;
    private readonly Grid _puzzleGrid = new()
    {
        RowSpacing = 4,
        ColumnSpacing = 4,
        HorizontalOptions = LayoutOptions.Fill
    };
    private readonly Label _movesLabel = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold
    };
    private readonly Label _messageLabel = new()
    {
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly List<int> _tiles = Enumerable.Range(0, 9).ToList();
    private int _moves;
    private int? _selectedPosition;
    private bool _completed;

    public PuzzlePage(PuzzleDefinition puzzle)
    {
        _puzzle = puzzle;
        Title = puzzle.Title;
        BackgroundColor = Color.FromArgb("#FFF8E7");

        for (var i = 0; i < 3; i++)
        {
            _puzzleGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            _puzzleGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        var shuffleButton = new Button
        {
            Text = "🔀  Opnieuw mengen",
            BackgroundColor = Color.FromArgb("#0891B2")
        };
        shuffleButton.Clicked += (_, _) => Shuffle();

        var preview = new Image
        {
            Source = $"{puzzle.Key}_puzzle.jpg",
            HeightRequest = 150,
            Aspect = Aspect.AspectFit
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 18,
                Spacing = 15,
                Children =
                {
                    new Label
                    {
                        Text = $"🧩 Bouw {puzzle.Title}",
                        FontSize = 27,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Sleep een blok naar een ander blok. Je kunt ook twee blokken na elkaar aanraken.",
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Border
                    {
                        BackgroundColor = Color.FromArgb("#DBEAFE"),
                        StrokeThickness = 0,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
                        Padding = 8,
                        Content = preview
                    },
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto)
                        },
                        Children =
                        {
                            new Label { Text = $"{puzzle.Emoji} {puzzle.Title} · Parijs", FontSize = 17, FontAttributes = FontAttributes.Bold },
                            _movesLabel
                        }
                    },
                    new Border
                    {
                        BackgroundColor = Colors.White,
                        Stroke = Color.FromArgb("#0891B2"),
                        StrokeThickness = 3,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                        Padding = 4,
                        HeightRequest = 350,
                        Content = _puzzleGrid
                    },
                    _messageLabel,
                    shuffleButton
                }
            }
        };

        Grid.SetColumn(_movesLabel, 1);
        Shuffle();
    }

    private void Shuffle()
    {
        _moves = 0;
        _completed = false;
        _selectedPosition = null;
        _messageLabel.Text = "";

        do
        {
            for (var i = _tiles.Count - 1; i > 0; i--)
            {
                var j = Random.Shared.Next(i + 1);
                (_tiles[i], _tiles[j]) = (_tiles[j], _tiles[i]);
            }
        } while (IsSolved());

        RenderPuzzle();
    }

    private void RenderPuzzle()
    {
        _puzzleGrid.Clear();
        _movesLabel.Text = $"Beurten: {_moves}";

        for (var position = 0; position < _tiles.Count; position++)
        {
            var currentPosition = position;
            var tileNumber = _tiles[position];
            var tileBorder = new Border
            {
                Padding = 1,
                StrokeThickness = _selectedPosition == position ? 4 : 1,
                Stroke = _selectedPosition == position
                    ? Color.FromArgb("#F59E0B")
                    : Color.FromArgb("#FFFFFF"),
                Content = new Image
                {
                    Source = $"{_puzzle.Key}_{tileNumber}.jpg",
                    Aspect = Aspect.AspectFill,
                    InputTransparent = true
                }
            };

            var drag = new DragGestureRecognizer();
            drag.DragStarting += (_, e) =>
                e.Data.Properties["puzzle-position"] = currentPosition;
            tileBorder.GestureRecognizers.Add(drag);

            var drop = new DropGestureRecognizer { AllowDrop = true };
            drop.Drop += (_, e) =>
            {
                if (e.Data.Properties.TryGetValue("puzzle-position", out var value)
                    && value is int sourcePosition)
                {
                    Swap(sourcePosition, currentPosition);
                }
            };
            tileBorder.GestureRecognizers.Add(drop);

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => SelectOrSwap(currentPosition);
            tileBorder.GestureRecognizers.Add(tap);

            _puzzleGrid.Add(tileBorder, position % 3, position / 3);
        }
    }

    private void SelectOrSwap(int position)
    {
        if (_completed) return;
        if (_selectedPosition is null)
        {
            _selectedPosition = position;
            RenderPuzzle();
            return;
        }

        var source = _selectedPosition.Value;
        _selectedPosition = null;
        if (source != position)
            Swap(source, position);
        else
            RenderPuzzle();
    }

    private async void Swap(int sourcePosition, int targetPosition)
    {
        if (_completed || sourcePosition == targetPosition) return;

        (_tiles[sourcePosition], _tiles[targetPosition]) =
            (_tiles[targetPosition], _tiles[sourcePosition]);
        _selectedPosition = null;
        _moves++;
        RenderPuzzle();

        if (!IsSolved()) return;

        _completed = true;
        var completionKey = $"puzzle_completed_{_puzzle.Key}";
        var firstCompletion = !Preferences.Default.Get(completionKey, false);
        var reward = firstCompletion ? 3 : 1;
        Preferences.Default.Set(completionKey, true);
        var stars = Preferences.Default.Get("stars", 0) + reward;
        Preferences.Default.Set("stars", stars);
        _messageLabel.TextColor = Color.FromArgb("#16A34A");
        _messageLabel.Text = $"Bravo! {_puzzle.Title} is compleet. +{reward} ⭐";
        await _puzzleGrid.ScaleTo(1.04, 180, Easing.CubicOut);
        await _puzzleGrid.ScaleTo(1, 180, Easing.CubicIn);
        await DisplayAlert("🏆 Puzzel voltooid!",
            $"Je hebt de puzzel in {_moves} beurten opgelost en {reward} ster(ren) gewonnen.",
            "Super!");
    }

    private bool IsSolved() =>
        _tiles.Select((tile, position) => tile == position).All(correct => correct);
}

public sealed record PuzzleDefinition(string Key, string Title, string Emoji);

public static class PuzzleCatalog
{
    public static IReadOnlyList<PuzzleDefinition> Items { get; } =
    [
        new("eiffel", "de Eiffeltoren", "🗼"),
        new("louvre", "het Louvre", "🔺"),
        new("arc", "de Arc de Triomphe", "🏛️"),
        new("notre_dame", "Notre-Dame", "⛪"),
        new("sacre_coeur", "Sacré-Cœur", "🤍"),
        new("pantheon", "het Panthéon", "🏛️"),
        new("invalides", "Les Invalides", "✨"),
        new("opera", "Opéra Garnier", "🎭")
    ];
}
