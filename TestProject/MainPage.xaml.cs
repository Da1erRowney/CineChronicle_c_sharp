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

        // Если база данных пуста, создаем контент с заданными значениями
        if (allContent.Count == 0)
        {
            Content emptyContent = new Content
            {
                Title = "Добавьте контент",
                Type = "Пустота",
                WatchStatus = "Смотрю",
                Image = "plus.png",
                Dubbing = null,
                LastWatchedSeries = 0,
                LastWatchedSeason = 0,
                NextEpisodeReleaseDate = null,
                Link = "",
                DateAdded = "2024-03-24 01:28:09",
                SeriesChangeDate = "",
                SmallDecription = ""
            };

            // Добавляем созданный контент в базу данных
            _databaseService.InsertContent(emptyContent);

             databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
            _databaseService = new DatabaseServiceContent(databasePath);
             connection = CreateDatabase(databasePath);
             databaseService = new DatabaseServiceContent(databasePath);

            // Получаем все элементы контента и сортируем их по дате добавления
            allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();

            // Отображаем только первые 5 элементов
            ContentAdded = allContent.Take(5).ToList();

             allContentSecond = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();
            ContentChange = allContentSecond.Take(5).ToList();
        }

        BindingContext = this;
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
        if (selectedContent.Type != "Пустота")
        {
            // Создайте новую страницу для отображения подробной информации
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);

            // Перейдите на новую страницу
            await Navigation.PushAsync(viewContentPage);
        }
        else
        {
            await Navigation.PushAsync(new AddMoreContentPage());
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

            databaseService.DeleteContent(selectedContent);

            databaseService.CloseConnection();
        }
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
        if (selectedContent.Type != "Пустота")
        {
            // Создайте новую страницу для отображения подробной информации
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);

            // Перейдите на новую страницу
            await Navigation.PushAsync(viewContentPage);
        }
        else
        {
            await Navigation.PushAsync(new AddMoreContentPage());
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

            databaseService.DeleteContent(selectedContent);

            databaseService.CloseConnection();
        }
    }


}

