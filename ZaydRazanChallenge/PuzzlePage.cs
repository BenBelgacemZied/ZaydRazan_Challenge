namespace ZaydRazanChallenge;

public sealed class PuzzlePage : ContentPage
{
    private const int MaxMoves = 10;
    private readonly PuzzleDefinition _puzzle;
    private readonly Grid _puzzleGrid = new()
    {
        RowSpacing = 1,
        ColumnSpacing = 1,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Label _movesLabel = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.End
    };
    private readonly Label _messageLabel = new()
    {
        FontSize = 17,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly List<int> _tiles;
    private readonly double _boardSize;
    private int _moves;
    private int? _selectedPosition;
    private bool _completed;

    public PuzzlePage(PuzzleDefinition puzzle)
    {
        _puzzle = puzzle;
        _tiles = Enumerable.Range(0, puzzle.GridSize * puzzle.GridSize).ToList();
        var display = DeviceDisplay.Current.MainDisplayInfo;
        _boardSize = Math.Min(390d, Math.Max(280d, display.Width / display.Density - 44d));

        Title = puzzle.Title;
        BackgroundColor = Color.FromArgb("#FFF8E7");

        for (var i = 0; i < puzzle.GridSize; i++)
        {
            _puzzleGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            _puzzleGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        _puzzleGrid.WidthRequest = _boardSize;
        _puzzleGrid.HeightRequest = _boardSize;

        var shuffleButton = new Button
        {
            Text = "🔀  Nieuwe uitdaging",
            BackgroundColor = Color.FromArgb("#0891B2"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold
        };
        shuffleButton.Clicked += (_, _) => Shuffle();

        var preview = new Image
        {
            Source = $"{puzzle.Key}_puzzle.jpg",
            HeightRequest = 125,
            Aspect = Aspect.AspectFit
        };

        var scoreRules = new Grid
        {
            ColumnSpacing = 6,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            }
        };
        scoreRules.Add(CreateRule("⭐⭐⭐", "0–4 zetten", "#FEF3C7"), 0);
        scoreRules.Add(CreateRule("⭐⭐", "5–7 zetten", "#E0F2FE"), 1);
        scoreRules.Add(CreateRule("⭐", "8–10 zetten", "#DCFCE7"), 2);

        var header = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        header.Add(new Label
        {
            Text = $"{puzzle.Emoji} {puzzle.Title} · {puzzle.GridSize}×{puzzle.GridSize}",
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            VerticalTextAlignment = TextAlignment.Center
        }, 0);
        header.Add(_movesLabel, 1);

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Spacing = 12,
                Children =
                {
                    new Label
                    {
                        Text = $"🧩 Bouw {puzzle.Title}",
                        FontSize = 25,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = "Tik twee blokken aan om ze te wisselen. Voltooi de puzzel in maximaal 10 zetten!",
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    scoreRules,
                    new Border
                    {
                        BackgroundColor = Color.FromArgb("#DBEAFE"),
                        StrokeThickness = 0,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                        Padding = 6,
                        Content = preview
                    },
                    header,
                    new Border
                    {
                        BackgroundColor = Colors.White,
                        Stroke = Color.FromArgb("#0891B2"),
                        StrokeThickness = 3,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
                        Padding = 3,
                        WidthRequest = _boardSize + 12,
                        HeightRequest = _boardSize + 12,
                        HorizontalOptions = LayoutOptions.Center,
                        Content = _puzzleGrid
                    },
                    _messageLabel,
                    shuffleButton
                }
            }
        };

        Shuffle();
    }

    private static Border CreateRule(string stars, string moves, string color) =>
        new()
        {
            BackgroundColor = Color.FromArgb(color),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            Padding = new Thickness(4, 7),
            Content = new VerticalStackLayout
            {
                Spacing = 1,
                Children =
                {
                    new Label
                    {
                        Text = stars,
                        FontSize = 15,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = moves,
                        FontSize = 12,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        };

    private void Shuffle()
    {
        for (var i = 0; i < _tiles.Count; i++)
            _tiles[i] = i;

        _moves = 0;
        _completed = false;
        _selectedPosition = null;
        _messageLabel.TextColor = Color.FromArgb("#334155");
        _messageLabel.Text = $"Nog {MaxMoves} zetten · kies twee blokken";

        // The challenge is produced with a reversible number of swaps, so even a
        // 6×6 board always remains solvable within the ten-move limit.
        var shuffleMoves = _puzzle.GridSize switch
        {
            4 => 4,
            5 => 7,
            _ => 9
        };

        do
        {
            for (var i = 0; i < _tiles.Count; i++)
                _tiles[i] = i;

            for (var move = 0; move < shuffleMoves; move++)
            {
                var first = Random.Shared.Next(_tiles.Count);
                int second;
                do second = Random.Shared.Next(_tiles.Count);
                while (second == first);

                (_tiles[first], _tiles[second]) = (_tiles[second], _tiles[first]);
            }
        } while (IsSolved());

        RenderPuzzle();
    }

    private void RenderPuzzle()
    {
        _puzzleGrid.Clear();
        _movesLabel.Text = $"Zetten: {_moves}/{MaxMoves}";
        var tileSize = _boardSize / _puzzle.GridSize;

        for (var position = 0; position < _tiles.Count; position++)
        {
            var currentPosition = position;
            var tileNumber = _tiles[position];
            var sourceColumn = tileNumber % _puzzle.GridSize;
            var sourceRow = tileNumber / _puzzle.GridSize;

            var image = new Image
            {
                Source = $"{_puzzle.Key}_puzzle.jpg",
                Aspect = Aspect.AspectFill,
                WidthRequest = _boardSize,
                HeightRequest = _boardSize,
                MinimumWidthRequest = _boardSize,
                MinimumHeightRequest = _boardSize,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                TranslationX = -sourceColumn * tileSize,
                TranslationY = -sourceRow * tileSize,
                InputTransparent = true
            };

            var tileViewport = new Grid
            {
                WidthRequest = tileSize,
                HeightRequest = tileSize,
                Clip = new Microsoft.Maui.Controls.Shapes.RectangleGeometry(
                    new Rect(0, 0, tileSize, tileSize)),
                BackgroundColor = Colors.White
            };
            tileViewport.Children.Add(image);

            var tileBorder = new Border
            {
                Padding = 0,
                WidthRequest = tileSize,
                HeightRequest = tileSize,
                StrokeThickness = _selectedPosition == position ? 4 : 1,
                Stroke = _selectedPosition == position
                    ? Color.FromArgb("#F59E0B")
                    : Colors.White,
                Content = tileViewport
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => SelectOrSwap(currentPosition);
            tileBorder.GestureRecognizers.Add(tap);

            _puzzleGrid.Add(tileBorder,
                position % _puzzle.GridSize,
                position / _puzzle.GridSize);
        }
    }

    private void SelectOrSwap(int position)
    {
        if (_completed) return;

        if (_selectedPosition is null)
        {
            _selectedPosition = position;
            _messageLabel.Text = "Kies nu het tweede blok";
            RenderPuzzle();
            return;
        }

        var source = _selectedPosition.Value;
        _selectedPosition = null;
        if (source != position)
            Swap(source, position);
        else
        {
            _messageLabel.Text = $"Nog {MaxMoves - _moves} zetten · kies twee blokken";
            RenderPuzzle();
        }
    }

    private async void Swap(int sourcePosition, int targetPosition)
    {
        if (_completed || sourcePosition == targetPosition) return;

        (_tiles[sourcePosition], _tiles[targetPosition]) =
            (_tiles[targetPosition], _tiles[sourcePosition]);
        _moves++;
        RenderPuzzle();

        if (IsSolved())
        {
            await CompletePuzzle();
            return;
        }

        var remaining = MaxMoves - _moves;
        if (remaining > 0)
        {
            _messageLabel.TextColor = remaining <= 2
                ? Color.FromArgb("#DC2626")
                : Color.FromArgb("#334155");
            _messageLabel.Text = $"Nog {remaining} zet{(remaining == 1 ? "" : "ten")}!";
            return;
        }

        _completed = true;
        _messageLabel.TextColor = Color.FromArgb("#DC2626");
        _messageLabel.Text = "Geen zetten meer. Probeer opnieuw!";
        var retry = await DisplayAlert(
            "⏱️ Uitdaging voorbij",
            "Je hebt de limiet van 10 zetten bereikt. Wil je een nieuwe puzzel proberen?",
            "Opnieuw",
            "Later");
        if (retry) Shuffle();
    }

    private async Task CompletePuzzle()
    {
        _completed = true;
        var reward = _moves <= 4 ? 3 : _moves <= 7 ? 2 : 1;
        var stars = Preferences.Default.Get("stars", 0) + reward;
        Preferences.Default.Set("stars", stars);

        var bestKey = $"puzzle_best_{_puzzle.Key}";
        var previousBest = Preferences.Default.Get(bestKey, 0);
        if (reward > previousBest)
            Preferences.Default.Set(bestKey, reward);

        var starText = new string('⭐', reward);
        _messageLabel.TextColor = Color.FromArgb("#16A34A");
        _messageLabel.Text = $"Bravo! {_puzzle.Title} is compleet. {starText}";

        await _puzzleGrid.ScaleTo(1.04, 180, Easing.CubicOut);
        await _puzzleGrid.ScaleTo(1, 180, Easing.CubicIn);
        await DisplayAlert(
            "🏆 Puzzel voltooid!",
            $"Opgelost in {_moves} zetten. Je wint {reward} ster{(reward == 1 ? "" : "ren")} {starText}",
            "Super!");
    }

    private bool IsSolved() =>
        _tiles.Select((tile, position) => tile == position).All(correct => correct);
}

public sealed record PuzzleDefinition(string Key, string Title, string Emoji, int GridSize);

public static class PuzzleCatalog
{
    public static IReadOnlyList<PuzzleDefinition> Items { get; } =
    [
        new("eiffel", "de Eiffeltoren", "🗼", 4),
        new("louvre", "het Louvre", "🔺", 4),
        new("arc", "de Arc de Triomphe", "🏛️", 4),
        new("notre_dame", "Notre-Dame", "⛪", 5),
        new("sacre_coeur", "Sacré-Cœur", "🤍", 5),
        new("pantheon", "het Panthéon", "🏛️", 5),
        new("invalides", "Les Invalides", "✨", 6),
        new("opera", "Opéra Garnier", "🎭", 6)
    ];
}
