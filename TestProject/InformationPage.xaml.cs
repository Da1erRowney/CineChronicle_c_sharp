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

    public async void CheckedAuthUser()
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            string userName = authUser.Email;
            int atIndex = userName.IndexOf('@');
            if (atIndex != -1)
            {
                userName = userName.Substring(0, atIndex);
            }
            
            EmailName.Text= userName;
            ButtonAuth.Text = "Сменить аккаунт";
            ButtonAuth.WidthRequest = 200;
            var getIcon = databaseService.GetUsereByEmail(authUser.Email);
            imageIcon.Source = getIcon.NameIcon;

        }
        else
        {
            EmailName.Text = "Пользователь отсутсвует";
            ButtonAuth.Text = "Авторизация";
            ButtonAuth.WidthRequest = 150;
        }
    }

    private async void AuthButton_Clicked(object sender, EventArgs e)
    {
        AuthorizationPage authorization = new AuthorizationPage();

        await Navigation.PushAsync(authorization);
    }
}