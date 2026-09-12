namespace ZaydRazanChallenge;

internal static class GameUi
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
            Text = "🏠",
            Command = new Command(async () => await page.Navigation.PopToRootAsync())
        });
    }
}

internal static class GameFeedback
{
    public static async Task SuccessAsync()
    {
        try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
        catch { }
        await Task.Delay(80);
    }

    public static async Task FailureAsync()
    {
        try { HapticFeedback.Default.Perform(HapticFeedbackType.LongPress); }
        catch { }
        await Task.Delay(80);
    }
}
