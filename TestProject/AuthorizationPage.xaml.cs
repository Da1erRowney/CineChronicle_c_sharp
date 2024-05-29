using DataContent;

namespace TestProject;

public partial class AuthorizationPage : ContentPage
{
	public AuthorizationPage()
	{
		InitializeComponent();
	}

    private void OnAddClicked(object sender, EventArgs e)
    {
        //....

        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);

        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            authUser.IsAuthenticated = false;
            databaseService.UpdateAuth(authUser);
        }

        var user = new User
        {
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };
        databaseService.InsertUser(user);

        var authenticated = new Authorized
        {
            Email = user.Email,
            IsAuthenticated = true
        };
        databaseService.InsertAuth(authenticated);

    }
}