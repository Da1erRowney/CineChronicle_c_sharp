using DataContent;
using Microsoft.Maui.Controls;
using SQLite;
using System.Collections.Generic;
using System.Windows.Input;

using System.Linq;
using CommunityToolkit.Maui.Core;
using System.ComponentModel;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace TestProject;
public class ContentRecommendation
{
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
}

public partial class MainPage : ContentPage
{

    private List<ContentRecommendation> ContentRecommendation = new List<ContentRecommendation>();
    private DatabaseServiceContent _databaseService;
    public static readonly string _databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
    public List<Content> ContentAdded { get; set; }
    public List<Content> ContentChange { get; set; }
    public Content SelectedItem { get; set; }

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
            GetRecommendation();
            DisplayListRecommendation();
        }
        // Если база данных пуста, создаем контент с заданными значениями
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

    public async Task<List<ContentRecommendation>> GetRecommendation()
    {
        string url = "https://www.toramp.com/";
    
        List<ContentRecommendation> recommendations = new List<ContentRecommendation>();
    
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
    
            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();
    
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);
    
                HtmlNodeCollection recommendationNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='content h_scroll dis_flex pt_5 pb_5']/div[@class='pos_rel pr_5']");
    
                if (recommendationNodes != null)
                {
                    foreach (HtmlNode recommendationNode in recommendationNodes)
                    {
                        HtmlNode imageNode = recommendationNode.SelectSingleNode(".//a[@class='imgWrapper']/img");
                        HtmlNode titleNode = recommendationNode.SelectSingleNode(".//a[@class='imgWrapper']/img/@alt");
    
                        string imageSrc = imageNode?.GetAttributeValue("src", "");
                        string title = titleNode?.GetAttributeValue("alt", "");
    
                        if (!string.IsNullOrEmpty(imageSrc) && !string.IsNullOrEmpty(title))
                        {
                            string type = GetContentType(title); // Extract the type from the title
                                int slashIndex = title.IndexOf('/');
                                string titleBeforeSlash = title;
                                if (slashIndex >= 0)
                                {
                                    titleBeforeSlash = title.Substring(0, slashIndex);
                                    // Используйте titleBeforeSlash как требуется
                                }
                                else
                                {
                                    // Символ `/` не найден в строке title
                                }
                                title = titleBeforeSlash;
                                var newContent = new ContentRecommendation
                                {
                                    ImageUrl = imageSrc,
                                    Title = title,
                                    Type = type
                                };
                                recommendations.Add(newContent);
                            }
                    }
                        ContentRecommendation = recommendations;
                }
            }
        }
    
        return recommendations;
    }

    public string GetContentType(string title)
    {

        if (title.Contains("(сериал)"))
        {
            return "Сериал";
        }
        else if (title.Contains("(аниме)"))
        {
            return "Аниме";
        }
        else if (title.Contains("(мультсериал)"))
        {
            return "Мультсериал";
        }
        else if (title.Contains("(мультфильм)"))
        {
            return "Мультсериал";
        }
        else
        {
            return "неизвестно";
        }
    }

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
        var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)RecentlyAddedCarouselView.ItemsSource).IndexOf(item);
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
    
    
    private async void DisplayListRecommendation()
    {
        await GetRecommendation();
        RecentlyRecommendationCarouselView.ItemsSource = ContentRecommendation;
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


