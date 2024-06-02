using DataContent;
using System.Text.RegularExpressions;

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
        if(Email1Entry.Text .Length == 0 && Password1Entry.Text.Length==0) 
        {
            await DisplayAlert("������", "�� ��� ���� ���������", "OK");
            return;
        }

        string email = Email1Entry.Text.ToLower().TrimEnd();
        if (!ValidateEmail(email))
        {
            await DisplayAlert("������", "������������ ������ �����", "OK");
            return;
        }

        if (databaseService.GetUsereByEmail(email) !=null ) 
        {
            await DisplayAlert("������","����� ������������ ��� ����������", "OK");
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
        await DisplayAlert("�����", "������� ������, �� ������� ����� � �������", "OK");
        await Navigation.PushAsync(new InformationPage());
    }

  
    private async void OnEntranceClicked(object sender, EventArgs e)
    {

        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
        if (EmailEntry.Text.Length == 0 && PasswordEntry.Text.Length == 0)
        {
            await DisplayAlert("������", "�� ��� ���� ���������", "OK");
            return;
        }
        string email = Email1Entry.Text.ToLower().TrimEnd();
        if (databaseService.GetUsereByEmail(email).Email != null)
        {
            await DisplayAlert("������", "��������� ����� �� ����������", "��");
            return;
        }

        if (databaseService.GetUsereByEmail(email).Password != PasswordEntry.Text)
        {
            await DisplayAlert("������", "������ �� ���������", "OK");
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
            await DisplayAlert("�����", "�� ������� ����� � �������", "OK");
            await Navigation.PushAsync(new InformationPage());
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
        TitlePage.Text = "����� ����� ����� � ������ ��������...";
        CreateLayout.IsVisible = true; //�� ���� ��� � ��������� ���� border ��� ��������
        EntranceBorder.IsVisible = false;
     
    }

    private void OnReturnTapped(object sender, EventArgs e)
    {
        TitlePage.Text = "�� ��� ����� ������...";
        CreateLayout.IsVisible = false;
        EntranceBorder.IsVisible = true;

    }

    private void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        //����� ������
    }
}