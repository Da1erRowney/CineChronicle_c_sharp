namespace TestProject;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        //MainPage = new AppShell();
        UpdateThemeIcons(); // Вызываем метод обновления иконок при запуске приложения

        // Подписываемся на событие изменения темы
        Application.Current.RequestedThemeChanged += (s, e) =>
        {
            UpdateThemeIcons(); // Вызываем метод обновления иконок при изменении темы
        };
    }

    // Метод для обновления иконок в зависимости от текущей темы
    private void UpdateThemeIcons()
    {
        if (Application.Current.UserAppTheme == AppTheme.Dark)
        {
            Main.Icon = ImageSource.FromFile("main_page_light.svg");
            Add.Icon = ImageSource.FromFile("add_more_content_page_light.svg");
            All.Icon = ImageSource.FromFile("all_content_page_light.svg");
            Information.Icon = ImageSource.FromFile("information_page_light.svg");
            Setting.Icon = ImageSource.FromFile("setting_page_light.svg");
        }
        else
        {
            Main.Icon = ImageSource.FromFile("main_page_dark.svg");
            Add.Icon = ImageSource.FromFile("add_more_content_page_dark.svg");
            All.Icon = ImageSource.FromFile("all_content_page_dark.svg");
            Information.Icon = ImageSource.FromFile("information_page_dark.svg");
            Setting.Icon = ImageSource.FromFile("setting_page_dark.svg");
        }
    }
}
