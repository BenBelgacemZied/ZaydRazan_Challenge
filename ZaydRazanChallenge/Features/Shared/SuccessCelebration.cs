namespace ZaydRazanChallenge.Features.Shared;

/// <summary>Shows a reusable celebration with stars and colorful confetti falling from above.</summary>
public static class SuccessCelebration
{
    private static readonly Color[] ConfettiColors =
    [
        Color.FromArgb("#F43F5E"), Color.FromArgb("#F59E0B"), Color.FromArgb("#22C55E"),
        Color.FromArgb("#3B82F6"), Color.FromArgb("#A855F7"), Color.FromArgb("#06B6D4")
    ];

    public static async Task ShowAsync(ContentPage page, string title, string message,
        string rewardEmoji, string continueText = "Verder")
    {
        var close = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#990B1830"),
            Padding = 22,
            Opacity = 0
        };

        var continueButton = new Button
        {
            Text = continueText,
            FontSize = 19,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#0891B2"),
            TextColor = Colors.White,
            CornerRadius = 22
        };
        continueButton.Clicked += (_, _) => close.TrySetResult();

        // Lightweight paper pieces sit behind the card and fall from the top edge.
        var confetti = new List<View>();
        for (var i = 0; i < 28; i++)
        {
            View piece;
            if (i % 4 == 0)
            {
                piece = new Label
                {
                    Text = i % 8 == 0 ? "⭐" : "✨",
                    FontSize = Random.Shared.Next(18, 29),
                    WidthRequest = 30,
                    HeightRequest = 34,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
            }
            else
            {
                piece = new Border
                {
                    WidthRequest = Random.Shared.Next(8, 14),
                    HeightRequest = Random.Shared.Next(14, 24),
                    BackgroundColor = ConfettiColors[Random.Shared.Next(ConfettiColors.Length)],
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 }
                };
            }

            piece.Opacity = 0;
            piece.Rotation = Random.Shared.Next(-40, 41);
            piece.HorizontalOptions = LayoutOptions.Start;
            piece.VerticalOptions = LayoutOptions.Start;
            confetti.Add(piece);
            overlay.Children.Add(piece);
        }

        var card = new Border
        {
            Padding = new Thickness(22, 26),
            BackgroundColor = Color.FromArgb("#FFFFF8E7"),
            Stroke = Color.FromArgb("#FFFCD34D"),
            StrokeThickness = 3,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 30 },
            VerticalOptions = LayoutOptions.Center,
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children =
                {
                    new Label { Text = "🎉  ✨  🎉", FontSize = 27, HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = title, FontSize = 25, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = message, FontSize = 18, TextColor = Color.FromArgb("#475569"), HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = rewardEmoji, FontSize = 54, HorizontalTextAlignment = TextAlignment.Center },
                    continueButton
                }
            }
        };
        overlay.Children.Add(card);

        var original = page.Content;
        var host = new Grid();
        if (original is not null)
        {
            page.Content = null;
            host.Children.Add(original);
        }
        host.Children.Add(overlay);
        page.Content = host;

        await overlay.FadeTo(1, 220, Easing.CubicOut);
        var confettiTask = AnimateConfettiAsync(overlay, confetti);
        await Task.WhenAll(confettiTask, close.Task);

        await overlay.FadeTo(0, 160, Easing.CubicIn);
        page.Content = original;
    }

    private static async Task AnimateConfettiAsync(Grid overlay, IReadOnlyList<View> pieces)
    {
        // Give MAUI a layout pass so the pieces can be spread across the screen width.
        await Task.Yield();
        var width = Math.Max(overlay.Width, 320);
        var height = Math.Max(overlay.Height, 500);
        var animations = pieces.Select(async piece =>
        {
            var x = Random.Shared.NextDouble() * Math.Max(1, width - 32);
            piece.Margin = new Thickness(x, 0, 0, 0);
            piece.TranslationY = -45;
            await Task.Delay(Random.Shared.Next(0, 650));
            piece.Opacity = 1;
            var duration = (uint)Random.Shared.Next(1500, 2400);
            await Task.WhenAll(
                piece.TranslateTo(Random.Shared.Next(-45, 46), height + 70, duration, Easing.SinIn),
                piece.RotateTo(piece.Rotation + Random.Shared.Next(220, 700), duration, Easing.Linear),
                piece.FadeTo(0, duration, Easing.CubicIn));
        });
        await Task.WhenAll(animations);
    }
}
