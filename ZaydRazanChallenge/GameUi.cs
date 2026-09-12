namespace ZaydRazanChallenge;

public static class GameUi
{
    public static Image OfficialCharacters(double width = 310) => new()
    {
        Source = "zayd_razan_official.png",
        Aspect = Aspect.AspectFit,
        WidthRequest = width,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.End,
        InputTransparent = true
    };

    public static void AddHomeButton(ContentPage page)
    {
        page.ToolbarItems.Add(new ToolbarItem
        {
            Text = "✕ Start",
            Order = ToolbarItemOrder.Primary,
            Command = new Command(async () => await page.Navigation.PopToRootAsync())
        });
    }
}

public static class GameFeedback
{
    public static Task SuccessAsync() => PlayAsync(true);
    public static Task FailureAsync() => PlayAsync(false);

    private static async Task PlayAsync(bool success)
    {
#if ANDROID
        using var player = new Android.Media.ToneGenerator(Android.Media.Stream.Music, 90);
        player.StartTone(
            success ? Android.Media.Tone.PropAck : Android.Media.Tone.PropNack,
            success ? 260 : 420);
        await Task.Delay(success ? 280 : 440);
#else
        await Task.CompletedTask;
#endif
    }
}
