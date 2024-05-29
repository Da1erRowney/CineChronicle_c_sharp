using DataContent;
using System.Net;

namespace TestProject;

public partial class InformationPage : ContentPage
{
	public InformationPage()
	{
		InitializeComponent();
        CheckedAuthUser();

    }
    public void CheckedAuthUser()
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            Email.Text= authUser.Email;
        }
    }

    private async void AuthButton_Clicked(object sender, EventArgs e)
    {
        AuthorizationPage authorization = new AuthorizationPage();

        await Navigation.PushAsync(authorization);
    }
}