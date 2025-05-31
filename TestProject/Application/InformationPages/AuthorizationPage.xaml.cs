using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;
using System.Net.Http.Json;
namespace TestProject;

public partial class AuthorizationPage : ContentPage
{
    private bool IsChange = false;

    #region [Ctor's]
    public AuthorizationPage(bool isChange = false)
	{
		InitializeComponent();
        IsChange= isChange;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new ViewAuthPageModel();
        if (!IsChange)
        {
            HideElements("Мы вас ждали, путник...", false);
        }
        else
        {
            HideAllForChangeData();
            FillingUserData();
        }
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
                case "Пользователь с таким ником уже существует":
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

    // Восстановление
    private async void RecoveryPassword()
    {
        if (!string.IsNullOrEmpty(EmailNickEntry.Text) && CodeBorder.IsVisible == false)
        {
            string message = ViewAuthPageModel.FindAccountUserForRecovery(EmailNickEntry.Text);
            switch (message)
            {
                case "Не удалось найти пользователя. Проверьте введенные данные.":
                case "Ошибка при отправке письма":
                    await DisplayAlert("Ошибка", message, "Ок");
                    break;
                default:
                    await DisplayAlert("Успех", message, "Ок");
                    CodeBorder.IsVisible = true;
                    LabelCode.IsVisible = true;
                    RecoveryButtonName.Text = "Подвердить код";
                    return;
            }
        }
        else if (CodeBorder.IsVisible == true && NewPasswordEntry.IsVisible == false)
        {
            if (!string.IsNullOrEmpty(CodeEntry.Text))
            {
                string message = ViewAuthPageModel.CheckRecoveryCode(CodeEntry.Text);
                switch (message)
                {
                    case "Код с почты указан неверно":
                        await DisplayAlert("Ошибка", message, "Ок");
                        CodeEntry.Text = null;
                        break;
                    default:
                        NewPasswordEntry.IsVisible = true;
                        RecoveryButtonName.Text = "Подвердить новый пароль";
                        EmailNickEntry.IsEnabled = false;
                        return;
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните поле с кодом, который пришел на вашу почту.", "Ок");
            }
        }
        else if (NewPasswordEntry.IsVisible)
        {
            if (!string.IsNullOrEmpty(PasswordNewEntry.Text))
            {
                string message = ViewAuthPageModel.CheckPasswordAndUpdateser(PasswordNewEntry.Text, EmailNickEntry.Text);
                switch (message)
                {
                    case "Пароль успешно обновлен.":
                        await DisplayAlert("Успех", message, "Ок");
                        HideElements("Мы вас ждали, путник...", false);
                        break;
                    default:
                        await DisplayAlert("Успех", message, "Ок");
                        return;
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните поле с паролем.", "Ок");
            }
        }
        else
        {
            await DisplayAlert("Ошибка", "Заполните поле ввода своей почтой либо своим ник неймом", "Ок");
        }
    }
 
    // Изменение данных
    private async void ChangeUserData()
    {
        if (ChangeLabel.IsVisible == false)
        {
            string message = ViewAuthPageModel.CheckChangeFields(EmailChangeEntry.Text, PasswordChangeEntry.Text, NickNameChangeEntry.Text);
            switch (message)
            {
                case "Пользователь успешно изменен":
                    await DisplayAlert("Успех", message, "Ок");
                    ReturnAfterAuth();
                    break;
                case "На вашу изначальную почту был выслан код подтверждения для изменений":
                    await DisplayAlert("Уведомление", message, "Ок");
                    VoidDataName.Text = "Подтвердить код";
                    ChangeLabel.IsVisible = true;
                    CodeChangeBorder.IsVisible = true;
                    break;
                default:
                    await DisplayAlert("Ошибка", message, "Ок");
                    break;
            }
        }
        else
        {
            string message = ViewAuthPageModel.CheckRecoveryCode(CodeChangeEntry.Text);
            switch (message)
            {
                case "Код с почты указан неверно":
                    await DisplayAlert("Ошибка", message, "Ок");
                    CodeEntry.Text = null;
                    break;
                default:
                    string messageLast = ViewAuthPageModel.SaveNewUserData(EmailChangeEntry.Text, PasswordChangeEntry.Text, NickNameChangeEntry.Text);
                    switch (messageLast)
                    {
                        case "Данные успешно изменены":
                            await DisplayAlert("Успех", messageLast, "Ок");
                            ReturnAfterAuth();
                            break;
                        default:
                            await DisplayAlert("Ошибка", messageLast, "Ок");
                            break;
                    }
                   
                    return;
            }
        }
    }
    private async void FillingUserData()
    {
        var user = ViewAuthPageModel.GetUser();

        EmailChangeEntry.Text = user.Email;
        PasswordChangeEntry.Text = user.Password;
        NickNameChangeEntry.Text = user.NickName;
    }

    // Возвращение к информации после манипуляций
    private async void ReturnAfterAuth()
    {
        // Переход к информации
        await Shell.Current.GoToAsync("//Information");
    }

    #endregion

    #region [Google Auth]
    private async Task GoogleAuthSystem()
    {
        try
        {
            var clientId = "28327101441-8lg1g6eb10bqubimnnv4gmeddcbuvhsc.apps.googleusercontent.com";
            var redirectUri = "com.googleusercontent.apps.28327101441-8lg1g6eb10bqubimnnv4gmeddcbuvhsc:/oauth2redirect";

            // Используем response_type=code вместо token
            var authUrl =
                "https://accounts.google.com/o/oauth2/v2/auth?" +
                $"client_id={clientId}&" +
                "response_type=code&" +  // Изменено на code
                $"redirect_uri={redirectUri}&" +
                "scope=email";

            var callbackUrl = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl),
                new Uri(redirectUri));

            // Теперь получаем authorization code вместо token
            var authCode = callbackUrl.Properties["code"];

            if (string.IsNullOrEmpty(authCode))
                throw new Exception("Authorization code not received");

            // Обмениваем code на access token
            var httpClient = new HttpClient();
            var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("code", authCode),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code")
        }));

            var tokenData = await tokenResponse.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            var accessToken = tokenData?.access_token;

            // Получаем email
            var userInfoResponse = await httpClient.GetAsync(
                $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");

            if (!userInfoResponse.IsSuccessStatusCode)
                throw new Exception("Failed to get user info");

            var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>();
            string message = ViewAuthPageModel.CheckGoogleAccount(userInfo?.Email);
            await DisplayAlert("Успех", message, "Ок");
            ReturnAfterAuth();
        }
        catch (Exception ex)
        {
            //await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Дополнительные классы для десериализации
    public class GoogleTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class GoogleUserInfo
    {
        public string Email { get; set; }
    }
    #endregion

    #region [Handle Methods]
    private void OnCreateTapped(object sender, EventArgs e)
    {
        HideElements("Начни жизнь с нового аккаунта...", true);
    }

    private void OnEntranceTapped(object sender, EventArgs e)
    {
        HideElements("Мы вас ждали, путник...", false);
    }

    private void OnGoogleAuthTapped(object sender, EventArgs e)
    {
       GoogleAuthSystem();
    }

    private void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        TitlePage.Text = "Восстановление героя";
        CreateLayout.IsVisible = false;
        GoEntrance.IsVisible = false;

        EntranceBorder.IsVisible = false;
        GoRegistr.IsVisible = false;

        RecoveryLayout.IsVisible = true;
        AfterRecoveryEntrance.IsVisible = true;

        RecoveryPassword();
    }

    private async void VoidDataClicked(object sender, EventArgs e)
    {
        ChangeUserData();
    }

    private async void RecoveryButtonTapped(object sender, EventArgs e)
    {
        RecoveryPassword();
    }
    #endregion

    #region [SomeBody Methods]
    private void HideAllForChangeData()
    {
        TitlePage.Text = "Изменение ваших учетных данных";
        ChangeLayout.IsVisible = true;
        ChangeLabel.IsVisible = false;
        CodeChangeBorder.IsVisible = false;

        CreateLayout.IsVisible = false;
        GoEntrance.IsVisible = false;

        EntranceBorder.IsVisible = false;
        GoRegistr.IsVisible = false;

        // Восстановление пароля
        RecoveryLayout.IsVisible = false;
        AfterRecoveryEntrance.IsVisible = false;
        VoidDataName.Text = "Подтвердить изменения";
    }

    private void HideElements(string str, bool status)
    {
        TitlePage.Text = str;

        CreateLayout.IsVisible = status;
        GoEntrance.IsVisible = status;

        EntranceBorder.IsVisible = !status;
        GoRegistr.IsVisible = !status;

        // Восстановление пароля
        RecoveryLayout.IsVisible = false;
        AfterRecoveryEntrance.IsVisible = false;
        // Сбрасываем поля относящиеся к восстановлению
        CodeBorder.IsVisible = false;
        LabelCode.IsVisible = false;
        NewPasswordEntry.IsVisible = false;
        RecoveryButtonName.Text = "Выслать код на почту";
        EmailNickEntry.IsEnabled = true;
        EmailNickEntry.Text = null;

        // Изменение данных
        ChangeLayout.IsVisible = false;
        ChangeLabel.IsVisible = false;
        CodeChangeBorder.IsVisible = false;
        VoidDataName.Text = "Подтвердить изменения";
    }
    #endregion
}