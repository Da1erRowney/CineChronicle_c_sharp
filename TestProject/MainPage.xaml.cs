namespace TestProject;

public partial class MainPage : ContentPage
{


	public MainPage()
	{
		InitializeComponent();
	}

	

    private void OnAllClicked(object sender, EventArgs e)
    {

    }

    private void OnMoviesClicked(object sender, EventArgs e)
    {

    }

    private void OnSeriesClicked(object sender, EventArgs e)
    {

    }

    private void OnAnimeClicked(object sender, EventArgs e)
    {

    }

    private void OnOthersClicked(object sender, EventArgs e)
    {

    }

    private void OnAddMoreClicked(object sender, EventArgs e)
    {

    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {

    }

    private async void OnSidebarToggle(object sender, EventArgs e)
    {
        if (Sidebar.IsVisible)
        {
            await Sidebar.FadeTo(1, 250); // Скрыть панель анимацией
            Sidebar.IsVisible = false;
            Sidebar.WidthRequest = 0; // Установить ширину панели в 0
        }
        else
        {
            Sidebar.IsVisible = true;
            await Sidebar.FadeTo(1, 250); // Показать панель анимацией
            Sidebar.WidthRequest = 200; // Установить желаемую ширину панели
        }
    }

}

