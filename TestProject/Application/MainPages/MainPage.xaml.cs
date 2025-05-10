using CineChronicle.Application.MainPage;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;
using HtmlAgilityPack;

namespace TestProject;

public partial class MainPage : ContentPage
{
    public Content SelectedItem { get; set; }

    public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content.db");

    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ViewContentMainPageModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = new ViewContentMainPageModel();

        if (Device.RuntimePlatform == "WinUI")
        {
            MobilePhoneRec.IsVisible = false;
        }
        else
        {
            MobilePhoneRec.IsVisible = true;
        }
    }

    private void ItemButtonClickedChange(object sender, EventArgs e)
    {
        HandleClick(sender);
    }

    private void ItemButtonClicked(object sender, EventArgs e)
    {
        HandleClick(sender);
    }

    private void HandleClick(object sender)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        OnItemClick(item.Id);
    }

    private async void ItemButtonClickedRecommendation(object sender, EventArgs e)
    {
        var selectedItem = (ContentRecommendation)((Button)sender).CommandParameter;
        var viewContentPage = new ViewContentPage(selectedItem);

        await Navigation.PushAsync(viewContentPage);
    }

    private async void OnItemClick(int id)
    {
        SelectedItem = ViewContentMainPageModel.GetContentById(id);
        if (SelectedItem.Type == "Пустота")
        {
            await Navigation.PushAsync(new AddMoreContentPage());
        }
        else
        {
            ViewContentPage viewContentPage = new ViewContentPage(SelectedItem);
            await Navigation.PushAsync(viewContentPage);
        }
    }
    
   
}


