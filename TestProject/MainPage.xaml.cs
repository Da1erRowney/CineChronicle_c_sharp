using DataContent;
using SQLite;

namespace TestProject;

public partial class MainPage : ContentPage
{


    private DatabaseServiceContent _databaseService;
    public SQLiteConnection CreateDatabase(string databasePath)
    {
        SQLiteConnection connection = new SQLiteConnection(databasePath);
        connection.CreateTable<Content>();
        return connection;
    }
    public MainPage()
	{
		InitializeComponent();
        string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
        _databaseService = new DatabaseServiceContent(databasePath);
        SQLiteConnection connection = CreateDatabase(databasePath);
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


 

   




}

