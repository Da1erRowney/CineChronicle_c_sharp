using CineChronicle.Application;
using CineChronicle.Tables;
using System.Windows.Input;


namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ViewContentPage : ContentPage
    {
        #region [Constants]
        private const string WIK = "Википедия";
        private const string KO = "Kinogo";
        private const string JS = "Jutsu";
        private const string AG = "AnimeGo";
        private const string PG = "PremierGo";
        private const string YT = "YouTube";
        private const string LF = "LordsFilm";
        private const string DE = "DateExit";

        private const string ANIME = "Аниме";
        private const string FILM = "Фильм";
        private const string SERIAL = "Сериал";
        private const string DORAMA = "Дорама";
        private const string OTHER = "Прочее";
        private const string CARTOON = "Мультсериал";
        #endregion

        #region [Private fields]
        public ICommand OpenLinkCommand { get; private set; }

        private ContentRecommendation recom;
        private Content content;
        private DateExit data;
        private GetParsingInfo parser = new();

        private bool isEditing = false; // Флаг, указывающий, в режиме редактирования или нет

        #endregion

        #region [Ctor's]
        public ViewContentPage(Content content)
        {
            this.content = content;
            InitializeComponent();
            BindingContext = content; // Привязываем объект Content к BindingContext страницы
            SetupLabelTappedEvents();
            OpenLinkCommand = new Command<string>(OpenLink);
        }

        public ViewContentPage(ContentRecommendation content)
        {
            this.recom = content;
            InitializeComponent();
            content.Type = recom.Type;
            BindingContext = content; // Привязываем объект Content к BindingContext страницы
            SetupLabelTappedEventsRecom();
            OpenLinkCommand = new Command<string>(OpenLink);
            Statics.IsVisible = false;
            DubbingPo.IsVisible = false;
            StatusP.IsVisible = false;
            EditButton.IsVisible = false;
            DeleteButton.IsVisible = false;
            AddButton.IsVisible = true;
        }
        #endregion


        private async void GetTrailers(string query, string type)
        {
            await Browser.OpenAsync(new Uri(parser.YouTube), BrowserLaunchMode.SystemPreferred); 
        }

        private async void SetupLabelTappedEvents()
        {
            // Включите метод для получения информации с веб-сайта при загрузке страницы
            string title = (BindingContext as Content)?.Title;
            string type = (BindingContext as Content)?.Type;

            GetInfo(type, title);
        }

        private async void SetupLabelTappedEventsRecom()
        {
            // Включите метод для получения информации с веб-сайта при загрузке страницы
            string title = (BindingContext as ContentRecommendation)?.Title;
            string type = (BindingContext as ContentRecommendation)?.Type;

            GetInfo(type, title);
        }

        private void GetInfo(string type, string title)
        {
            string typePars = string.Empty;
            switch (type)
            {
                case ANIME:
                    typePars = AG;
                    WatchingButton.Source = "anime.png";
                    break;
                case FILM:
                case SERIAL:
                case CARTOON:
                case DORAMA:
                case OTHER:
                    typePars = WIK;
                    WatchingButton.Source = "movie.png";
                    break;
                default:
                    WatchingButton.Source = "movie.png";
                    break;
            }

            PushParser(type, title, typePars);
            //ShowInformation();
        }

        //Отображение информации пользователю
        private void ShowInformation()
        {
            DescriptionLabel.Text = parser?.Description;
            PosterImage.Source = ImageSource.FromUri(new Uri(parser?.Image));
            Background.Source = ImageSource.FromUri(new Uri(parser?.Image));
            TrailerWeb.Source = parser?.YouTube;
            NextEpisodeReleaseDateEntry.Text = parser?.NextEpisodeReleaseDate;
            CountLabel.Text = parser?.CountLabel;
        }

        //Запуск парсера
        private async void PushParser(string type, string title, string typePars)
        {
            parser.DescriptionRead += HandleLoadDescription;
            parser.ImageRead += HandleLoadImage;
            parser.YouTubeRead += HandleLoadYouTube;
            parser.NextEpisodeReleaseDateRead += HandleLoadNextEpisode;
            parser.CountLabelRead += HandleLoadCount;
            parser.DateReleaseRead += HandleLoadDateRelease;

            try
            {
                var task1 = parser.GetInfo(title, type, typePars, false);
                var task2 = parser.GetInfo(title, type, typePars, true);
                var task3 = parser.GetInfo(title, type, YT, false);
                var task4 = parser.GetInfo(title, type, DE, false);

                await Task.WhenAll(task1);
                await Task.WhenAll(task2);
                await Task.WhenAll(task3);
                await Task.WhenAll(task4);

                await Task.Delay(100);
            }
            finally
            {
                parser.DescriptionRead -= HandleLoadDescription;
                parser.ImageRead -= HandleLoadImage;
                parser.YouTubeRead -= HandleLoadYouTube;
                parser.NextEpisodeReleaseDateRead -= HandleLoadNextEpisode;
                parser.CountLabelRead -= HandleLoadCount;
                parser.DateReleaseRead -= HandleLoadDateRelease;
            }
        }

        private void HandleLoadDescription(GetParsingInfo.DescriptionResult result)
        {
            if (!string.IsNullOrEmpty(result.Description))
            {
                Console.WriteLine($"Описание: {result.Description}");
                DescriptionLabel.Text = result.Description;
            }
            
        }
        private void HandleLoadImage(GetParsingInfo.ImageResult result)
        {
            if (!string.IsNullOrEmpty(result.Image))
            {
                Console.WriteLine($"Картинка: {result.Image}");
                PosterImage.Source = ImageSource.FromUri(new Uri(parser?.Image));
                Background.Source = ImageSource.FromUri(new Uri(parser?.Image));
            }
        }
        private void HandleLoadYouTube(GetParsingInfo.YouTubeResult result)
        {
            if (!string.IsNullOrEmpty(result.YouTube))
            {
                Console.WriteLine($"Ссылка на трейлер: {result.YouTube}");
                TrailerWeb.Source = parser?.YouTube;
            }
        }
        private void HandleLoadNextEpisode(GetParsingInfo.NextEpisodeReleaseDateResult result)
        {
            if (!string.IsNullOrEmpty(result.NextEpisodeReleaseDate))
            {
                Console.WriteLine($"Следующий эпизод: {result.NextEpisodeReleaseDate}");
                NextEpisodeReleaseDateEntry.Text = parser?.NextEpisodeReleaseDate;
            }
        }
        private void HandleLoadCount(GetParsingInfo.CountLabelResult result)
        {
            if (!string.IsNullOrEmpty(result.CountLabel))
            {
                Console.WriteLine($"Количество: {result.CountLabel}");
                CountLabel.Text = parser?.CountLabel;
            }
        }
        private void HandleLoadDateRelease(GetParsingInfo.DateReleaseResult result)
        {
            if (!string.IsNullOrEmpty(result.DateRelease))
                Console.WriteLine($"Дата выхода: {result.DateRelease}");
        }

        // Открываем ссылку в браузере
        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }

        //Кнопка удаления контента
        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Уведомление", $"Вы уверены, что хотите удалить {content.Title}?", "Да", "Нет");

            if (result)
            {
                TapDelete();
                await Navigation.PopAsync();
            }
        }

        //Пользователь выбрал удалить контент
        private void TapDelete()
        {
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);

            databaseService.DeleteContent(content);

            databaseService.CloseConnection();
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            if (isEditing) SaveChanges();   // Если уже в режиме редактирования, то нужно сохранить изменения
            else StartEditing();            // Если не в режиме редактирования, то переключиться в этот режим
        }

        private void HideElements(bool isVisible)
        {
            // Разблокировать поля ввода
            LinkSecondLabel.IsVisible = isVisible;
            LinkEntry.IsVisible = isVisible;
            LinkEntry.IsReadOnly = isVisible;
            TrailerWebBorder.IsVisible = isVisible;

            DecriptionBorder.IsVisible = isVisible;

            ViewContent.IsVisible = isVisible;
            TypeEntry.IsVisible = isVisible;
            TypeLabel.IsVisible = isVisible;
            WatchStatusEntry.IsVisible = isVisible;
            WatchStatusLabel.IsVisible = isVisible;

            TitleEntry.IsReadOnly = isVisible;
            DubbingEntry.IsReadOnly = isVisible;
            LastWatchedSeriesEntry.IsReadOnly = isVisible;
            LastWatchedSeasonEntry.IsReadOnly = isVisible;

            DataLabel.IsVisible = isVisible;
            NextEpisodeReleaseDateEntry.IsVisible = isVisible;
            CountLabel.IsVisible = isVisible;

            WatchStatusEntry.IsReadOnly = isVisible;

            TypePicker.IsVisible = isVisible;
            WatchStatusPicker.IsVisible = isVisible;
        }

        private void StartEditing()
        {
            isEditing = true;
            EditButton.Text = "Сохранить";
            CancelButton.IsVisible = true; // Отобразить кнопку "Отмена"

            HideElements(false);

            TypePicker.IsVisible = true;
            TypePicker.SelectedItem = content.Type;

            WatchStatusPicker.IsVisible = true;
            WatchStatusPicker.SelectedItem = content.WatchStatus;
           
        }

        private void SaveChanges()
        {
            isEditing = false;
            EditButton.Text = "Изменить";
            CancelButton.IsVisible = false;  // Скрыть кнопку "Отмена"

            HideElements(false);

            ReturnVisibleAfterChanges();

            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            content = databaseService.GetContentById(content.Id);
            content.Title = TitleEntry.Text;
            content.Type = TypePicker.SelectedItem.ToString();
            content.Dubbing = DubbingEntry.Text;
            if (LastWatchedSeriesEntry.Text != content.LastWatchedSeries.ToString() || LastWatchedSeasonEntry.Text != content.LastWatchedSeason.ToString())
            {
                DateTime currentDate = DateTime.UtcNow;
                DateTime newDate = currentDate.AddHours(+3);
                content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                content.SeriesChangeDate = content.SeriesChangeDate;

            }
            switch (content.Type)
            {
                case ANIME:
                    content.Link = "https://animego.org/search/all?q=" + TitleEntry.Text;
                    break;
                case DORAMA:
                    content.Link = "https://dorama.land/search?q=" + TitleEntry.Text;
                    break;
                case SERIAL:
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case CARTOON:
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case FILM:
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                default:
                    break;
            }

            content.LastWatchedSeries = int.Parse(LastWatchedSeriesEntry.Text);
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);

            content.NextEpisodeReleaseDate = NextEpisodeReleaseDateEntry.Text;
            content.WatchStatus = WatchStatusPicker.SelectedItem.ToString();
            content.DateAdded = content.DateAdded;
            databaseService.UpdateContent(content);
            databaseService.CloseConnection();
            SetupLabelTappedEvents();

            // Обновить данные в БД
            // Ваш код для обновления данных в БД
        }

        private void ReturnVisibleAfterChanges()
        {
            TrailerWebBorder.IsVisible = true;
            ViewContent.IsVisible = true;
            DecriptionBorder.IsVisible = true;
            TypeEntry.IsVisible = true;
            TypeLabel.IsVisible = true;
            WatchStatusEntry.IsVisible = true;
            WatchStatusLabel.IsVisible = true;
            DataLabel.IsVisible = true;
            NextEpisodeReleaseDateEntry.IsVisible = true;
            CountLabel.IsVisible = true;
            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Отменить изменения и переключиться из режима редактирования
            isEditing = false;
            EditButton.Text = "Изменить";
            CancelButton.IsVisible = false; // Скрыть кнопку "Отмена"

            // Получаем путь к базе данных


            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);

            // Получаем данные из базы данных по ID
            content = databaseService.GetContentById(content.Id);

            // Проверяем наличие данных в объекте content
            if (content != null)
            {
                // Заполняем поля ввода данными из объекта content
                TitleEntry.Text = content.Title;
                TypeEntry.Text = content.Type;
                DubbingEntry.Text = content.Dubbing;
                LastWatchedSeriesEntry.Text = content.LastWatchedSeries.ToString();
                LastWatchedSeasonEntry.Text = content.LastWatchedSeason.ToString();
                NextEpisodeReleaseDateEntry.Text = content.NextEpisodeReleaseDate;
                WatchStatusEntry.Text = content.WatchStatus;

            }

            HideElements(true);

            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = false;
            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;
        }

        private void TrailerButton_Clicked(object sender, EventArgs e)
        {
            GetTrailers(content.Title, content.Type);
        }

        private async void WatchingButton_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(content.Link), BrowserLaunchMode.SystemPreferred);
        }

        private void StepperSeries_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newValue = e.NewValue;
            LastWatchedSeriesEntry.Text = newValue.ToString();
            SaveStepperAfterChange();
        }

        private void StepperSeason_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newValue = e.NewValue;
            LastWatchedSeasonEntry.Text = newValue.ToString();
            SaveStepperAfterChange();
        }

        private void SaveStepperAfterChange()
        {
            if (LastWatchedSeasonEntry.Text == null || LastWatchedSeriesEntry.Text == null) return;
            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            content = databaseService.GetContentById(content.Id);
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);
            content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);
            content.LastWatchedSeries = int.Parse(LastWatchedSeriesEntry.Text);
            databaseService.UpdateContent(content);
            databaseService.CloseConnection();
        }

        private async void ImageTapped(object sender, EventArgs e)
        {
            AddMoreContentPage addMoreContentPage = new AddMoreContentPage(recom);
            await Navigation.PushAsync(addMoreContentPage);
        }
    }

}

  
