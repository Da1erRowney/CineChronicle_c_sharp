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

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllContentPage : ContentPage
    {
        private string Choise;
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

        private List<Content> _contentSearch;
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
        public SQLiteConnection CreateDatabase(string databasePath)
        {
            SQLiteConnection connection = new SQLiteConnection(databasePath);
            connection.CreateTable<Content>();
            return connection;

        }

        public AllContentPage()
        {
            InitializeComponent();
            Choise = "All";

            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
            _databaseService = new DatabaseServiceContent(databasePath);
            SQLiteConnection connection = CreateDatabase(databasePath);
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

            List<Content> contents = databaseService.GetAllContent().ToList();
            ContentAll = contents.ToList();

            List<Content> filteredContents = contents.Where(c => c.Type == "Сериал").ToList();
            ContentSerial= filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Аниме").ToList();
            ContentAnime = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Фильм").ToList();
            ContentFilm = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Дорама").ToList();
            ContentDorama = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Мультсериал").ToList();
            ContentMult = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Документалка").ToList();
            ContentDocum = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Прочее").ToList();
            ContentOther = filteredContents.ToList();

            filteredContents = contents.Where(c => c.WatchStatus == "Просмотрено").ToList();
            ContentViewed = filteredContents.ToList();

            filteredContents = contents.Where(c => c.WatchStatus == "Смотрю").ToList();
            ContentProcess = filteredContents.ToList();

            filteredContents = contents.Where(c => c.WatchStatus == "Не начинал").ToList();
            ContentNotStart = filteredContents.ToList();

            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateContent();
           
        }

        private void UpdateContent()
        {
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
            _databaseService = new DatabaseServiceContent(databasePath);
            SQLiteConnection connection = CreateDatabase(databasePath);
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

            List<Content> contents = databaseService.GetAllContent().ToList();
            ContentAll = contents.ToList();

            List<Content> filteredContents = contents.Where(c => c.Type == "Сериал").ToList();
            ContentSerial = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Аниме").ToList();
            ContentAnime = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Фильм").ToList();
            ContentFilm = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Дорама").ToList();
            ContentDorama = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Мультсериал").ToList();
            ContentMult = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Документалка").ToList();
            ContentDocum = filteredContents.ToList();

            filteredContents = contents.Where(c => c.Type == "Прочее").ToList();
            ContentOther = filteredContents.ToList();

            filteredContents = contents.Where(c => c.WatchStatus == "Просмотрено").ToList();
            ContentViewed = filteredContents.ToList();

            filteredContents = contents.Where(c => c.WatchStatus == "Смотрю").ToList();
            ContentProcess = filteredContents.ToList();

            filteredContents = contents.Where(c => c.WatchStatus == "Не начинал").ToList();
            ContentNotStart = filteredContents.ToList();

            BindingContext = this;
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

        }

        private async void АнимеButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = true;
            Film.IsVisible = false;
            Dorama.IsVisible = false;
            Mult.IsVisible = false;
            Docum.IsVisible = false;
            Other.IsVisible = false;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void ФильмыButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = true;
            Dorama.IsVisible = false;
            Mult.IsVisible = false;
            Docum.IsVisible = false;
            Other.IsVisible = false;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void СериалыButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = true;
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

        private async void ДорамыButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = false;
            Dorama.IsVisible = true;
            Mult.IsVisible = false;
            Docum.IsVisible = false;
            Other.IsVisible = false;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void МультсериалыButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = false;
            Dorama.IsVisible = false;
            Mult.IsVisible = true;
            Docum.IsVisible = false;
            Other.IsVisible = false;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void ДокументалкиButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = false;
            Dorama.IsVisible = false;
            Mult.IsVisible = false;
            Docum.IsVisible = true;
            Other.IsVisible = false;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void ПрочееButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = false;
            Dorama.IsVisible = false;
            Mult.IsVisible = false;
            Docum.IsVisible = false;
            Other.IsVisible = true;
            Viewed.IsVisible = false;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void ПросмотреноButton_Clicked(object sender, EventArgs e)
        {
            All.IsVisible = false;
            Serial.IsVisible = false;
            Anime.IsVisible = false;
            Film.IsVisible = false;
            Dorama.IsVisible = false;
            Mult.IsVisible = false;
            Docum.IsVisible = false;
            Other.IsVisible = false;
            Viewed.IsVisible = true;
            Process.IsVisible = false;
            NotStart.IsVisible = false;
        }

        private async void ВпроцессеButton_Clicked(object sender, EventArgs e)
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
            Process.IsVisible = true;
            NotStart.IsVisible = false;
        }

        private async void НеначатоButton_Clicked(object sender, EventArgs e)
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
            NotStart.IsVisible = true;
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
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
            _databaseService = new DatabaseServiceContent(databasePath);
            SQLiteConnection connection = CreateDatabase(databasePath);
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

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
                SearchList.IsVisible = true;
                AllContentPage viewModel = new AllContentPage();
                BindingContext = viewModel;
                List<Content> contents = databaseService.GetAllContent().ToList();

                List<Content> filteredContents = contents.Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                viewModel.ContentSearch = filteredContents.ToList();

            }
            else
            {
                SearchList.IsVisible = false;
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
                UpdateContent();
            }

           

        }
    }
}