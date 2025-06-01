using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
namespace TestProject;

public partial class SettingPage : ContentPage
{
    private static DatabaseServiceContent _databaseService;
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
        string randomImage = $"{BackgroundImages.GetImageOfTheme(Application.Current.UserAppTheme, Application.Current.PlatformAppTheme)[_random.Next(0, BackgroundImages.GetImageOfTheme(Application.Current.UserAppTheme, Application.Current.PlatformAppTheme).Length)]}.jpg";
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
        UseNewBackground();
    }

    private void OnVideoToggled(object sender, ToggledEventArgs e)
    {
        Preferences.Set("ShowVideos", e.Value);
        _databaseService = new DatabaseServiceContent(CineChronicle.Application.DeviceInfo._databasePath);
        var setting = _databaseService.GetUserSettingById(CineChronicle.Application.DeviceInfo.UserId);
        if (setting == null) return;
        setting.IsVideoBackground = Preferences.Get("ShowVideos", true);
        _databaseService.UpdateUserSetting(setting);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Обновляем состояние при каждом появлении страницы
        ThemeSwitch.IsToggled = ShouldUseDarkTheme();

        _databaseService = new DatabaseServiceContent(CineChronicle.Application.DeviceInfo._databasePath);
        var setting = _databaseService.GetUserSettingById(CineChronicle.Application.DeviceInfo.UserId);
        if (setting != null)
        {
            if (setting.IsVideoBackground)
            {
                Preferences.Set("ShowVideos", true);
                VideoSwitch.IsToggled = Preferences.Get("ShowVideos", true);
            }
            else
            {
                Preferences.Set("ShowVideos", false);
                VideoSwitch.IsToggled = Preferences.Get("ShowVideos", false);
            }
        }
        else
        {
            VideoSwitch.IsToggled = Preferences.Get("ShowVideos", true);
        }
    }
}