using CineChronicle.Application;
using CineChronicle.Application.MainPage;
using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;

namespace TestProject
{

    public partial class AddMoreContentPage : ContentPage 
    { 

        private DatabaseServiceContent _databaseService;

        #region [Ctor's]
        public AddMoreContentPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            LastWatchedSeriesEntry.TextChanged += LastWatchedSeriesEntry_TextChanged;
            LastWatchedSeasonEntry.TextChanged += LastWatchedSeasonEntry_TextChanged;
        }
        public AddMoreContentPage(ContentRecommendation data)
        {
            InitializeComponent();
            TitleEntry.Text = data.Title;
            if (data.Type == ContentTypes.SERIAL)
            {
                // Найти объект элемента "Сериал" в списке элементов TypePicker
                var selectedType = TypePicker.ItemsSource.Cast<string>().FirstOrDefault(item => item == ContentTypes.SERIAL);

                // Присвоить найденный объект элемента в SelectedItem
                TypePicker.SelectedItem = selectedType;
            }
            else if (data.Type == ContentTypes.ANIME)
            {
                // Найти объект элемента "Сериал" в списке элементов TypePicker
                var selectedType = TypePicker.ItemsSource.Cast<string>().FirstOrDefault(item => item == ContentTypes.ANIME);

                // Присвоить найденный объект элемента в SelectedItem
                TypePicker.SelectedItem = selectedType;
            }
            else if (data.Type == ContentTypes.CARTOON)
            {
                // Найти объект элемента "Сериал" в списке элементов TypePicker
                var selectedType = TypePicker.ItemsSource.Cast<string>().FirstOrDefault(item => item == ContentTypes.CARTOON);

                // Присвоить найденный объект элемента в SelectedItem
                TypePicker.SelectedItem = selectedType;
            }
            
            _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            LastWatchedSeriesEntry.TextChanged += LastWatchedSeriesEntry_TextChanged;
            LastWatchedSeasonEntry.TextChanged += LastWatchedSeasonEntry_TextChanged;

        }
        #endregion

        #region [Handle saving content]
        private async void OnAddClicked(object sender, EventArgs e)
        {
            EnabledElements(false);
            await ShowLoadingAnimation();
            string title = TitleEntry?.Text;
            string type = TypePicker.SelectedItem?.ToString();
            title = title.TrimEnd();
            if (!await CheckNullFields(title, type)) return;
            if (!await CheckExistingContent(title)) return;

            CineChronicle.Application.DeviceInfo device = new();
            GetParsingInfo getParsingInfo = new();

            // Парсим данные если есть интернет подключение
            if (device.CheckInternetConnection())
            {
                bool parseSuccess = await getParsingInfo.GetData(type, title);
                if (!parseSuccess)
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные", "OK");
                    EnabledElements(true);
                    await HideLoadingAnimation();
                    return;
                }
                // Если название неправильно указано
                if (string.IsNullOrEmpty(getParsingInfo.Description) || getParsingInfo.RealTitle != title)
                {
                    bool result = await DisplayAlert("Проверка названия",
                            $"Вы уверены, что ваш контент называется '{title}', а не '{getParsingInfo.RealTitle}'?\n\n" +
                            "Если правильное название второе, нажмите \"Да\"",
                            "Да",
                            "Нет");
                    if (result)
                    {
                        title = getParsingInfo.RealTitle;
                        bool parseSuccessawait = await getParsingInfo.GetData(type, title);
                    }
                }
            }

            string link = GetSourcesLink(type);
            string dubbing = DubbingEntry.Text;
            string dateAdded = GetTodaysDate().ToString("yyyy-MM-dd HH:mm:ss");
            string statusWatches = WatchStatusPicker.SelectedItem?.ToString();
            int lastWatchedSeries = int.TryParse(LastWatchedSeriesEntry.Text, out var series) ? series : 0;
            int lastWatchedSeason = int.TryParse(LastWatchedSeasonEntry.Text, out var season) ? season : 0;

            // Создаем новый экземпляр контента
            var newContent = new Content
            {
                CountLabel = getParsingInfo?.CountLabel,
                DateAdded = dateAdded,
                Description = getParsingInfo?.Description,
                Dubbing = dubbing,
                DateRelease = getParsingInfo?.DateRelease,
                Image = string.IsNullOrEmpty(getParsingInfo?.Image) ? "notwificonnection.jpg" : getParsingInfo.Image,
                LastWatchedSeason = lastWatchedSeason,
                LastWatchedSeries = lastWatchedSeries,
                NextEpisodeReleaseDate = getParsingInfo?.NextEpisodeReleaseDate,
                OriginalTitle = getParsingInfo?.OriginalTitle,
                SeriesChangeDate = string.Empty,
                SourceLink = link,
                Title = title,
                Type = type,
                WatchStatus = string.IsNullOrEmpty(statusWatches) ? "Не начинал" : statusWatches,
                YouTubeLink = getParsingInfo?.YouTubeLink,
                YouTubeBackground = getParsingInfo?.YouTubeBackground
            };

            _databaseService.InsertContent(newContent);

            ClearInputFields();
            EnabledElements(true);
            await HideLoadingAnimation();
            await DisplayAlert("Уведомление", "Ваши данные сохранены", "Oк");

            Navigation.RemovePage(this);
            // Переход к главному экрану
            await Shell.Current.GoToAsync("//Main");
            
        }

        #endregion

        #region [Play/Stop Animation]
        private async Task ShowLoadingAnimation()
        {
            Overlay.IsVisible = true;
            SavingAnimation.IsVisible = true;
            SavingAnimation.Opacity = 1; // Убедитесь, что анимация видима
            await SavingAnimation.FadeTo(1, 0); // Убедитесь, что анимация начинает с полной непрозрачности
           // SavingAnimation.Play(); // Запустите анимацию, если это возможно
        }

        private async Task HideLoadingAnimation()
        {
            await SavingAnimation.FadeTo(0, 250);
            SavingAnimation.IsVisible = false;
            Overlay.IsVisible = false;
        }
        #endregion

        #region [Methods for saving content]
        private void EnabledElements(bool IsEnabled)
        {
            TitleEntry.IsEnabled = IsEnabled;
            TypePicker.IsEnabled = IsEnabled;
            DubbingEntry.IsEnabled = IsEnabled;
            WatchStatusPicker.IsEnabled = IsEnabled;
            LastWatchedSeriesEntry.IsEnabled = IsEnabled;
            LastWatchedSeasonEntry.IsEnabled = IsEnabled;
            LinkEntry.IsEnabled = IsEnabled;
        }

        //Получение сегодняшней даты
        private static DateTime GetTodaysDate()
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);
            return newDate;
        }

        //Получение ссылки для контента
        private string GetSourcesLink(string type)
        {
            if (string.IsNullOrEmpty(LinkEntry.Text))
            {
                switch (type)
                {
                    case ContentTypes.ANIME:
                        return "https://animego.org/search/all?q=" + TitleEntry.Text;
                    case ContentTypes.DORAMA:
                        return "https://dorama.land/search?q=" + TitleEntry.Text;
                    case ContentTypes.SERIAL:
                    case ContentTypes.CARTOON:
                    case ContentTypes.FILM:
                        return "https://kinogo.biz/search/" + TitleEntry.Text;
                    default:
                        return "https://kinogo.biz/search/" + TitleEntry.Text;
                }
            }
            else
            {
                return LinkEntry.Text;
            }
        }

        //Проверка на нулевые значения
        private async Task<bool> CheckNullFields(string title, string type)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(type))
            {
                await DisplayAlert("Уведомление", "Заполните поля \"Название\" и \"Тип\" для возможности сохранения", "OK");
                EnabledElements(true);
                await HideLoadingAnimation();
                return false;
            }
            return true;
        }

        //Проверка существующего контента
        private async Task<bool> CheckExistingContent(string title)
        {
            List<Content> contents = _databaseService.GetAllContent().ToList();

            List<Content> filteredContents = contents.Where(c => c.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (filteredContents.Count != 0)
            {
                bool result = await DisplayAlert("Уведомление", $"Похоже {title} уже существует, вы уверены, что хотите создать копию?", "Да", "Нет");

                if (!result)
                {
                    ClearInputFields();
                    EnabledElements(true);
                    await HideLoadingAnimation();
                    return false;
                }
            }
            return true;
        }

        //Очистка полей после сохранения
        private void ClearInputFields()
        {
            TitleEntry.Text = string.Empty;
            DubbingEntry.Text = string.Empty;
            LastWatchedSeriesEntry.Text = string.Empty;
            LastWatchedSeasonEntry.Text = string.Empty;
            LinkEntry.Text = string.Empty;
        }
        #endregion

        #region [Other event handlers]

        private void LastWatchedSeriesEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем каждый введенный символ в поле
            foreach (char c in e.NewTextValue)
            {
                // Если символ не является цифрой, удаляем его из текста
                if (!char.IsDigit(c))
                {
                    LastWatchedSeriesEntry.Text = LastWatchedSeriesEntry.Text.Remove(LastWatchedSeriesEntry.Text.Length - 1);
                    break;
                }
            }
        }

        private void LastWatchedSeasonEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем каждый введенный символ в поле
            foreach (char c in e.NewTextValue)
            {
                // Если символ не является цифрой, удаляем его из текста
                if (!char.IsDigit(c))
                {
                    LastWatchedSeasonEntry.Text = LastWatchedSeasonEntry.Text.Remove(LastWatchedSeasonEntry.Text.Length - 1);
                    break;
                }
            }
        }
        #endregion
    }
}
