namespace ZaydRazanChallenge;

public sealed class PuzzlePage : ContentPage
{
    private readonly PuzzleDefinition _puzzle;
    private readonly int _gridSize;
    private readonly int _maxMoves;
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
        _gridSize = Random.Shared.Next(3, 7);
        _maxMoves = Random.Shared.Next(7, 21);
        _tiles = Enumerable.Range(0, _gridSize * _gridSize).ToList();

        var display = DeviceDisplay.Current.MainDisplayInfo;
        _boardSize = Math.Min(390d, Math.Max(280d, display.Width / display.Density - 44d));

        Title = "Verrassingspuzzel";
        BackgroundColor = Color.FromArgb("#FFF8E7");
        GameUi.AddHomeButton(this);

        for (var i = 0; i < _gridSize; i++)
        {
            _puzzleGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            _puzzleGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }
        _puzzleGrid.WidthRequest = _boardSize;
        _puzzleGrid.HeightRequest = _boardSize;

        var newChallengeButton = new Button
        {
            Text = "🎲 Andere willekeurige puzzel",
            BackgroundColor = Color.FromArgb("#0891B2"),
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold
        };
        newChallengeButton.Clicked += async (_, _) =>
            await Navigation.PushAsync(new PuzzlePage(PuzzleCatalog.GetRandom(_puzzle.Key)));

        var preview = new Image
        {
            Source = $"{puzzle.Key}_puzzle.jpg",
            HeightRequest = 125,
            Aspect = Aspect.AspectFit
        };

        var threeStarLimit = Math.Max(1, (int)Math.Floor(_maxMoves * .50));
        var twoStarLimit = Math.Max(threeStarLimit + 1, (int)Math.Floor(_maxMoves * .75));
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
        scoreRules.Add(CreateRule("⭐⭐⭐", $"≤ {threeStarLimit}", "#FEF3C7"), 0);
        scoreRules.Add(CreateRule("⭐⭐", $"≤ {twoStarLimit}", "#E0F2FE"), 1);
        scoreRules.Add(CreateRule("⭐", $"≤ {_maxMoves}", "#DCFCE7"), 2);

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
            Text = $"{puzzle.Emoji} {puzzle.Title} · {_gridSize}×{_gridSize}",
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
                        Text = "🎲 Défi culturel surprise",
                        FontSize = 25,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label
                    {
                        Text = $"Image, grille et limite choisies au hasard · maximum {_maxMoves} déplacements",
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
                    newChallengeButton
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
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            }
        };

    private void Shuffle()
    {
        _moves = 0;
        _completed = false;
        _selectedPosition = null;
        _messageLabel.TextColor = Color.FromArgb("#334155");
        _messageLabel.Text = $"Nog {_maxMoves} zetten";

        var minimumShuffle = Math.Max(3, (int)Math.Ceiling(_maxMoves * .35));
        var maximumShuffle = Math.Max(minimumShuffle, (int)Math.Floor(_maxMoves * .60));
        var shuffleMoves = Random.Shared.Next(minimumShuffle, maximumShuffle + 1);

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
        _movesLabel.Text = $"{_moves}/{_maxMoves}";
        var tileSize = _boardSize / _gridSize;

        for (var position = 0; position < _tiles.Count; position++)
        {
            var currentPosition = position;
            var tileNumber = _tiles[position];
            var sourceColumn = tileNumber % _gridSize;
            var sourceRow = tileNumber / _gridSize;

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
            _puzzleGrid.Add(tileBorder, position % _gridSize, position / _gridSize);
        }
    }

    private void SelectOrSwap(int position)
    {
        if (_completed) return;

        if (_selectedPosition is null)
        {
            _selectedPosition = position;
            _messageLabel.Text = "Kies het tweede blok.";
            RenderPuzzle();
            return;
        }

        var source = _selectedPosition.Value;
        _selectedPosition = null;
        if (source != position)
            Swap(source, position);
        else
        {
            _messageLabel.Text = $"Nog {_maxMoves - _moves} zetten";
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

        var remaining = _maxMoves - _moves;
        if (remaining > 0)
        {
            _messageLabel.TextColor = remaining <= 3
                ? Color.FromArgb("#DC2626")
                : Color.FromArgb("#334155");
            _messageLabel.Text = $"Nog {remaining} zet{(remaining == 1 ? "" : "ten")}";
            return;
        }

        _completed = true;
        await GameFeedback.FailureAsync();
        _messageLabel.TextColor = Color.FromArgb("#DC2626");
        _messageLabel.Text = "Geen zetten meer. Probeer opnieuw!";
        var retry = await DisplayAlert(
            "⏱️ Limiet bereikt",
            $"Je hebt alle {_maxMoves} zetten gebruikt. Wil je deze afbeelding opnieuw mengen?",
            "Opnieuw",
            "Andere afbeelding");
        if (retry)
            Shuffle();
        else
            await Navigation.PushAsync(new PuzzlePage(PuzzleCatalog.GetRandom(_puzzle.Key)));
    }

    private async Task CompletePuzzle()
    {
        _completed = true;
        await GameFeedback.SuccessAsync();
        var ratio = (double)_moves / _maxMoves;
        var reward = ratio <= .50 ? 3 : ratio <= .75 ? 2 : 1;
        var stars = Preferences.Default.Get("stars", 0) + reward;
        Preferences.Default.Set("stars", stars);
        Preferences.Default.Set($"puzzle_completed_{_puzzle.Key}", true);

        var bestKey = $"puzzle_best_{_puzzle.Key}";
        var previousBest = Preferences.Default.Get(bestKey, 0);
        if (reward > previousBest)
            Preferences.Default.Set(bestKey, reward);

        var starText = new string('⭐', reward);
        _messageLabel.TextColor = Color.FromArgb("#16A34A");
        _messageLabel.Text = $"Goed gedaan! {_puzzle.FrenchName} {starText}";

        await _puzzleGrid.ScaleTo(1.04, 180, Easing.CubicOut);
        await _puzzleGrid.ScaleTo(1, 180, Easing.CubicIn);

        await Navigation.PushAsync(
            new PuzzleDiscoveryPage(_puzzle, _moves, _maxMoves, reward));
    }

    private bool IsSolved() =>
        _tiles.Select((tile, position) => tile == position).All(correct => correct);
}

public sealed record PuzzleDefinition(
    string Key,
    string Title,
    string FrenchName,
    string Emoji,
    string Description);

public static class PuzzleCatalog
{
    public static IReadOnlyList<PuzzleDefinition> Items { get; } =
    [
        new("eiffel", "de Eiffeltoren", "La tour Eiffel", "🗼",
            "La tour Eiffel werd gebouwd voor de Wereldtentoonstelling van 1889. In het Frans zegt men « la tour Eiffel »."),
        new("louvre", "het Louvre", "Le Louvre", "🔺",
            "Le Louvre is een beroemd museum in Parijs. Je vindt er duizenden kunstwerken uit de hele wereld."),
        new("arc", "de Arc de Triomphe", "L’Arc de Triomphe", "🏛️",
            "L’Arc de Triomphe staat aan het einde van de Champs-Élysées en eert belangrijke momenten uit de Franse geschiedenis."),
        new("notre_dame", "Notre-Dame", "Notre-Dame de Paris", "⛪",
            "Notre-Dame is een middeleeuwse kathedraal op het Île de la Cité, in het hart van Parijs."),
        new("sacre_coeur", "Sacré-Cœur", "Le Sacré-Cœur", "🤍",
            "Le Sacré-Cœur staat op de heuvel van Montmartre en biedt een prachtig uitzicht over Parijs."),
        new("pantheon", "het Panthéon", "Le Panthéon", "🏛️",
            "Le Panthéon is een groot monument waar beroemde Franse schrijvers, wetenschappers en helden worden geëerd."),
        new("invalides", "Les Invalides", "Les Invalides", "✨",
            "Les Invalides herken je aan de gouden koepel. Het gebouw vertelt veel over de militaire geschiedenis van Frankrijk."),
        new("opera", "Opéra Garnier", "L’Opéra Garnier", "🎭",
            "L’Opéra Garnier is een rijk versierd theater voor opera en ballet, gebouwd in de negentiende eeuw."),
        new("joconde", "de Mona Lisa", "La Joconde", "🖼️",
            "La Joconde werd geschilderd door Leonardo da Vinci. Het wereldberoemde portret hangt in het Louvre."),
        new("croissant", "de croissant", "Le croissant", "🥐",
            "Le croissant is een luchtig Frans gebak met veel dunne laagjes. Het wordt vaak bij het ontbijt gegeten."),
        new("baguette", "het stokbrood", "La baguette", "🥖",
            "La baguette is een lang, knapperig Frans brood. De Franse kennis en traditie rond baguettebrood zijn beroemd."),
        new("versailles", "het paleis van Versailles", "Le château de Versailles", "👑",
            "Le château de Versailles was een koninklijk paleis. Het is bekend om zijn Spiegelzaal en grote Franse tuinen.")
    ];

    public static PuzzleDefinition GetRandom(string? excludedKey = null)
    {
        var choices = Items.Where(x => x.Key != excludedKey).ToArray();
        return choices[Random.Shared.Next(choices.Length)];
    }
}
