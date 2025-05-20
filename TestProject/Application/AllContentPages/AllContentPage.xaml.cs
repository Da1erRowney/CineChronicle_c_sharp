using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;
using System.Globalization;
using System.Net.Mime;
using System.Xml.Linq;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllContentPage : ContentPage
    { 
    
        public string Choise ="All";

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

        private DatabaseServiceContent _databaseService = new DatabaseServiceContent(CineChronicle.Application.DeviceInfo._databasePath);

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
                VisibleFalse();
            }
            else
            {
                Sort.IsVisible = false;
                ViewData();
            }
        }
           
        // Скрытие блоков, где контент пуст
        private void ViewData()
        {

            //if (ContentAll.Count == 0)
            //{
            //    All.IsVisible = false;
            //}
            //else
            //{
            //    All.IsVisible = true;
            //}
            //if (ContentSerial.Count == 0)
            //{
            //    Serial.IsVisible = false;
            //}
            //else
            //{
            //    Serial.IsVisible = true;
            //}
            //if (ContentAnime.Count == 0)
            //{
            //    Anime.IsVisible = false;
            //}
            //else
            //{
            //    Anime.IsVisible = true;
            //}
            //if (ContentFilm.Count == 0)
            //{
            //    Film.IsVisible = false;
            //}
            //else
            //{
            //    Film.IsVisible = true;
            //}
            //if (ContentDorama.Count == 0)
            //{
            //    Dorama.IsVisible = false;
            //}
            //else
            //{
            //    Dorama.IsVisible = true;
            //}
            //if (ContentMult.Count == 0)
            //{
            //    Mult.IsVisible = false;
            //}
            //else
            //{
            //    Mult.IsVisible = true;
            //}
            //if (ContentDocum.Count == 0)
            //{
            //    Docum.IsVisible = false;
            //}
            //else
            //{
            //    Docum.IsVisible = true;
            //}
            //if (ContentOther.Count == 0)
            //{
            //    Other.IsVisible = false;
            //}
            //else
            //{
            //    Other.IsVisible = true;
            //}
            //if (ContentViewed.Count == 0)
            //{
            //    Viewed.IsVisible = false;
            //}
            //else
            //{
            //    Viewed.IsVisible = true;
            //}
            //if (ContentProcess.Count == 0)
            //{
            //    Process.IsVisible = false;
            //}
            //else
            //{
            //    Process.IsVisible = true;
            //}
            //if (ContentNotStart.Count == 0)
            //{
            //    NotStart.IsVisible = false;
            //}
            //else
            //{
            //    NotStart.IsVisible = true;
            //}
        }

        private void VisibleFalse()
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = false;
            Dorama.IsVisible = false;
            Mult.IsVisible = false;
            Docum.IsVisible = false;
            Other.IsVisible = false;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        // Возврат показа всего
        private async void ВсеButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = true;
            Serial.IsVisible = true;
            Anime.IsVisible = true;
            Film.IsVisible = true;
            Dorama.IsVisible = true;
            Mult.IsVisible = true;
            Docum.IsVisible = true;
            Other.IsVisible = true;
            Viewed.IsVisible = true;
            Process.IsVisible = true;
            NotStart.IsVisible = true;


            Sort.IsVisible = false;
            ViewData();
            searchBar.Text = "";
            Choise = "All";

        }

        #region [Category]
        private async void CategoryButton_Clicked(object sender, EventArgs e)
        {
            ViewCategoryPage viewContentPage = new ViewCategoryPage(ViewContentAllPageModel.PrepareCategory(sender), ViewContentAllPageModel._categoryData[1]);
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
                //Filling();
                ViewData();
            }
        }

        private void ItemButtonClickedSort(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<Content>)SortContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedSort(item, selectedIndex);
        }
        private async void OnItemSelectedSort(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentSort[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }

        private void SearchContent(object sender, EventArgs e)
        {
            _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            string searchQuery = searchBar.Text;
            if (searchQuery != "")
            {
                All.IsVisible = false;
                Serial.IsVisible = false;
                Anime.IsVisible = false;
                Film.IsVisible = false;
                Dorama.IsVisible = false;
                Mult.IsVisible = false;
                Docum.IsVisible = false;
                Other.IsVisible = false;
                Viewed.IsVisible = false;
                Process.IsVisible = false;
                NotStart.IsVisible = false;
                SearchList.IsVisible = false;
                Sort.IsVisible = true;
                SortLabel.Text = $"Искомый контент по запросу \"{searchQuery}\"";
                AllContentPage viewModel = new AllContentPage();
                BindingContext = viewModel;
                List<Content> contents = databaseService.GetAllContent().ToList();

                List<Content> filteredContents = contents.Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                ContentSort = filteredContents.ToList();
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
                //Filling();
                ViewData();
            }
        }
    }
}