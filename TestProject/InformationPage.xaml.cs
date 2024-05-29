using System.Net;

namespace TestProject;

public partial class InformationPage : ContentPage
{
	public InformationPage()
	{
		InitializeComponent();
	}

    private async void AuthButton_Clicked(object sender, EventArgs e)
    {
        AuthorizationPage authorization = new AuthorizationPage();

        await Navigation.PushAsync(authorization);
    }
}