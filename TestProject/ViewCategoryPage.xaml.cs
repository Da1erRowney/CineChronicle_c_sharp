namespace TestProject;
using SQLite;
using DataContent;
using Microsoft.Maui.Controls;
using static Microsoft.Maui.Controls.Internals.Profile;
using System.Diagnostics;
using System.Xml.Linq;

public partial class ViewCategoryPage : ContentPage
{
    private string NameCategory;
    public List<Content> ContentCategory { get; set; }
    public List<Content> ContentCategoryReserve { get; set; }
    public ViewCategoryPage(List<Content> ContentSort, string nameCategory)
	{
		InitializeComponent();
        ContentCategoryReserve = ContentSort;
        ContentCategory = ContentSort;
        SortLabel.Text = nameCategory;
        NameCategory = nameCategory;
         BindingContext = this;
        OnPropertyChanged(nameof(ContentCategory));
    }

    private async void OnItemSelectedSort(Content item, int selectedIndex)
    {
        if (item == null)
            return;

        Content selectedContent = ContentCategory[selectedIndex];
        ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
        await Navigation.PushAsync(viewContentPage);
    }

    private void ItemButtonClickedSort(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)ContentCategoryCollectionView.ItemsSource).IndexOf(item);
        OnItemSelectedSort(item, selectedIndex);
    }
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchQuery = searchBar.Text;
        if (searchQuery != "")
        {
           
        }
        else
        {
            SortLabel.Text = NameCategory;
            searchBar.Text = "";
            ContentCategory = ContentCategoryReserve;
            OnPropertyChanged(nameof(ContentCategory));
        }
    }
    private void SearchContent(object sender, EventArgs e)
    {

        string searchQuery = searchBar.Text;
        if (searchQuery != "")
        {
           
            SortLabel.Text = $"Искомый контент по запросу \"{searchQuery}\"";
            List<Content> SearchContent = ContentCategoryReserve;

            List<Content> filteredContents = SearchContent.Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            ContentCategory = filteredContents.ToList();
            BindingContext = this;
            OnPropertyChanged(nameof(ContentCategory));
        }
        else
        {
            SortLabel.Text = NameCategory;
            searchBar.Text = "";
            ContentCategory = ContentCategoryReserve;
            OnPropertyChanged(nameof(ContentCategory));

        }
    }
}