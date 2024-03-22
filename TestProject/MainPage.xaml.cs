using DataContent;
using Microsoft.Maui.Controls;
using SQLite;
using System.Collections.Generic;
using System.Windows.Input;

using System.Linq;

namespace TestProject;

public partial class MainPage : ContentPage
{

    private DatabaseServiceContent _databaseService;
    public List<Content> ContentAdded { get; set; }
    public List<Content>  ContentChange { get; set; }
    public Content SelectedItem { get; set; }
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
        DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

        // Получаем все элементы контента и сортируем их по дате добавления
        List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();

        // Отображаем только первые 5 элементов
        ContentAdded = allContent.Take(5).ToList();

        List<Content> allContentSecond = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();
        ContentChange = allContentSecond.Take(5).ToList();

        BindingContext = this;
       
        //OnBackButtonPressed();
      

    }
    //protected override bool OnBackButtonPressed()
    //{
    //    // Отменяем обработку стандартного поведения кнопки "Назад"
    //    NavigationPage.SetHasBackButton(this, false);
    //    return true;
    //}

    private async void OnItemSelected(Content item, int selectedIndex)
    {
        if (item == null)
            return;

        Content selectedContent = ContentAdded[selectedIndex];
        // Создайте новую страницу для отображения подробной информации
        ViewContentPage viewContentPage = new ViewContentPage(selectedContent);

        // Перейдите на новую страницу
        await Navigation.PushAsync(viewContentPage);
    }
    private void ItemButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)RecentlyAddedCarouselView.ItemsSource).IndexOf(item);
        OnItemSelected(item, selectedIndex);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
       
        DisplayListAdded();
        DisplayListChange();
        //DisplayRecentlyViewedContent();
    }
    private void DisplayListAdded()
    {
        string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
        _databaseService = new DatabaseServiceContent(databasePath);
        DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
        List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();
        ContentAdded = allContent.Take(5).ToList();
        BindingContext = this;
        RecentlyAddedCarouselView.ItemsSource = ContentAdded;
    }
    private void DisplayListChange()
    {
        string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
        _databaseService = new DatabaseServiceContent(databasePath);
        DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
        List<Content> allContentSecond = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();
        ContentChange = allContentSecond.Take(5).ToList();
        BindingContext = this;
        RecentlyChangeCarouselView.ItemsSource = ContentChange;
    }

    private void ItemButtonClickedChange(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)RecentlyChangeCarouselView.ItemsSource).IndexOf(item);
        OnItemSelectedChange(item, selectedIndex);
    }
    private async void OnItemSelectedChange(Content item, int selectedIndex)
    {
        if (item == null)
            return;

        Content selectedContent = ContentChange[selectedIndex];
        // Создайте новую страницу для отображения подробной информации
        ViewContentPage viewContentPage = new ViewContentPage(selectedContent);

        // Перейдите на новую страницу
        await Navigation.PushAsync(viewContentPage);
    }


    //private void DisplayRecentlyViewedContent()
    //{
    //    // Получаем путь к базе данных
    //    string databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "content.db");

    //    // Создаем экземпляр сервиса базы данных
    //    DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

    //    // Получаем все элементы контента и сортируем их по дате изменения серии в порядке убывания
    //    List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();

    //    // Отображаем только первые 5 элементов
    //    List<Content> recentlyViewedContent = allContent.Take(20).ToList();

    //    // Очищаем содержимое StackLayout перед добавлением новых элементов
    //    RecentlyViewedStackLayout.Children.Clear();

    //    // Создаем и добавляем метки для каждого элемента контента в StackLayout
    //    // Создаем и добавляем метки для каждого элемента контента в StackLayout
    //    // Создаем метку "Недавно просмотренные"
    //    Label titleSecondLabel = new Label
    //    {
    //        Text = "Недавно просмотренные",
    //        FontAttributes = FontAttributes.Bold,
    //        HorizontalOptions = LayoutOptions.Center,
    //        Margin = new Thickness(0, 20, 0, 10)
    //    };

    //    // Добавляем метку в StackLayout
    //    RecentlyViewedStackLayout.Children.Add(titleSecondLabel);

    //    // Создаем и добавляем метки для каждого элемента контента в StackLayout
    //    foreach (var contentItem in recentlyViewedContent)
    //    {
    //        // Проверяем, если SeriesChangeDate не пустой
    //        if (!string.IsNullOrEmpty(contentItem.SeriesChangeDate))
    //        {
    //            Label titleLabel = new Label
    //            {
    //                Text = $"{contentItem.Title}",
    //                FontAttributes = FontAttributes.Bold
    //            };

    //            Label seriesLabel = new Label
    //            {
    //                Text = $"Серия: {contentItem.LastWatchedSeries}, Сезон: {contentItem.LastWatchedSeason}"
    //            };

    //            StackLayout contentLayout = new StackLayout
    //            {
    //                Children = { titleLabel, seriesLabel },
    //                Spacing = 2
    //            };

    //            // Добавляем обработчик событий нажатия для каждого элемента списка
    //            contentLayout.GestureRecognizers.Add(new TapGestureRecognizer
    //            {
    //                Command = new Command(async () =>
    //                {
    //                    // Создаем экземпляр ViewContentPage и передаем выбранный контент
    //                    ViewContentPage viewContentPage = new ViewContentPage(contentItem);

    //                    // Используем Navigation для перехода на страницу ViewContentPage
    //                    await Navigation.PushAsync(viewContentPage);
    //                })
    //            });

    //            RecentlyViewedStackLayout.Children.Add(contentLayout);
    //        }
    //    }



    //    // Закрываем соединение с базой данных
    //    databaseService.CloseConnection();
    //}



}

