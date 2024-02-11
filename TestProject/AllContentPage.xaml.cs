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
                MainLabel.Text = "Ваш весь контент";
				return;
            case "Movies":
                MainLabel.Text = "Ваши фильмы";
                return;
            case "Series":
                MainLabel.Text = "Ваши сериалы";
                return;
            case "Anime":
                MainLabel.Text = "Ваше аниме";
                return;
            case "Other":
                MainLabel.Text = "Прочий контент";
                return;
            case "Doram":
                MainLabel.Text = "Ваши дорамы";
                return;
            case "Mult":
                MainLabel.Text = "Ваши мультсериалы";
                return;
            case "Documental":
                MainLabel.Text = "Ваши документальные передачи";
                return;


        }


    }

}