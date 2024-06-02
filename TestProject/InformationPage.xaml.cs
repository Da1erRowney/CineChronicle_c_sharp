#pragma warning disable CS4008
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


    public async Task CheckedAuthUser()
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
            InformationLayout.IsVisible = true;
            ButtonExit.IsVisible = true;
            NoteAuthAccountLayout.IsVisible = false;

        }
        else
        {
            InformationLayout.IsVisible = false;
            EmailName.Text = "Пользователь отсутствует";
            NoteAuthAccountLayout.IsVisible = true;
            imageIcon.Source  = "";
            ButtonExit.IsVisible = false;
            ButtonAuth.Text = "Авторизация";
            ButtonAuth.WidthRequest = 150;
        }
    }

    private async void AuthButton_Clicked(object sender, EventArgs e)
    {
        AuthorizationPage authorization = new AuthorizationPage();

        await Navigation.PushAsync(authorization);
    }

    private async void ExitAccountButton_Clicked(object sender, EventArgs e)
    {
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            var authUser = databaseService.GetAuthorizedByAuth(true);
            authUser.IsAuthenticated = false;
            databaseService.UpdateAuth(authUser);
            CheckedAuthUser();
            //await Navigation.PushAsync(new InformationPage());

    }

}