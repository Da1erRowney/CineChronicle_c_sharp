using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;

namespace TestProject;

public partial class InformationPage : ContentPage
{
    ViewInformationPageModel _model;

    #region [Ctor's]
    public InformationPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UseNewBackground();

        BindingContext = new ViewInformationPageModel();
        _model = (ViewInformationPageModel)BindingContext;

        CheckedAuthUser();
    }

    private void UseNewBackground()
    {
        Random _random = new Random();
        string randomImage = $"{BackgroundImages._backgroundImages[_random.Next(0, BackgroundImages._backgroundImages.Length)]}.jpg";
        Background.Source = randomImage;
    }
    #endregion

    #region [Somebody Methods]
    private void CheckedAuthUser()
    {
        if(_model.HaveAthorizedUser)
        {
            HideElements(false, "Сменить аккаунт",200);
        }
        else
        {
            HideElements(true, "Авторизация", 150);
        }
    }
    private void HideElements(bool status, string str, int width)
    {
        NoteAuthAccountLayout.IsVisible = status;
        InformationBlock.IsVisible = !status;
        ButtonExit.IsVisible = !status;
        ButtonChange.IsVisible = !status;
        ButtonAuth.Text = str;
        ButtonAuth.WidthRequest = width;
    }
    #endregion

    #region [Handle Methods]
    private async void OnAvatarTapped(object sender, EventArgs e)
    {
        await _model.ChangeAvatarAsync(NicknameUserLabel.Text);
    }
    private async void AuthButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AuthorizationPage());
    }
    private void ExitAccountButton_Clicked(object sender, EventArgs e)
    {
        _model.ExitAccount();
        CheckedAuthUser();
    }
    private async void ChangeUserDataButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AuthorizationPage(true));
    }
    #endregion

}