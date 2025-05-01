using CineChronicle.Tables;
using HtmlAgilityPack;

namespace TestProject;

public partial class MainPage : ContentPage
{
    private List<ContentRecommendation> ContentRecommendation = new List<ContentRecommendation>();
    private ContentRecommendation abc = new();

    private DatabaseServiceContent _databaseService;
    public List<Content> ContentAdded { get; set; }
    public List<Content> ContentChange { get; set; }
    public Content SelectedItem { get; set; }

    public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content.db");

    public MainPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseServiceContent(_databasePath);
        _databaseService.CreateTables();
        DatabaseServiceContent databaseService = new DatabaseServiceContent(_databasePath);
        List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();
        ContentAdded = allContent.Take(5).ToList();
        List<Content> allContentSecond = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();
        ContentChange = allContentSecond.Take(5).ToList();

        if (Device.RuntimePlatform == "WinUI")
        {
            MobilePhoneRec.IsVisible = false;
        }
        else
        {
            abc.GetRecommendation(ContentRecommendation);
            DisplayListRecommendation();
        }

        if (allContent.Count == 0)
        {
            Content emptyContent = new Content
            {
                Title = "Добавьте ваш первый контент",
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
            _databaseService.InsertContent(emptyContent);
            _databaseService = new DatabaseServiceContent(_databasePath);
             databaseService = new DatabaseServiceContent(_databasePath);
            allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();
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
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }
        else
        {
            await Navigation.PushAsync(new AddMoreContentPage());

            DatabaseServiceContent databaseService = new DatabaseServiceContent(_databasePath);
            databaseService.DeleteContent(selectedContent);
            databaseService.CloseConnection();
        }
    }
    private void ItemButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        var selectedIndex = new List<Content>((IEnumerable<Content>)RecentlyAddedCarouselView.ItemsSource).IndexOf(item);
        OnItemSelected(item, selectedIndex);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
       
        DisplayListAdded();
        DisplayListChange();
    
        if (Device.RuntimePlatform != "WinUI")
        {
            DisplayListRecommendation();
        }
        //DisplayRecentlyViewedContent();
    }
    
    
    private async void DisplayListRecommendation() //?
    {
        //await GetRecommendation();
        //RecentlyRecommendationCarouselView.ItemsSource = ContentRecommendation;
    }
    
    private void DisplayListAdded()
    {
        
        _databaseService = new DatabaseServiceContent(_databasePath);
        DatabaseServiceContent databaseService = new DatabaseServiceContent(_databasePath);
        List<Content> allContent = databaseService.GetAllContent().OrderByDescending(c => c.DateAdded).ToList();
        ContentAdded = allContent.Take(5).ToList();
        BindingContext = this;
        RecentlyAddedCarouselView.ItemsSource = ContentAdded;
    }
    private void DisplayListChange()
    {
        
        _databaseService = new DatabaseServiceContent(_databasePath);
        DatabaseServiceContent databaseService = new DatabaseServiceContent(_databasePath);
        List<Content> allContentSecond = databaseService.GetAllContent().OrderByDescending(c => c.SeriesChangeDate).ToList();
        ContentChange = allContentSecond.Take(5).ToList();
        BindingContext = this;
        RecentlyChangeCarouselView.ItemsSource = ContentChange;
    }
    
    private void ItemButtonClickedChange(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        var selectedIndex = new List<Content>((IEnumerable<Content>)RecentlyChangeCarouselView.ItemsSource).IndexOf(item);
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
            
    
            DatabaseServiceContent databaseService = new DatabaseServiceContent(_databasePath);
    
            databaseService.DeleteContent(selectedContent);
    
            databaseService.CloseConnection();
        }
    }
    
    private async void ItemButtonClickedRecommendation(object sender, EventArgs e)
    {
        var selectedItem = (ContentRecommendation)((Button)sender).CommandParameter;
        var viewContentPage = new ViewContentPage(selectedItem);
    
        await Navigation.PushAsync(viewContentPage);
    }
}


