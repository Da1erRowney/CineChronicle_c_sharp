namespace TestProject;

public partial class MainPage : ContentPage
{


	public MainPage()
	{
		InitializeComponent();
	}

    public string UserChoice;


    private async void OnAllClicked(object sender, EventArgs e)
    {
        UserChoice = "All";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }


    private async void OnMoviesClicked(object sender, EventArgs e)
    {
        UserChoice = "Movies";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnSeriesClicked(object sender, EventArgs e)
    {
        UserChoice = "Series";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnAnimeClicked(object sender, EventArgs e)
    {
        UserChoice = "Anime";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnOthersClicked(object sender, EventArgs e)
    {
        UserChoice = "Other";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }
    private async void OnDoramClicked(object sender, EventArgs e)
    {
        UserChoice = "Doram";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnMultSeriesClicked(object sender, EventArgs e)
    {
        UserChoice = "Mult";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
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

    private void OnUserStatisticsClicked(object sender, EventArgs e)
    {

    }

}

