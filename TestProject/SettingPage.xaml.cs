namespace TestProject;

public partial class SettingPage : ContentPage
{
	public SettingPage()
	{
		InitializeComponent();
	}
    private async void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        bool isDarkTheme = e.Value;

        if (isDarkTheme)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;

        }
        else
        { 
            Application.Current.UserAppTheme = AppTheme.Light;
        }

    }
}