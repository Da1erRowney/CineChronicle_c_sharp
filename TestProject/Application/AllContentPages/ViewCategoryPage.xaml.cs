namespace TestProject;

using CineChronicle.Application.AllContentPages;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;
using Microsoft.Maui.Controls;

public partial class ViewCategoryPage : ContentPage
{
    #region [Private Fields]

    public Content SelectedItem { get; set; }

    private ViewContentCategoryPageModel _model;
    private static string _nameCategory { get; set; }
    #endregion

    #region [Ctor's]
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

        string search = searchBar.Text;
        if (string.IsNullOrEmpty(search))
        {
            if (_model == null)
            {
                _model = new ViewContentCategoryPageModel(_nameCategory);
                BindingContext = _model;
            }
            else
            {
                BindingContext = _model;
            }
        }
        else
        {

        }
    }
    #endregion

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

    #region [Search Methods]

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchQuery = searchBar.Text;
        if (searchQuery != "")
        {
            SortLabel.Text = $"Искомый контент по запросу \"{searchQuery}\"";
        }
        else
        {
            SortLabel.Text = _nameCategory;
        }
            _model.UpdateContentsByQuery(searchQuery);
    }
    private void SearchContent(object sender, EventArgs e)
    {
        string searchQuery = searchBar.Text;
        if (searchQuery != "")
        {
          
        }
        else
        {

        }
    }
    #endregion
}