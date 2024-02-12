using DataContent;
namespace TestProject;

public partial class MainPage : ContentPage
{


    public static string UserChoice;

	public MainPage()
	{
		InitializeComponent();
        DisplayRecentlyAddedContent();
        DisplayRecentlyViewedContent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        DisplayRecentlyAddedContent();
        DisplayRecentlyViewedContent();
    }

    private void DisplayRecentlyAddedContent()
    {
        // Получаем путь к базе данных
        string databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "content.db");

        // Создаем экземпляр сервиса базы данных
        DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

        // Получаем все элементы контента и сортируем их по дате добавления
        List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();

        // Отображаем только первые 5 элементов
        List<Content> recentlyAddedContent = allContent.Take(5).ToList();

        // Очищаем содержимое StackLayout перед добавлением новых элементов
        RecentlyAddedStackLayout.Children.Clear();

        // Создаем и добавляем метки для каждого элемента контента в StackLayout
        // Создаем метку для заголовка
        Label titleLabel = new Label
        {
            Text = "Недавно добавленный контент",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 10)
        };

        // Добавляем метку заголовка в StackLayout
        RecentlyAddedStackLayout.Children.Add(titleLabel);

        // Создаем и добавляем метки для каждого элемента контента в StackLayout
        foreach (var contentItem in recentlyAddedContent)
        {
            Label nameLabel = new Label
            {
                Text = $"{contentItem.Title}",
                FontAttributes = FontAttributes.Bold
            };

            Label typeLabel = new Label
            {
                Text = $"{contentItem.Type}"
            };

            StackLayout contentLayout = new StackLayout
            {
                Children = { nameLabel, typeLabel },
                Spacing = 2
            };

            // Добавляем обработчик событий нажатия для каждого элемента списка
            contentLayout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    // Создаем экземпляр ViewContentPage и передаем выбранный контент
                    ViewContentPage viewContentPage = new ViewContentPage(contentItem);

                    // Используем Navigation для перехода на страницу ViewContentPage
                    await Navigation.PushAsync(viewContentPage);
                })
            });

            RecentlyAddedStackLayout.Children.Add(contentLayout);
        }


        // Закрываем соединение с базой данных
        databaseService.CloseConnection();
    }

    private void DisplayRecentlyViewedContent()
    {
        // Получаем путь к базе данных
        string databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "content.db");

        // Создаем экземпляр сервиса базы данных
        DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

        // Получаем все элементы контента и сортируем их по дате изменения серии в порядке убывания
        List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();

        // Отображаем только первые 5 элементов
        List<Content> recentlyViewedContent = allContent.Take(5).ToList();

        // Очищаем содержимое StackLayout перед добавлением новых элементов
        RecentlyViewedStackLayout.Children.Clear();

        // Создаем и добавляем метки для каждого элемента контента в StackLayout
        // Создаем и добавляем метки для каждого элемента контента в StackLayout
        // Создаем метку "Недавно просмотренные"
        Label titleSecondLabel = new Label
        {
            Text = "Недавно просмотренные",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 10)
        };

        // Добавляем метку в StackLayout
        RecentlyViewedStackLayout.Children.Add(titleSecondLabel);

        // Создаем и добавляем метки для каждого элемента контента в StackLayout
        foreach (var contentItem in recentlyViewedContent)
        {
            // Проверяем, если SeriesChangeDate не пустой
            if (!string.IsNullOrEmpty(contentItem.SeriesChangeDate))
            {
                Label titleLabel = new Label
                {
                    Text = $"{contentItem.Title}",
                    FontAttributes = FontAttributes.Bold
                };

                Label seriesLabel = new Label
                {
                    Text = $"Серия: {contentItem.LastWatchedSeries}, Сезон: {contentItem.LastWatchedSeason}"
                };

                StackLayout contentLayout = new StackLayout
                {
                    Children = { titleLabel, seriesLabel },
                    Spacing = 2
                };

                // Добавляем обработчик событий нажатия для каждого элемента списка
                contentLayout.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        // Создаем экземпляр ViewContentPage и передаем выбранный контент
                        ViewContentPage viewContentPage = new ViewContentPage(contentItem);

                        // Используем Navigation для перехода на страницу ViewContentPage
                        await Navigation.PushAsync(viewContentPage);
                    })
                });

                RecentlyViewedStackLayout.Children.Add(contentLayout);
            }
        }



        // Закрываем соединение с базой данных
        databaseService.CloseConnection();
    }


    private async void OnAllClicked(object sender, EventArgs e)
    {
        UserChoice = "All";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }


    private async void OnMoviesClicked(object sender, EventArgs e)
    {
        UserChoice = "Movies";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnSeriesClicked(object sender, EventArgs e)
    {
        UserChoice = "Series";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnAnimeClicked(object sender, EventArgs e)
    {
        UserChoice = "Anime";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnOthersClicked(object sender, EventArgs e)
    {
        UserChoice = "Other";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }
    private async void OnDoramClicked(object sender, EventArgs e)
    {
        UserChoice = "Doram";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnMultSeriesClicked(object sender, EventArgs e)
    {
        UserChoice = "Mult";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }
    private async void OnDocumentalClicked(object sender, EventArgs e)
    {
        UserChoice = "Documental";
        await Navigation.PushAsync(new AllContentPage(UserChoice));
    }

    private async void OnAddMoreClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddMoreContentPage());
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {

    }

    private async void OnSidebarToggle(object sender, EventArgs e)
    {
        if (Sidebar.IsVisible)
        {
            await Sidebar.FadeTo(0, 700); // Увеличить время анимации до 500 миллисекунд
            Sidebar.IsVisible = false;
            TabButton.BackgroundColor = Color.FromHex("#808080"); // Используйте цвет серого
            Sidebar.WidthRequest = 0; // Установить ширину панели в 0
        }
        else
        {
            Sidebar.IsVisible = true;
            TabButton.BackgroundColor = Color.FromHex("#A9A9A9"); // Используйте цвет темно-серого
            await Sidebar.FadeTo(1, 100); // Увеличить время анимации до 500 миллисекунд
            Sidebar.WidthRequest = 350; // Установить желаемую ширину панели
        }
    }






    private void OnUserStatisticsClicked(object sender, EventArgs e)
    {

    }

}

