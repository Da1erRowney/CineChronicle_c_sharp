using CineChronicle.Application;
using CineChronicle.Application.SupportClass;
using CineChronicle.Application.ViewModel;
using CineChronicle.Tables;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DeviceInfo = CineChronicle.Application.DeviceInfo;


namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ViewContentPage : ContentPage
    {
        #region [Private fields]
        public ICommand OpenLinkCommand { get; private set; }
        private ContentRecommendation recommendated;
        private Content content;
        private DateExit data;
        private GetParsingInfo parser = new();
        DeviceInfo _device = new();

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

        public ViewContentPage(ContentRecommendation recommendated)
        {
            InitializeComponent();
            this.recommendated = recommendated;

            CheckPageRecommendated();
        }

        #endregion

        #region [Refresh Data]
        private async void OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                _oldType = ""; // Инициализируйте значением по умолчанию
                if (!_device.CheckInternetConnection())
                {
                    await DisplayAlert("Ошибка", "Отсутствует интернет соединение", "OK");
                    return;
                }
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
        /// 5. Если есть соединение с интернетом отображаем данные, иначе ставим базовую картинку
        /// 5.1 Проверяем статус в настройках видео, если стоит видео, отображаем видео
        /// 5.1.2 Если постер отсутсвует, ставим на фон базовую картинку
        /// 5.2 Если нет сети, отображаем базовую картинку
        /// 5.3 Если постера в оффлайне нет, ставим базовую картинку
        /// 6. Обновляем картинку в источнике
        /// 7. Скрываем строку с озвучкой если оно пустое
        /// 8. Если данные из блока Информация об... null, то скрываем этот блок
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
            HideContentDescription(content.Description);

            // 5
            InternetChecking();

            // 6
            CheckContentType(content.Type);

            // 7
            HideDubbing(content.Dubbing);

            // 8
            HideDataContent();

            if(content.Type == ContentTypes.FILM)
            {
                SeriesBorders.IsVisible = false;
                SeasonBorders.IsVisible = false;
            }
            else
            {
                SeriesBorders.IsVisible = true;
                SeasonBorders.IsVisible = true;
            }

            if (string.IsNullOrEmpty(content.UserLink))
            {
                UserSourseBorder.IsVisible = false;
            }
            else
            {
                UserSourseBorder.IsVisible = true;
            }
            //WatchStatusPicker.SelectedItem = content.WatchStatus;
        }
        private void UseNewBackground()
        {
            Random _random = new Random();
            string randomImage = $"{BackgroundImages.GetImageOfTheme(Application.Current.UserAppTheme, Application.Current.PlatformAppTheme)[_random.Next(0, BackgroundImages.GetImageOfTheme(Application.Current.UserAppTheme, Application.Current.PlatformAppTheme).Length)]}.jpg";
            Background.Source = randomImage;
        }

        private void InternetChecking()
        {
            if (_device.CheckInternetConnection())
            {
                // 5.1
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
                    // 5.1.2
                    if (string.IsNullOrEmpty(content.Image))
                    {
                        UseNewBackground();
                    }
                    else
                    {
                        Background.Source = content.Image;
                    }
                }
                TrailerWebBorder.IsVisible = true;
                ViewContent.IsVisible = true;

                if (!string.IsNullOrEmpty(content.Image))
                {
                    PosterImage.Source = content.Image;
                }
            }
            else
            {
                // 5.2
                TrailerWebBackground.IsVisible = false;
                Background.IsVisible = true;
                UseNewBackground();

                // 5.3
                if (string.IsNullOrEmpty(content.Image))
                {
                    PosterImage.Source = "notwificonnection.jpg";
                }

                TrailerWebBorder.IsVisible = false;
                ViewContent.IsVisible = false;
            }
        }

        private void HideDataContent()
        {
            if ((string.IsNullOrEmpty(content.CountLabel) && string.IsNullOrEmpty(content.NextEpisodeReleaseDate)) || content.Type == ContentTypes.FILM)
            {
                InfoBorder.IsVisible = false;
            }
            else
            {
                InfoBorder.IsVisible = true;
            }
        }

        private void HideDubbing(string dubbing)
        {
            if (string.IsNullOrEmpty(dubbing))
            {
                DubbingPo.IsVisible = false;
            }
            else
            {
                DubbingPo.IsVisible = true;
            }
        }

        private void CheckContentType(string type)
        {
            switch (type)
            {
                case ContentTypes.ANIME:
                    WatchingButton.Source = "anime.png";
                    WatchingUserButton.Source = "anime.png";
                    break;
                case ContentTypes.FILM:
                case ContentTypes.SERIAL:
                case ContentTypes.CARTOON:
                case ContentTypes.DORAMA:
                case ContentTypes.OTHER:
                    WatchingButton.Source = "movie.png";
                    WatchingUserButton.Source = "movie.png";
                    break;
                default:
                    WatchingButton.Source = "movie.png";
                    WatchingUserButton.Source = "movie.png";
                    break;
            }
        }

        private void HideContentDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                DecriptionBorder.IsVisible = false;
            }
            else
            {
                DecriptionBorder.IsVisible = true;
            }
        }

        private async Task CheckPageRecommendated()
        {
            SelectRecomendetContent();
            if (_device.CheckInternetConnection())
            {
                await GetDataRecom();
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
                // 5.1.2
                if (string.IsNullOrEmpty(recommendated.ImageUrl))
                {
                    UseNewBackground();
                }
                else
                {
                    Background.Source = recommendated.ImageUrl;
                }
            }

           // InternetChecking();
            CheckContentType(recommendated.Type);
            HideContentDescription(parser?.Description);
            BindingContext = new ViewContentPageRefreshModel(recommendated, parser);
        }
        private async Task GetDataRecom()
        {
            await ShowLoadingAnimation();
            bool parseSuccess = await parser.GetData(recommendated.Type, recommendated.Title);
            await HideLoadingAnimation();
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
            if (!CheckUserLink())
            {
                await DisplayAlert("Ошибка", $"Похоже вы указали не корректную ссылку.", "Ок");
                return;
            }
            IsEditing(false, "Изменить");

            HideElements(false);
            ReturnVisibleAfterChanges();

            await GetNewDataAndSave();
        }

        private bool CheckUserLink()
        {
            string pattern = @"^https://";
            if (string.IsNullOrEmpty(content.UserLink)) return true;
            bool isValid = Regex.IsMatch(content.UserLink, pattern);

            if (isValid)  return true;
            else return false; 
        }

        private async Task GetNewDataAndSave()
        {
            var currentContent = content;
            currentContent.WatchStatus = WatchStatusPicker.SelectedItem?.ToString() ?? content.WatchStatus;
            currentContent.Type = TypePicker.SelectedItem != null ? TypePicker.SelectedItem.ToString() : content.Type;
            currentContent.Title = currentContent.Title.TrimEnd();

            currentContent.UserLink = currentContent?.UserLink?.TrimEnd();


            // Проверяем, поменяли ли мы тип или название для получения новых данных из парсерса
            if (_oldName != currentContent.Title || _oldType!= currentContent.Type)
            {
                await ShowLoadingAnimation();
                if (_device.CheckInternetConnection())
                {
                    bool parseSuccess = await parser.GetData(currentContent.Type, currentContent.Title);

                    // Если название неправильно указано
                    if (string.IsNullOrEmpty(parser.Description) || (parser.RealTitle != currentContent.Title && !string.IsNullOrEmpty(parser.RealTitle)))
                    {
                        bool result = await DisplayAlert("Проверка названия",
                                $"Вы уверены, что ваш контент называется '{currentContent.Title}', а не '{parser.RealTitle}'?\n\n" +
                                "Если правильное название второе, нажмите \"Да\"",
                                "Да",
                                "Нет");
                        if (result)
                        {
                            currentContent.Title = parser.RealTitle;
                            bool parseSuccessawait = await parser.GetData(currentContent.Type, currentContent.Title);
                        }
                    }
                }

                currentContent.CountLabel = parser?.CountLabel;
                currentContent.Description = parser?.Description;
                currentContent.DateRelease = parser?.DateRelease;
                currentContent.Image = parser?.Image;
                currentContent.NextEpisodeReleaseDate = parser?.NextEpisodeReleaseDate;
                currentContent.YouTubeLink = parser?.YouTubeLink;
                currentContent.YouTubeBackground = parser?.YouTubeBackground;
                currentContent.OriginalTitle = parser?.OriginalTitle;

                // Проверяем, поменяли ли мы тип, для получения новой ссылки на источник
                if (_oldType != currentContent.Type)
                {
                    if (string.IsNullOrEmpty(parser.ExtractUrlWatch))
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
                    else
                    {
                        currentContent.SourceLink = parser?.ExtractUrlWatch;
                    }
                }

                await HideLoadingAnimation();
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

        private void StatusPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                string selectedType = (string)picker.ItemsSource[selectedIndex];
                content.WatchStatus = selectedType;

                DatabaseServiceContent _databaseService = new DatabaseServiceContent(MainPage._databasePath);
                _databaseService.UpdateContent(content);
                _databaseService.CloseConnection();

            }
        }

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

            CheckPage();

        }

        private async void TrailerButton_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(content.YouTubeLink), BrowserLaunchMode.SystemPreferred);
        }

        private async void WatchingButton_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(content?.SourceLink), BrowserLaunchMode.SystemPreferred);
        }
        private async void WatchingUserButton_Clicked(object sender, EventArgs e)
        {
            await Browser.OpenAsync(new Uri(content.UserLink), BrowserLaunchMode.SystemPreferred);
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
            AddMoreContentPage addMoreContentPage = new AddMoreContentPage(recommendated); //?

            await Navigation.PushAsync(addMoreContentPage);
        }
        #endregion

        #region [Hide Elements]
        private void HideElelmetsAfterCansel()
        {
            UserLinkBorder.IsVisible = false;
            TypePicker.IsVisible = false;
            //WatchStatusPicker.IsVisible = false;
        }

        private void StartEditing()
        {
            IsEditing(true, "Сохранить");

            HideElements(false);

            TypePicker.IsVisible = true;
            TypePicker.SelectedItem = content.Type;

            //WatchStatusPicker.IsVisible = true;
            //WatchStatusPicker.SelectedItem = content.WatchStatus;
        }

        private void ReturnVisibleAfterChanges()
        {
            TrailerWebBorder.IsVisible = true;
            ViewContent.IsVisible = true;
            DecriptionBorder.IsVisible = true;
            TypeEntry.IsVisible = true;

            InfoBorder.IsVisible = true;
            DataLabel.IsVisible = true;
            CountLabel.IsVisible = true;
            TitleEntry.IsEnabled = false;
            TitleEntry.IsVisible = false;
            TitleLabel.IsVisible = true;
            OriginalTitleLabel.IsVisible = true;
            UserLinkBorder.IsVisible = false;
            DubbingEntry.IsEnabled = false;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
        }
        private void IsEditing(bool status, string str)
        {
            isEditing = status;
            EditButton.Text = str;
            EditsButton.Text = str;
            CancelButton.IsVisible = status;
        }
        private void HideElements(bool isVisible)
        {
            // Разблокировать поля ввода
            UserLinkBorder.IsVisible = !isVisible;
            TrailerWebBorder.IsVisible = isVisible;

            DecriptionBorder.IsVisible = isVisible;
            InfoBorder.IsVisible = isVisible;
            ViewContent.IsVisible = isVisible;
            TypeEntry.IsVisible = isVisible;

            TitleLabel.IsVisible = isVisible;
            OriginalTitleLabel.IsVisible = isVisible;
            TitleEntry.IsVisible = !isVisible;
            TitleEntry.IsEnabled = !isVisible;
            DubbingEntry.IsEnabled = !isVisible;
            DubbingPo.IsVisible = !isVisible;
            LastWatchedSeriesEntry.IsReadOnly = isVisible;
            LastWatchedSeasonEntry.IsReadOnly = isVisible;

            DataLabel.IsVisible = isVisible;
            CountLabel.IsVisible = isVisible;

            TypePicker.IsVisible = isVisible;
            //WatchStatusPicker.IsVisible = isVisible;
        }

        private void SelectRecomendetContent()
        {
            AddButton.IsVisible = true;
            EditButton.IsVisible = false;
            ViewContent.IsVisible = false;
            DeleteButton.IsVisible = false;
            UserSourseBorder.IsVisible = false;
            OurInformationBlock.IsVisible = false;
            ServiceSourseBorder.IsVisible = false;
        }
        #endregion

    }

}

  
