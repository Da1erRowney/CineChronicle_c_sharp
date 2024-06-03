using DataContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using SQLite;
using Microsoft.Maui.Platform;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Input;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllContentPage : ContentPage
    { 
    
        public string Choise ="All";
        public List<Content> ContentAll { get; set; }
        public List<Content> ContentSerial { get; set; }
        public List<Content> ContentAnime { get; set; }
        public List<Content> ContentFilm { get; set; }
        public List<Content> ContentDorama { get; set; }
        public List<Content> ContentMult { get; set; }
        public List<Content> ContentDocum { get; set; }
        public List<Content> ContentOther { get; set; }
        public List<Content> ContentViewed { get; set; }
        public List<Content> ContentProcess { get; set; }
        public List<Content> ContentNotStart { get; set; }

        public List<Content> ContentAllall { get; set; }
        public List<Content> ContentSerialall { get; set; }
        public List<Content> ContentAnimeall { get; set; }
        public List<Content> ContentFilmall { get; set; }
        public List<Content> ContentDoramaall { get; set; }
        public List<Content> ContentMultall { get; set; }
        public List<Content> ContentDocumall { get; set; }
        public List<Content> ContentOtherall { get; set; }
        public List<Content> ContentViewedall { get; set; }
        public List<Content> ContentProcessall { get; set; }
        public List<Content> ContentNotStartall { get; set; }

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

        private DatabaseServiceContent _databaseService;

        public AllContentPage()
        {
            InitializeComponent();
            Filling();
        }
        public void Filling()
        {
            _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);

            List<Content> contents = databaseService.GetAllContent().ToList();
            ContentAll = contents.Take(8).ToList();
            ContentAllall = contents.ToList();

            ContentSerialall = databaseService.GetContentByType("Сериал").ToList();
            ContentAnimeall = databaseService.GetContentByType("Аниме").ToList();
            ContentFilmall = databaseService.GetContentByType("Фильм").ToList();
            ContentDoramaall = databaseService.GetContentByType("Дорама").ToList();
            ContentMultall = databaseService.GetContentByType("Мультсериал").ToList();
            ContentDocumall = databaseService.GetContentByType("Документалка").ToList();
            ContentOtherall = databaseService.GetContentByType("Прочее").ToList();
            ContentViewedall = databaseService.GetContentByWatchStatus("Просмотрено").ToList();
            ContentProcessall = databaseService.GetContentByWatchStatus("Смотрю").ToList();
            ContentNotStartall = databaseService.GetContentByWatchStatus("Не начинал").ToList();

            ContentSerial   = ContentSerialall.Take(8).ToList();
            ContentAnime    = ContentAnimeall.Take(8).ToList();
            ContentFilm     = ContentFilmall.Take(8).ToList();
            ContentDorama   = ContentDoramaall.Take(8).ToList();
            ContentMult     = ContentMultall.Take(8).ToList();
            ContentDocum    = ContentDocumall.Take(8).ToList();
            ContentOther    = ContentOtherall.Take(8).ToList();
            ContentViewed   = ContentViewedall.Take(8).ToList();
            ContentProcess  = ContentProcessall.Take(8).ToList();
            ContentNotStart = ContentNotStartall.Take(8).ToList();



            OnPropertyChanged(nameof(ContentAll));
            OnPropertyChanged(nameof(ContentSerial));
            OnPropertyChanged(nameof(ContentAnime));
            OnPropertyChanged(nameof(ContentDorama));
            OnPropertyChanged(nameof(ContentMult));
            OnPropertyChanged(nameof(ContentFilm));
            OnPropertyChanged(nameof(ContentDocum));
            OnPropertyChanged(nameof(ContentOther));
            OnPropertyChanged(nameof(ContentViewed));
            OnPropertyChanged(nameof(ContentProcess));
            OnPropertyChanged(nameof(ContentNotStart));
            OnPropertyChanged(nameof(ContentSort));

            string search = searchBar.Text;
            if (search != null && search !="")
            {
                Sort.IsVisible = true;
                VisibleFalse();
            }
            else
            {
                Sort.IsVisible = false;
                ViewData();
            }

            BindingContext = this;
        }
       
           
        private void ViewData()
        {

            if (ContentAll.Count == 0)
            {
                All.IsVisible = false;
            }
            else
            {
                All.IsVisible = true;
            }
            if (ContentSerial.Count == 0)
            {
                Serial.IsVisible = false;
            }
            else
            {
                Serial.IsVisible = true;
            }
            if (ContentAnime.Count == 0)
            {
                Anime.IsVisible = false;
            }
            else
            {
                Anime.IsVisible = true;
            }
            if (ContentFilm.Count == 0)
            {
                Film.IsVisible = false;
            }
            else
            {
                Film.IsVisible = true;
            }
            if (ContentDorama.Count == 0)
            {
                Dorama.IsVisible = false;
            }
            else
            {
                Dorama.IsVisible = true;
            }
            if (ContentMult.Count == 0)
            {
                Mult.IsVisible = false;
            }
            else
            {
                Mult.IsVisible = true;
            }
            if (ContentDocum.Count == 0)
            {
                Docum.IsVisible = false;
            }
            else
            {
                Docum.IsVisible = true;
            }
            if (ContentOther.Count == 0)
            {
                Other.IsVisible = false;
            }
            else
            {
                Other.IsVisible = true;
            }
            if (ContentViewed.Count == 0)
            {
                Viewed.IsVisible = false;
            }
            else
            {
                Viewed.IsVisible = true;
            }
            if (ContentProcess.Count == 0)
            {
                Process.IsVisible = false;
            }
            else
            {
                Process.IsVisible = true;
            }
            if (ContentNotStart.Count == 0)
            {
                NotStart.IsVisible = false;
            }
            else
            {
                NotStart.IsVisible = true;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Filling();
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

        public async void GetContent(string type, string name)
        {
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            List<Content> contents = databaseService.GetContentByType(type).ToList();
            ContentSort = contents;
            ViewCategoryPage viewContentPage = new ViewCategoryPage(ContentSort, name);
            await Navigation.PushAsync(viewContentPage);
        }

        private void АнимеButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Всё ваше Аниме";
            GetContent("Аниме", nameCategory);
        }

        private void ФильмыButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Все ваши Фильмы";
            GetContent("Фильм", nameCategory);
        }

        private void СериалыButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Все ваши Сериалы";
            GetContent("Сериал", nameCategory);
        }

        private void ДорамыButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Все ваши Дорамы";
            GetContent("Дорама", nameCategory);
        }

        private void МультсериалыButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Все ваши Мультсериалы";
            GetContent("Мультсериал", nameCategory);
        }

        private void ДокументалкиButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Все ваши Документальные фильмы";
            GetContent("Документалка", nameCategory);
        }
        private async void ВесьКонтентClicked(object sender, EventArgs e)
        {
            string nameCategory = "Весь ваш контент";
            ContentSort = ContentAllall;
            ViewCategoryPage viewContentPage = new ViewCategoryPage(ContentSort, nameCategory);
            await Navigation.PushAsync(viewContentPage);
        }
        private void ПрочееButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Ваш прочий контент";
            GetContent("Прочее", nameCategory);
        }

        private void ПросмотреноButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Просмотренный контент";
            GetContent("Просмотрено", nameCategory);
        }

        private void ВпроцессеButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Контент, который вы начали смотреть";
            GetContent("Смотрю", nameCategory);
        }

        private void НеначатоButton_Clicked(object sender, EventArgs e)
        {
            string nameCategory = "Не начатый контент";
            GetContent("Не начинал", nameCategory);
        }

        private async void OnItemSelectedAll(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentAll[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
            
        }

        private void ItemButtonClickedAll(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)AllContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedAll(item, selectedIndex);
        }


        private void ItemButtonClickedSerial(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)SerialContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedSerial(item, selectedIndex);
        }

        private async void OnItemSelectedSerial(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentSerial[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedAnime(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)AnimeContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedAnime(item, selectedIndex);
        }

        private async void OnItemSelectedAnime(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentAnime[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedFilm(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)FilmContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedFilm(item, selectedIndex);
        }

        private async void OnItemSelectedFilm(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentFilm[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedDorama(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)DoramaContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedDorama(item, selectedIndex);
        }

        private async void OnItemSelectedDorama(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentDorama[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedMult(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)MultContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedMult(item, selectedIndex);
        }

        private async void OnItemSelectedMult(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentMult[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedDocum(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)DocumContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedDocum(item, selectedIndex);
        }

        private async void OnItemSelectedDocum(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentDocum[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedOther(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)OtherContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedOther(item, selectedIndex);
        }

        private async void OnItemSelectedOther(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentOther[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedViewed(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)ViewedContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedViewed(item, selectedIndex);
        }

        private async void OnItemSelectedViewed(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentViewed[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedProcess(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)ProcessContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedProcess(item, selectedIndex);
        }

        private async void OnItemSelectedProcess(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentProcess[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }


        private void ItemButtonClickedNotStart(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)NotStartContentCollectionView.ItemsSource).IndexOf(item);
            OnItemSelectedNotStart(item, selectedIndex);
        }

        private async void OnItemSelectedNotStart(Content item, int selectedIndex)
        {
            if (item == null)
                return;

            Content selectedContent = ContentNotStart[selectedIndex];
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);
            await Navigation.PushAsync(viewContentPage);
        }
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
                Filling();
                ViewData();
            }
        }

        private void ItemButtonClickedSort(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (Content)button.CommandParameter;
            var selectedIndex = new List<Content>((IEnumerable<DataContent.Content>)SortContentCollectionView.ItemsSource).IndexOf(item);
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
                Filling();
                ViewData();
            }
        }
    }
}