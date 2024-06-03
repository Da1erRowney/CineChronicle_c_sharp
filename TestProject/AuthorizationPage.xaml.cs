using DataContent;

using System.Text.RegularExpressions;

namespace TestProject;

public partial class AuthorizationPage : ContentPage
{
    private readonly InformationPage _informationPage;
    public AuthorizationPage()
	{
		InitializeComponent();
        CheckedAuthUser();
    }
    public AuthorizationPage(InformationPage informationPage)
    {
        InitializeComponent();
        _informationPage = informationPage;
    }

    public async void CheckedAuthUser()
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            string userName = authUser.Email;

        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if(Email1Entry.Text .Length == 0 && Password1Entry.Text.Length==0) 
        {
            await DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
            return;
        }

        string email = Email1Entry.Text.ToLower().TrimEnd();
        if (!ValidateEmail(email))
        {
            await DisplayAlert("Ошибка", "Неправильный формат почты", "OK");
            return;
        }

        if (databaseService.GetUsereByEmail(email) !=null ) 
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
            Email = email,
            Password = Password1Entry.Text,
            NameIcon = "defaulticon.jpg"
        };

        databaseService.InsertUser(user);

        var authenticated = new Authorized
        {
            Email = user.Email,
            IsAuthenticated = true
        };

        databaseService.InsertAuth(authenticated);
        await DisplayAlert("Успех", "Аккаунт создан, вы успешно вошли в аккаунт", "OK");

        ReturnAfterAuth();
    }

    private async void ReturnAfterAuth()
    {
        //InformationPage informationPage = new InformationPage();
        //await Navigation.PushAsync(informationPage);
        await _informationPage.CheckedAuthUser();
        await Navigation.PopModalAsync();

    }
    private async void OnEntranceClicked(object sender, EventArgs e)
    {

        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if (EmailEntry.Text.Length == 0 && PasswordEntry.Text.Length == 0)
        {
            await DisplayAlert("Ошибка", "Не все поля заполнены", "OK");
            return;
        }
        string email = EmailEntry.Text.ToLower().TrimEnd();
        if (databaseService.GetUsereByEmail(email) == null)
        {
            await DisplayAlert("Ошибка", "Введенной почты не существует", "Ок");
            return;
        }

        if (databaseService.GetUsereByEmail(email).Password != PasswordEntry.Text)
        {
            await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
            PasswordEntry.Text = "";
            return;
        }

        if (databaseService.GetAuthorizedByAuth(true) != null)
        {
            var authUser = databaseService.GetAuthorizedByAuth(true);
            authUser.IsAuthenticated = false;
            databaseService.UpdateAuth(authUser);
            var newAuthUser = databaseService.GetAuthorizedByEmail(email);
            newAuthUser.IsAuthenticated = true;

            databaseService.UpdateAuth(newAuthUser);
            await DisplayAlert("Успех", "Вы успешно вошли в аккаунт", "OK");

            ReturnAfterAuth();

        }
        else
        {
            var newAuthUser = databaseService.GetAuthorizedByEmail(email);
            newAuthUser.IsAuthenticated = true;

            databaseService.UpdateAuth(newAuthUser);
            await DisplayAlert("Успех", "Вы успешно вошли в аккаунт", "OK");

            ReturnAfterAuth();
        }
    }
    public static bool ValidateEmail(string email)
    {
        string emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";

        if (Regex.IsMatch(email, emailRegex))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCreateTapped(object sender, EventArgs e)
    {
        TitlePage.Text = "Начни жизнь с нового аккаунта...";
        CreateLayout.IsVisible = true;
        EntranceBorder.IsVisible = false;
     
    }

    private void OnReturnTapped(object sender, EventArgs e)
    {
        TitlePage.Text = "Мы вас ждали путник...";
        CreateLayout.IsVisible = false;
        EntranceBorder.IsVisible = true;

    }

    private void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        //Забыл пароль
    }
}