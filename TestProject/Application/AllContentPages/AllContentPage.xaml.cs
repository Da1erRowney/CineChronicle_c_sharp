using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllContentPage : ContentPage
    { 

        public string Choise = "All";

        private List<Content> _contentSearch;
        public List<Content> ContentSort { get; set; }
        public List<Content> ContentSearch
        {
            get { return _contentSearch; }
            set
            {
                _contentSearch = value;
                OnPropertyChanged(nameof(ContentSearch));
            }
        }

        public Content SelectedItem { get; set; }


        public AllContentPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = new ViewContentAllPageModel();

            string search = searchBar.Text;
            if (search != null && search != "")
            {
                Sort.IsVisible = true;
                DoShowElements(false);
            }
            else
            {
                Sort.IsVisible = false;
            }
        }

        // Возврат показа всего
        private async void ВсеButton_Clicked(object sender, EventArgs e)
        {
            DoShowElements(true);

            Sort.IsVisible = false;
            searchBar.Text = "";
            Choise = "All";

        }

        private void DoShowElements(bool isShow)
        {
            All.IsVisible = isShow;
            Serial.IsVisible = isShow;
            Anime.IsVisible = isShow;
            Film.IsVisible = isShow;
            Dorama.IsVisible = isShow;
            Mult.IsVisible = isShow;
            Docum.IsVisible = isShow;
            Other.IsVisible = isShow;
            Viewed.IsVisible = isShow;
            Process.IsVisible = isShow;
            NotStart.IsVisible = isShow;
        }

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

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Content selectedContent)
            {
                ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
                await Navigation.PushAsync(viewContentPage);
            }
        }
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = searchBar.Text;
            if (searchQuery != "")
            {
               
            }
            else
            {
                searchBar.Text = "";
                SearchList.IsVisible = false;
                Sort.IsVisible = false;
                ContentSort = null; // Установка источника данных в null
                OnPropertyChanged(nameof(ContentSort));
            }
        }

        private void SearchContent(object sender, EventArgs e)
        {
            string searchQuery = searchBar.Text;
            if (searchQuery != "")
            {
                DoShowElements(false);

                SearchList.IsVisible = false;

                Sort.IsVisible = true;
                SortLabel.Text = $"Искомый контент по запросу \"{searchQuery}\"";

                ContentSort = ViewContentAllPageModel.GetContentsByQuery(searchQuery);
                BindingContext = this;
            }
            else
            {
                SortLabel.Text = "";
                searchBar.Text = "";
                SearchList.IsVisible = false;
                Sort.IsVisible = false;
                ContentSort = null; // Установка источника данных в null
                OnPropertyChanged(nameof(ContentSort));
            }
        }

    }
}