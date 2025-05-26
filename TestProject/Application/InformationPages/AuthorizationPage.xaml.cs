using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;

namespace TestProject;

public partial class AuthorizationPage : ContentPage
{
    #region [Ctor's]
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
    #endregion

    #region [Main Handle Methods]
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

    #endregion

    #region [Handle Methods]
    private void OnCreateTapped(object sender, EventArgs e)
    {
        HideElements("Начни жизнь с нового аккаунта...", true);
    }

    private void OnEntranceTapped(object sender, EventArgs e)
    {
        HideElements("Мы вас ждали путник...",false);
    }

    private void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        //Забыл пароль
    }
    #endregion

    #region [SomeBody Methods]
    private void HideElements(string str, bool status)
    {
        TitlePage.Text = "Мы вас ждали путник...";

        CreateLayout.IsVisible = status;
        GoEntrance.IsVisible = status;

        EntranceBorder.IsVisible = !status;
        GoRegistr.IsVisible = !status;
    }
    #endregion
}