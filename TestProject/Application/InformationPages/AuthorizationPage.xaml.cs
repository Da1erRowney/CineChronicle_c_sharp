using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;

namespace TestProject;

public partial class AuthorizationPage : ContentPage
{
    public AuthorizationPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new ViewAuthPageModel();
        UseNewBackground();
    }
    private void UseNewBackground()
    {
        Random _random = new Random();
        string randomImage = $"{BackgroundImages._backgroundImages[_random.Next(0, BackgroundImages._backgroundImages.Length)]}.jpg";
        Background.Source = randomImage;
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

    // Создание
    private async void OnAddClicked(object sender, EventArgs e)
    {
        string message = ViewAuthPageModel.AddAccount(EmailEEntry.Text, PasswordEEntry.Text, NickNameEntry.Text);
        if (message != null)
        {
            switch (message)
            {
                case "Не все поля заполнены":
                case "Неправильный формат почты":
                case "Такой пользователь уже существует":
                case "Пароль меньше 8 символов. Придумайте пароль длинее":
                    await DisplayAlert("Ошибка", message, "Ок");
                    PasswordEntry.Text = "";
                    break;
                case "Аккаунт создан, вы успешно вошли в аккаунт":
                    await DisplayAlert("Успех", message, "Ок");
                    ReturnAfterAuth();
                    break;
                default:
                    break;
            }
        }
    }

    private async void ReturnAfterAuth()
    {
        // Переход к информации
        await Shell.Current.GoToAsync("//Information");
    }

    // Вход
    private async void OnEntranceClicked(object sender, EventArgs e)
    {
        string message = ViewAuthPageModel.CheckValidator(EmailEntry.Text, PasswordEntry.Text);
        if (message != null) 
        {
            switch (message)
            {
                case "Не все поля заполнены":
                case "Введенной почты не существует":
                case "Пароли не совпадают":
                    await DisplayAlert("Ошибка", message, "Ок");
                    PasswordEntry.Text = "";
                    break;
                case "Вы успешно вошли в аккаунт":
                    await DisplayAlert("Успех", message, "Ок");
                    ReturnAfterAuth();
                    break;
                default:
                    break;
            }
        } 
    }


    private void OnCreateTapped(object sender, EventArgs e)
    {
        TitlePage.Text = "Начни жизнь с нового аккаунта...";

        CreateLayout.IsVisible = true;
        GoEntrance.IsVisible = true;

        EntranceBorder.IsVisible = false;
        GoRegistr.IsVisible = false;




    }

    private void OnEntranceTapped(object sender, EventArgs e)
    {
        TitlePage.Text = "Мы вас ждали путник...";

        CreateLayout.IsVisible = false;
        GoEntrance.IsVisible = false;

        EntranceBorder.IsVisible = true;
        GoRegistr.IsVisible = true;

    }

    private void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        //Забыл пароль
    }
}