using DataContent;

namespace TestProject;

public partial class AuthorizationPage : ContentPage
{
	public AuthorizationPage()
	{
		InitializeComponent();
	}

    private async void OnAddClicked(object sender, EventArgs e)
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        //....
        if (databaseService.GetUsereByEmail(EmailEntry.Text) !=null ) 
        {
            await DisplayAlert("Ошибка","Такой пользователь уже существует", "OK");
            return;
        }

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
        await DisplayAlert("Успех", "Аккаунт создан // вы вошли в аккаунт", "OK");
        await Navigation.PushAsync(new InformationPage());
    }

    private async void OnEntranceClicked(object sender, EventArgs e)
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        //....

        if (databaseService.GetUsereByEmail(EmailEntry.Text).Password != PasswordEntry.Text)
        {
            await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
            return;
        }

        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            authUser.IsAuthenticated = false;
            databaseService.UpdateAuth(authUser);
            var newAuthUser = databaseService.GetAuthorizedByEmail(EmailEntry.Text);
            newAuthUser.IsAuthenticated = true;

        databaseService.UpdateAuth(newAuthUser);
            await DisplayAlert("Успех", "Вы вошли в аккаунт", "OK");
            await Navigation.PushAsync(new InformationPage());
        }
    }
}