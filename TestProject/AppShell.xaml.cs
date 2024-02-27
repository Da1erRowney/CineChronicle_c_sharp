namespace TestProject;

public partial class AppShell : Shell
{
    public string UserChoice;

    public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("AllContentPage", typeof(AllContentPage));
        Routing.RegisterRoute("AboutPage", typeof(MainPage));

    }



    private async void OnMainPageClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new MainPage());
    }

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
    private async void OnDocumentalClicked(object sender, EventArgs e)
    {
        UserChoice = "Documental";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }
    private async void OnViewedClicked(object sender, EventArgs e)
    {
        UserChoice = "Viewed";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnInProgressClicked(object sender, EventArgs e)
    {
        UserChoice = "Progress";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnNotStartedClicked(object sender, EventArgs e)
    {
        UserChoice = "NotStarted";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private void OnUserStatisticsClicked(object sender, EventArgs e)
    {

    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {

    }

    private async void AddMoreContentPageClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddMoreContentPage());
    }
}

