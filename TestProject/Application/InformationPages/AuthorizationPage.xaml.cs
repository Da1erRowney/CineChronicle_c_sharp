using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;
using System.Net.Http.Json;
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
    private void RecoveryPassword()
    {
        
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

    private async void RecoveryButtonTapped(object sender, EventArgs e)
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
    #endregion

    #region [SomeBody Methods]
    private void HideElements(string str, bool status)
    {
        TitlePage.Text = str;

        CreateLayout.IsVisible = status;
        GoEntrance.IsVisible = status;

        EntranceBorder.IsVisible = !status;
        GoRegistr.IsVisible = !status;

        // Восстановление пароля
        RecoveryLayout.IsVisible = false;
        CodeBorder.IsVisible = false;
        LabelCode.IsVisible = false;
        NewPasswordEntry.IsVisible = false;
        AfterRecoveryEntrance.IsVisible = false;
        RecoveryButtonName.Text = "Выслать код на почту";
        EmailNickEntry.IsEnabled = true;
        EmailNickEntry.Text = null;

    }
    #endregion
}