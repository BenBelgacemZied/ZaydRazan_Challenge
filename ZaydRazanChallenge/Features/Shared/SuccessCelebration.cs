namespace ZaydRazanChallenge.Features.Shared;

/// <summary>A short, reusable celebration with an activity-specific animated reward.</summary>
public static class SuccessCelebration
{
    public static async Task ShowAsync(ContentPage page, string title, string message,
        string rewardEmoji, string rewardLabel, string continueText = "Verder")
    {
        var close = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#990B1830"),
            Padding = 22,
            Opacity = 0,
            RowDefinitions = { new RowDefinition(GridLength.Star) }
        };

        var reward = new Label
        {
            Text = rewardEmoji,
            FontSize = 72,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            Scale = .25,
            Rotation = -18
        };
        var rewardButton = new Button
        {
            Text = rewardLabel,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#FEF3C7"),
            TextColor = Color.FromArgb("#78350F"),
            CornerRadius = 18,
            IsEnabled = false,
            Opacity = 0
        };
        var continueButton = new Button
        {
            Text = continueText,
            FontSize = 19,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Color.FromArgb("#0891B2"),
            TextColor = Colors.White,
            CornerRadius = 22,
            IsEnabled = false,
            Opacity = 0
        };

        rewardButton.Clicked += async (_, _) =>
        {
            rewardButton.IsEnabled = false;
            await reward.ScaleTo(1.28, 130, Easing.CubicOut);
            await reward.ScaleTo(1, 180, Easing.BounceOut);
            reward.Rotation = 12;
            await reward.RotateTo(0, 220, Easing.SpringOut);
            continueButton.IsEnabled = true;
            await continueButton.FadeTo(1, 180);
        };
        continueButton.Clicked += (_, _) => close.TrySetResult();

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
                Spacing = 12,
                Children =
                {
                    new Label { Text = "✨  🎉  ✨", FontSize = 25, HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = title, FontSize = 25, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#17324D"), HorizontalTextAlignment = TextAlignment.Center },
                    new Label { Text = message, FontSize = 18, TextColor = Color.FromArgb("#475569"), HorizontalTextAlignment = TextAlignment.Center },
                    reward,
                    rewardButton,
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
        await reward.ScaleTo(1, 360, Easing.BounceOut);
        await reward.RotateTo(0, 220, Easing.SpringOut);
        rewardButton.IsEnabled = true;
        await rewardButton.FadeTo(1, 180);
        await close.Task;

        await overlay.FadeTo(0, 160, Easing.CubicIn);
        page.Content = original;
    }
}
