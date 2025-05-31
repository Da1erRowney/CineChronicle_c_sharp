using CineChronicle.Application.SupportClass;
namespace TestProject;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();
        UseNewBackground();

        // Инициализация переключателя с учетом всех возможных сценариев
        ThemeSwitch.IsToggled = ShouldUseDarkTheme();
    }

    private void UseNewBackground()
    {
        Random _random = new Random();
        string randomImage = $"{BackgroundImages._backgroundImages[_random.Next(0, BackgroundImages._backgroundImages.Length)]}.jpg";
        Background.Source = randomImage;
    }

    private bool ShouldUseDarkTheme()
    {
        // 1. Если тема явно задана в приложении - используем её
        if (Application.Current.UserAppTheme != AppTheme.Unspecified)
        {
            return Application.Current.UserAppTheme == AppTheme.Dark;
        }

        // 2. Иначе используем системную тему
        return Application.Current.PlatformAppTheme == AppTheme.Dark;
    }

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        // Устанавливаем явную тему (перестаём следовать системной)
        Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set("DarkTheme", e.Value);
    }

    private void OnVideoToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Set("ShowVideos", e.Value);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Обновляем состояние при каждом появлении страницы
        ThemeSwitch.IsToggled = ShouldUseDarkTheme();
        VideoSwitch.IsToggled = Preferences.Get("ShowVideos", true);
    }
}