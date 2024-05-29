using DataContent;

namespace TestProject;

public partial class InformationPage : ContentPage
{
	public InformationPage()
	{
		InitializeComponent();
        UpdateAuthenticatedDate();
      
    }

    private void UpdateAuthenticatedDate()
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            Email.Text = $"Почта: {authUser.Email}";

        }
    }

    private async void AccountButton_Clicked(object sender, EventArgs e)
    {
        var authorizationPage = new AuthorizationPage();

        await Navigation.PushAsync(authorizationPage);
    }
}