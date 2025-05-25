namespace TestProject;

using CineChronicle.Application.AllContentPages;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;
using Microsoft.Maui.Controls;

public partial class ViewCategoryPage : ContentPage
{
    private static string _nameCategory {get;set;}
    public Content SelectedItem { get; set; }
    private ViewContentCategoryPageModel _model;

    public ViewCategoryPage(string nameCategory)
	{
		InitializeComponent();
        _nameCategory = nameCategory;
        SortLabel.Text = _nameCategory;

        // Инициализация конвертера
        Resources.Add("GreaterThanZeroConverter", new GreaterThanZeroConverter());
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_model == null)
        {
            _model = new ViewContentCategoryPageModel(_nameCategory);
            BindingContext = _model;
        }
    }

    #region [Content]
    private void ItemButtonClickedSort(object sender, EventArgs e)
    {
        HandleClick(sender);
    }

    private void HandleClick(object sender)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        OnItemClick(item.Id);
    }

    private async void OnItemClick(int id)
    {
        SelectedItem = ViewContentCategoryPageModel.GetContentById(id);
        if (SelectedItem.Type == "Ваш контент")
        {
            ViewContentCategoryPageModel.DeleteBaseContent();
            await Navigation.PushAsync(new AddMoreContentPage());
        }
        else
        {
            ViewContentPage viewContentPage = new ViewContentPage(SelectedItem);
            await Navigation.PushAsync(viewContentPage);
        }
    }
    #endregion

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchQuery = searchBar.Text;
        if (searchQuery != "")
        {
           
        }
        else
        {
            SortLabel.Text = _nameCategory;
            searchBar.Text = "";
            //ContentCategory = ContentCategoryReserve;
           // OnPropertyChanged(nameof(ContentCategory));
        }
    }
    private void SearchContent(object sender, EventArgs e)
    {

        string searchQuery = searchBar.Text;
        if (searchQuery != "")
        {
           
            SortLabel.Text = $"Искомый контент по запросу \"{searchQuery}\"";
           // List<Content> SearchContent = ContentCategoryReserve;

           // List<Content> filteredContents = SearchContent.Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
           // ContentCategory = filteredContents.ToList();
           // BindingContext = this;
            //OnPropertyChanged(nameof(ContentCategory));
        }
        else
        {
            SortLabel.Text = _nameCategory;
            searchBar.Text = "";
           // ContentCategory = ContentCategoryReserve;
          //  OnPropertyChanged(nameof(ContentCategory));

        }
    }
}