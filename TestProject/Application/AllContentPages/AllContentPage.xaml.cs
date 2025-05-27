using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllContentPage : ContentPage
    {
        #region [Private Fields]
        public Content SelectedItem { get; set; }

        private ViewContentAllPageModel _model;
        private int _oldUser;
        #endregion

        #region [Ctor's]
        public AllContentPage()
        {
            InitializeComponent();
            _oldUser = CineChronicle.Application.DeviceInfo.UserId;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UseNewBackground();

            string search = searchBar.Text;
            if (string.IsNullOrEmpty(search))
            {
                if (_model == null || _oldUser != CineChronicle.Application.DeviceInfo.UserId)
                {
                    _oldUser = CineChronicle.Application.DeviceInfo.UserId;
                    BindingContext = new ViewContentAllPageModel();
                    _model = (ViewContentAllPageModel)BindingContext;
                }
                    Sort.IsVisible = false;
            }
            else
            {
               
            }
        }
        private void UseNewBackground()
        {
            Random _random = new Random();
            string randomImage = $"{BackgroundImages._backgroundImages[_random.Next(0, BackgroundImages._backgroundImages.Length)]}.jpg";
            Background.Source = randomImage;
        }
        #endregion

        #region [Category]
        private async void CategoryButton_Clicked(object sender, EventArgs e)
        {
            ViewCategoryPage viewContentPage = new ViewCategoryPage(ViewContentAllPageModel.GetName(sender));
            await Navigation.PushAsync(viewContentPage);
        }

        #endregion

        #region [Content]
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

        private async void OnItemClick(int id)
        {
            SelectedItem = ViewContentAllPageModel.GetContentById(id);
            if (SelectedItem.Type == "Ваш контент")
            {
                ViewContentAllPageModel.DeleteBaseContent();
                await Navigation.PushAsync(new AddMoreContentPage());
            }
            else
            {
                ViewContentPage viewContentPage = new ViewContentPage(SelectedItem);
                await Navigation.PushAsync(viewContentPage);
            }
        }
        #endregion

        #region [Refresh Data]
        private async void OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                BindingContext = new ViewContentAllPageModel();
                _model = (ViewContentAllPageModel)BindingContext;
            }
            catch (Exception ex)
            {
                if (RefreshView != null)
                    RefreshView.IsRefreshing = false;
            }
            finally
            {
                if (RefreshView != null)
                    RefreshView.IsRefreshing = false;
            }
        }
        #endregion

        #region [Search Methods]
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = searchBar.Text;
            if (searchQuery != "")
            {
                DoShowElements(false);
                Sort.IsVisible = true;
                SortLabel.Text = $"Искомый контент по запросу \"{searchQuery}\"";
                _model.UpdateContentsByQuery(searchQuery);
            }
            else
            {
                SortLabel.Text = "";
                searchBar.Text = "";
                Sort.IsVisible = false;

                DoShowElements(true);
                BindingContext = _model;
            }
        }

        private void SearchContent(object sender, EventArgs e)
        {
            string searchQuery = searchBar.Text;
            if (searchQuery != "")
            {

            }
            else
            {
                SortLabel.Text = "";
                searchBar.Text = "";
                Sort.IsVisible = false;

                DoShowElements(true);
                BindingContext = _model;
            }
        }
        // Возврат показа всего
        private async void ВсеButton_Clicked(object sender, EventArgs e)
        {
            DoShowElements(true);

            Sort.IsVisible = false;
            searchBar.Text = "";
        }

        private void DoShowElements(bool isShow)
        {
            bool[] visible = _model.GetVisibleProperties();
            var elements = new[]
            {
                (Element: All, VisibleIndex: 0),
                (Element: Serial, VisibleIndex: 1),
                (Element: Anime, VisibleIndex: 2),
                (Element: Film, VisibleIndex: 3),
                (Element: Dorama, VisibleIndex: 4),
                (Element: Mult, VisibleIndex: 5),
                (Element: Docum, VisibleIndex: 6),
                (Element: Other, VisibleIndex: 7),
                (Element: Viewed, VisibleIndex: 8),
                (Element: Process, VisibleIndex: 9),
                (Element: NotStart, VisibleIndex: 10)
            };

            foreach (var (element, visibleIndex) in elements)
            {
                element.IsVisible = isShow && visible[visibleIndex];
            }
        }
        #endregion
    }
}