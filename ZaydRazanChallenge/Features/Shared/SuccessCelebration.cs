namespace ZaydRazanChallenge.Features.Shared;

/// <summary>Shows falling stars and confetti over the current page, then returns when the celebration ends.</summary>
public static class SuccessCelebration
{
    private static readonly Color[] ConfettiColors =
    [
        Color.FromArgb("#F43F5E"), Color.FromArgb("#F59E0B"), Color.FromArgb("#22C55E"),
        Color.FromArgb("#3B82F6"), Color.FromArgb("#A855F7"), Color.FromArgb("#06B6D4")
    ];

    public static async Task ShowAsync(ContentPage page)
    {
        var overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#220B1830"),
            Opacity = 0
        };

        // Lightweight paper pieces fall across the existing screen from its top edge.
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
        await confettiTask;

        await overlay.FadeTo(0, 160, Easing.CubicIn);

        // Detach the original content from the temporary host before assigning it
        // back to the ContentPage; MAUI elements cannot have two parents.
        if (original is not null)
            host.Children.Remove(original);
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
