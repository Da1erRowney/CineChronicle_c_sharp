using CineChronicle.Application;
using CineChronicle.Application.MainPage;
using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModel;
using CineChronicle.Tables;
using System.Windows.Input;


namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ViewContentPage : ContentPage
    {
        #region [Private fields]
        public ICommand OpenLinkCommand { get; private set; }
        private ContentRecommendation recom;
        private Content content;
        private DateExit data;
        private GetParsingInfo parser = new();

        private bool isEditing = false; // Флаг, указывающий, в режиме редактирования или нет
        private string _oldName;
        private string _oldType;

        #endregion

        #region [Ctor's]
        public ViewContentPage(Content content)
        {
            InitializeComponent();
            this.content = content;

            CheckPage();
        }

        public ViewContentPage(ContentRecommendation content)
        {
            this.recom = content;
            InitializeComponent();
            content.Type = recom.Type;
            BindingContext = content; // Привязываем объект Content к BindingContext страницы

            CheckPage();
            SelectRecomendetContent(); // Скрываем для рекомендаций не нужные элементы
        }

        #endregion

        #region [Refresh Data]
        private async void OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                _oldType = ""; // Инициализируйте значением по умолчанию

                if (GetNewDataAndSave != null) // Проверка, если это делегат
                {
                    await GetNewDataAndSave();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении: {ex.Message}");
                // Можно показать пользователю сообщение:
                await DisplayAlert("Ошибка", "Не удалось обновить данные", "OK");
            }
            finally
            {
                if (RefreshView != null)
                    RefreshView.IsRefreshing = false;
            }
        }
        #endregion

        #region [Main Ctor's Methods]

        /// <summary>
        /// 1. Привязываем обновленные данные в BindingContext
        /// 2. Обновляем переменные хранящие старые значения (до изменения)
        /// 3. Обновляем команду для перехода по ссылке
        /// 4. Скрываем поле с описанием, если оно пустое
        /// 5. Ставим базовый фон, если Image пуст
        /// 6. Обновляем картинку в источнике
        /// 7. Скрываем строку с озвучкой если оно пустое
        /// </summary>
        private void CheckPage()
        {
            // 1
            BindingContext = new ViewContentPageRefreshModel(content);              

            // 2
            _oldName = content.Title;
            _oldType = content.Type;

            // 3
            OpenLinkCommand = new Command<string>(OpenLink);

            // 4
            if (string.IsNullOrEmpty(content.Description))
            {
                DecriptionBorder.IsVisible = false;
            }
            else
            {
                DecriptionBorder.IsVisible = true;
            }

            // 5
            if (string.IsNullOrEmpty(content.Image))
            {
                Background.Source = "gradientfive.jpg";
            }
            else
            {
                Background.Source = content.Image;
            }

            // 6
            switch (content.Type)
            {
                case ContentTypes.ANIME:
                    WatchingButton.Source = "anime.png";
                    break;
                case ContentTypes.FILM:
                case ContentTypes.SERIAL:
                case ContentTypes.CARTOON:
                case ContentTypes.DORAMA:
                case ContentTypes.OTHER:
                    WatchingButton.Source = "movie.png";
                    break;
                default:
                    WatchingButton.Source = "movie.png";
                    break;
            }

            // 7
            if (string.IsNullOrEmpty(content.Dubbing))
            {
                DubbingPo.IsVisible = false;
            }
            else
            {
                DubbingPo.IsVisible = true;
            }

            bool showVideos = Preferences.Get("ShowVideos", true);
            if (showVideos)
            {
                TrailerWebBackground.IsVisible = true;
                Background.IsVisible = false;
            }
            else
            {
                TrailerWebBackground.IsVisible = false;
                Background.IsVisible = true;
            }
        }

        #endregion

        #region [Methods]
        // Открываем ссылку в браузере
        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }

        //Пользователь выбрал удалить контент
        private void TapDelete()
        {
            DatabaseServiceContent _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            _databaseService.DeleteContent(content);
            _databaseService.CloseConnection();
        }

        private void SaveStepperAfterChange()
        {
            if (string.IsNullOrEmpty(LastWatchedSeasonEntry?.Text) || string.IsNullOrEmpty(LastWatchedSeriesEntry?.Text)) return;

            UpdateContentData();
        }

        private void UpdateContentData()
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);
            content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");

            DatabaseServiceContent _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            _databaseService.UpdateContent(content);
            _databaseService.CloseConnection();
        }

        private async void SaveChanges()
        {
            IsEditing(false, "Изменить");

            HideElements(false);
            ReturnVisibleAfterChanges();

            await GetNewDataAndSave();
        }

        private async Task GetNewDataAndSave()
        {
            var currentContent = content;
            currentContent.WatchStatus = WatchStatusPicker.SelectedItem?.ToString() ?? content.WatchStatus;
            currentContent.Type = TypePicker.SelectedItem != null ? TypePicker.SelectedItem.ToString() : content.Type;
            currentContent.Title = currentContent.Title.TrimEnd();

            // Проверяем, поменяли ли мы тип или название для получения новых данных из парсерса
            if (_oldName != currentContent.Title || _oldType!= currentContent.Type)
            {
                await ShowLoadingAnimation();
                bool parseSuccess = await parser.GetData(currentContent.Type, currentContent.Title);

                // Если название неправильно указано
                if (string.IsNullOrEmpty(parser.Description) || parser.RealTitle != currentContent.Title)
                {
                    bool result = await DisplayAlert("Проверка названия",
                            $"Вы уверены, что ваш контент называется '{currentContent.Title}', а не '{parser.RealTitle}'?\n\n" +
                            "Если правильное название второе, нажмите \"Да\"",
                            "Да",
                            "Нет");
                    if (result)
                    {
                        currentContent.Title = parser.RealTitle;
                        bool parseSuccessawait = await parser.GetData(currentContent.Type,currentContent.Title);
                    }
                }

                currentContent.CountLabel = parser.CountLabel;
                currentContent.Description = parser.Description;
                currentContent.DateRelease = parser.DateRelease;
                currentContent.Image = parser.Image;
                currentContent.NextEpisodeReleaseDate = parser.NextEpisodeReleaseDate;
                currentContent.YouTubeLink = parser.YouTubeLink;
                currentContent.YouTubeBackground = parser.YouTubeBackground;
                await HideLoadingAnimation();
            }

            // Проверяем, поменяли ли мы тип, для получения новой ссылки на источник
            if (_oldType != currentContent.Type)
            {
                // Обновляем ссылку на источник
                switch (currentContent.Type)
                {
                    case ContentTypes.ANIME:
                        currentContent.SourceLink = "https://animego.org/search/all?q=" + TitleEntry.Text;
                        break;
                    case ContentTypes.DORAMA:
                        currentContent.SourceLink = "https://dorama.land/search?q=" + TitleEntry.Text;
                        break;
                    case ContentTypes.SERIAL:
                    case ContentTypes.CARTOON:
                    case ContentTypes.FILM:
                        currentContent.SourceLink = "https://kinogo.biz/search/" + TitleEntry.Text;
                        break;
                    default:
                        break;
                }
            }

            // Обновляем контент в базе данных
            DatabaseServiceContent _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            _databaseService.UpdateContent(currentContent);
            _databaseService.CloseConnection();

            CheckPage();
        }
        #endregion

        #region [Play/Stop Animation]
        private async Task ShowLoadingAnimation()
        {
            Overlay.IsVisible = true;
            SavingAnimation.IsVisible = true;
            SavingAnimation.Opacity = 1; // Убедитесь, что анимация видима
            await SavingAnimation.FadeTo(1, 0); // Убедитесь, что анимация начинает с полной непрозрачности
        }

        private async Task HideLoadingAnimation()
        {
            await SavingAnimation.FadeTo(0, 250);
            SavingAnimation.IsVisible = false;
            Overlay.IsVisible = false;
        }
        #endregion

        #region [Handle Method's]

        //Кнопка удаления контента
        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Уведомление", $"Вы уверены, что хотите удалить {content.Title}?", "Да", "Нет");

            if (result)
            {
                TapDelete();
                await Shell.Current.GoToAsync("//Main");
                Navigation.RemovePage(this);
            }
        }

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            if (isEditing) SaveChanges();   // Если уже в режиме редактирования, то нужно сохранить изменения
            else StartEditing();            // Если не в режиме редактирования, то переключиться в этот режим
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Возвращаем предыдущие значения
            BindingContext = null;
            DatabaseServiceContent _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            content = _databaseService.GetContentById(content.Id);
            BindingContext = new ViewContentPageRefreshModel(content);

            // Возвращаем компонентам свойства
            IsEditing(false, "Изменить");
            HideElements(true);
            HideElelmetsAfterCansel();
        }

        private async void TrailerButton_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(content.YouTubeLink), BrowserLaunchMode.SystemPreferred);
        }

        private async void WatchingButton_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(content.SourceLink), BrowserLaunchMode.SystemPreferred);
        }

        private void StepperSeries_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SaveStepperAfterChange();
        }

        private void StepperSeason_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SaveStepperAfterChange();
        }

        // Обработчик добавления данных, если представление рекомендаций
        private async void ImageTapped(object sender, EventArgs e)
        {
            AddMoreContentPage addMoreContentPage = new AddMoreContentPage(recom); //?
            await Shell.Current.GoToAsync("//Add");
            Navigation.RemovePage(this);
        }
        #endregion

        #region [Hide Elements]
        private void HideElelmetsAfterCansel()
        {
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = false;
            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;
        }

        private void StartEditing()
        {
            IsEditing(true, "Сохранить");

            HideElements(false);

            TypePicker.IsVisible = true;
            TypePicker.SelectedItem = content.Type;

            WatchStatusPicker.IsVisible = true;
            WatchStatusPicker.SelectedItem = content.WatchStatus;
        }

        private void ReturnVisibleAfterChanges()
        {
            TrailerWebBorder.IsVisible = true;
            ViewContent.IsVisible = true;
            DecriptionBorder.IsVisible = true;
            TypeEntry.IsVisible = true;

            WatchStatusEntry.IsVisible = true;
            InfoBorder.IsVisible = true;
            DataLabel.IsVisible = true;
            CountLabel.IsVisible = true;
            TitleEntry.IsEnabled = false;
            TitleEntry.IsVisible = false;
            TitleLabel.IsVisible = true;
            DubbingEntry.IsEnabled = false;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
        }
        private void IsEditing(bool status, string str)
        {
            isEditing = status;
            EditButton.Text = str;
            CancelButton.IsVisible = status;
        }
        private void HideElements(bool isVisible)
        {
            // Разблокировать поля ввода
            LinkSecondLabel.IsVisible = isVisible;
            LinkEntry.IsVisible = isVisible;
            LinkEntry.IsReadOnly = isVisible;
            TrailerWebBorder.IsVisible = isVisible;

            DecriptionBorder.IsVisible = isVisible;
            InfoBorder.IsVisible = isVisible;
            ViewContent.IsVisible = isVisible;
            TypeEntry.IsVisible = isVisible;
            WatchStatusEntry.IsVisible = isVisible;

            TitleLabel.IsVisible = isVisible;
            TitleEntry.IsVisible = !isVisible;
            TitleEntry.IsEnabled = !isVisible;
            DubbingEntry.IsEnabled = !isVisible;
            DubbingPo.IsVisible = !isVisible;
            LastWatchedSeriesEntry.IsReadOnly = isVisible;
            LastWatchedSeasonEntry.IsReadOnly = isVisible;

            DataLabel.IsVisible = isVisible;
            CountLabel.IsVisible = isVisible;

            WatchStatusEntry.IsReadOnly = isVisible;

            TypePicker.IsVisible = isVisible;
            WatchStatusPicker.IsVisible = isVisible;
        }

        private void SelectRecomendetContent()
        {
            DubbingPo.IsVisible = false;
            StatusP.IsVisible = false;
            EditButton.IsVisible = false;
            DeleteButton.IsVisible = false;
            AddButton.IsVisible = true;
        }
        #endregion

    }

}

  
