namespace TestProject;

public partial class AllContentPage : ContentPage
{
	private string Choise;
	public AllContentPage( string userChoise)
	{
		InitializeComponent();
		Choise = userChoise;
		switch (Choise)
		{
			case "All":
                MainLabel.Text = "��� ���� �������";
				return;
            case "Movies":
                MainLabel.Text = "���� ������";
                return;
            case "Series":
                MainLabel.Text = "���� �������";
                return;
            case "Anime":
                MainLabel.Text = "���� �����";
                return;
            case "Other":
                MainLabel.Text = "������ �������";
                return;
            case "Doram":
                MainLabel.Text = "���� ������";
                return;
            case "Mult":
                MainLabel.Text = "���� ������������";
                return;
            case "Documental":
                MainLabel.Text = "���� �������������� ��������";
                return;


        }


    }

}