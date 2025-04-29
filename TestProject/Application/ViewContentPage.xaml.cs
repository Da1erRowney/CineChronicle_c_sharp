using CineChronicle.Application;
using CineChronicle.Tables;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
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
        #endregion

        #region [Private fields]
        public ICommand OpenLinkCommand { get; private set; }

        private ContentRecommendation recom;
        private Content content;
        private DateExit data;
        private GetParsingInfo parser = new();

        private string videoUrl = null;

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
            string url = $"https://www.youtube.com/results?search_query={query}+{type}+трейлер";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string htmlContent = await response.Content.ReadAsStringAsync();

                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(htmlContent);

                        string pattern = "\\\\/vi\\\\/([^\\/\\\\\"]+)";
                        Match match = Regex.Match(htmlDocument.DocumentNode.OuterHtml, pattern);

                        if (match.Success)
                        {
                            videoUrl = $"https://www.youtube.com/watch?v={match.Groups[1].Value}";

                            await Browser.OpenAsync(new Uri(videoUrl), BrowserLaunchMode.SystemPreferred);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async void DataExitNextEpisod(string query)
        {
            string type = "";
            if (content != null)
            {
                type = content.Type;
            }
            else
            {
                type = recom.Type;
            }

            string url = $"https://www.toramp.com/ru/search/?q={query}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    // Извлечение ссылки из HTML
                    HtmlNode linkNode = htmlDocument.DocumentNode.SelectSingleNode($"//div[@class='content']//a[contains(., '{query}')]");
                    if (linkNode != null)
                    {
                        string link = linkNode.GetAttributeValue("href", "");
                        HttpResponseMessage responseIn = await client.GetAsync(link);

                        if (responseIn.IsSuccessStatusCode)
                        {
                            string htmlContentIn = await responseIn.Content.ReadAsStringAsync();

                            HtmlDocument htmlDocumentIn = new HtmlDocument();
                            htmlDocumentIn.LoadHtml(htmlContentIn);

                            // Извлечение ссылки из HTML
                            HtmlNode linkNodeIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_3']/em");
                            HtmlNode linkNodeCount = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_0']");
                            if (linkNodeIn != null || linkNodeCount != null)
                            {
                                string exitEpisod = linkNodeIn.InnerText;
                                string countText = linkNodeCount.InnerText.Trim();

                                int startIndex = exitEpisod.IndexOf("осталось") + "осталось".Length; // Индекс после слова "осталось"
                                int daysIndex = exitEpisod.IndexOf("дней", startIndex); // Индекс слова "дней" после startIndex

                                if (daysIndex == -1)
                                {
                                    daysIndex = exitEpisod.IndexOf("дня", startIndex); // Индекс слова "дня" после startIndex
                                }

                                if (daysIndex == -1)
                                {
                                    daysIndex = exitEpisod.IndexOf("день", startIndex); // Индекс слова "день" после startIndex
                                }

                                if (daysIndex != -1)
                                {
                                    // Извлекаем подстроку между startIndex и daysIndex
                                    string daysString = exitEpisod.Substring(startIndex, daysIndex - startIndex).Trim();

                                    if (int.TryParse(daysString, out int days))
                                    {
                                        DateTime releaseDate = DateTime.Today.AddDays(days);

                                        // Формируем строку для вывода
                                        string output = $"Осталось {days} дней до выхода ({releaseDate.ToShortDateString()})";

                                        NextEpisodeReleaseDateEntry.Text = output;
                                        CountLabel.Text = countText;

                                        
                                        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
                                        if (databaseService.GetDateByTitle(query) == null)
                                        {
                                            var newContent = new DateExit
                                            {
                                                Title = query,
                                                DateRelease = releaseDate.ToShortDateString()
                                            };
                                            databaseService.InsertDate(newContent);
                                        }
                                        else
                                        {
                                            data = databaseService.GetDateByTitle(query);
                                            data.DateRelease = releaseDate.ToShortDateString();
                                            databaseService.UpdateContent(data);
                                        }

                                    }
                                }

                                else
                                {
                                    exitEpisod = exitEpisod.Replace(".", ".");

                                    NextEpisodeReleaseDateEntry.Text = exitEpisod;
                                    CountLabel.Text = countText;


                                }
                            }
                            else
                            {
                                NextEpisodeReleaseDateEntry.Text = $"Информация о {query} не найдена";
                            }

                            if (type == "Сериал" || type == "Дорама" || type == "Мультсериал")
                            {
                                HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                                if (imgIn != null)
                                {
                                    string imageUrl = imgIn.GetAttributeValue("src", "");

                                    // Проверяем, содержит ли URL префикс "https://"
                                    if (!imageUrl.StartsWith("https://"))
                                    {
                                        // Добавляем префикс "https://", если его нет
                                        imageUrl = "https:" + imageUrl;
                                    }
                                    // Устанавливаем изображение в элементы UI
                                    PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                                    Background.Source = ImageSource.FromUri(new Uri(imageUrl));

                                    // Обновляем ссылку на изображение в базе данных
                                    if (content != null)
                                    {
                                        
                                        DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
                                        content = databaseService.GetContentById(content.Id);
                                        content.Image = imageUrl;
                                        databaseService.UpdateContent(content);
                                        databaseService.CloseConnection();
                                    }
                                }
                                else
                                {
                                    //Console.WriteLine("Изображение не найдено.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Не удалось выполнить запрос к сайту.");
                        }

                    }
                    else
                    {
                        linkNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='content']//a");
                        if (linkNode != null)
                        {
                            string link = linkNode.GetAttributeValue("href", "");
                            HttpResponseMessage responseIn = await client.GetAsync(link);

                            if (responseIn.IsSuccessStatusCode)
                            {
                                string htmlContentIn = await responseIn.Content.ReadAsStringAsync();

                                HtmlDocument htmlDocumentIn = new HtmlDocument();
                                htmlDocumentIn.LoadHtml(htmlContentIn);

                                // Извлечение ссылки из HTML
                                HtmlNode linkNodeIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_3']/em");
                                HtmlNode linkNodeCount = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_0']");
                                if (linkNodeIn != null || linkNodeCount != null)
                                {
                                    string exitEpisod = linkNodeIn.InnerText;
                                    string countText = linkNodeCount.InnerText.Trim();

                                    int startIndex = exitEpisod.IndexOf("осталось") + "осталось".Length; // Индекс после слова "осталось"
                                    int daysIndex = exitEpisod.IndexOf("дней", startIndex); // Индекс слова "дней" после startIndex

                                    if (daysIndex == -1)
                                    {
                                        daysIndex = exitEpisod.IndexOf("дня", startIndex); // Индекс слова "дня" после startIndex
                                    }

                                    if (daysIndex == -1)
                                    {
                                        daysIndex = exitEpisod.IndexOf("день", startIndex); // Индекс слова "день" после startIndex
                                    }

                                    if (daysIndex != -1)
                                    {
                                        // Извлекаем подстроку между startIndex и daysIndex
                                        string daysString = exitEpisod.Substring(startIndex, daysIndex - startIndex).Trim();

                                        if (int.TryParse(daysString, out int days))
                                        {
                                            // Вычисляем дату через указанное количество дней
                                            DateTime releaseDate = DateTime.Today.AddDays(days);

                                            // Формируем строку для вывода
                                            string output = $"Осталось {days} дней до выхода ({releaseDate.ToShortDateString()})";

                                            // Устанавливаем строку в NextEpisodeReleaseDateEntry
                                            NextEpisodeReleaseDateEntry.Text = output;
                                            CountLabel.Text = countText;
                                            
                                            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
                                            if (databaseService.GetDateByTitle(query) == null)
                                            {
                                                var newContent = new DateExit
                                                {
                                                    Title = query,
                                                    DateRelease = releaseDate.ToShortDateString()
                                                };
                                                databaseService.InsertDate(newContent);
                                            }
                                            else
                                            {
                                                data = databaseService.GetDateByTitle(query);
                                                data.DateRelease = releaseDate.ToShortDateString();
                                                databaseService.UpdateContent(data);
                                            }
                                        }
                                    }

                                    else
                                    {

                                        // Заменяем каждую точку на точку с отступом и символ перевода строки
                                        exitEpisod = exitEpisod.Replace(".", ".");

                                        // Устанавливаем отформатированную строку в NextEpisodeReleaseDateEntry
                                        NextEpisodeReleaseDateEntry.Text = exitEpisod;
                                        CountLabel.Text = countText;


                                    }
                                }
                                else
                                {
                                    NextEpisodeReleaseDateEntry.Text = $"Информация о {query} не найдена";
                                }

                                if (type == "Сериал" || type == "Дорама" || type == "Мультсериал")
                                {
                                    HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                                    if (imgIn != null)
                                    {
                                        string imageUrl = imgIn.GetAttributeValue("src", "");

                                        // Проверяем, содержит ли URL префикс "https://"
                                        if (!imageUrl.StartsWith("https://"))
                                        {
                                            // Добавляем префикс "https://", если его нет
                                            imageUrl = "https:" + imageUrl;
                                        }
                                        // Устанавливаем изображение в элементы UI
                                        PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                                        Background.Source = ImageSource.FromUri(new Uri(imageUrl));
                                        if (content != null)
                                        {
                                            // Обновляем ссылку на изображение в базе данных
                                            
                                            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
                                            content = databaseService.GetContentById(content.Id);
                                            content.Image = imageUrl;
                                            databaseService.UpdateContent(content);
                                            databaseService.CloseConnection();
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else
                                    {
                                        //Console.WriteLine("Изображение не найдено.");
                                    }
                                }









                            }
                            else
                            {
                                Console.WriteLine("Не удалось выполнить запрос к сайту.");
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Не удалось выполнить запрос к сайту.");
                }
            }
        }

        private async void SetupLabelTappedEvents()
        {
            // Вызываем метод для получения информации с Википедии при загрузке страницы
            string title = (BindingContext as Content)?.Title;
            string type = (BindingContext as Content)?.Type;

            GetInfo(type, title);
        }

        private async void SetupLabelTappedEventsRecom()
        {
            // Вызываем метод для получения информации с Википедии при загрузке страницы
            string title = (BindingContext as ContentRecommendation)?.Title;
            string type = (BindingContext as ContentRecommendation)?.Type;

            GetInfo(type, title);
        }

        private void GetInfo(string type, string title)
        {
            switch (type)
            {
                case "Аниме":
                    parser?.GetInfo(title, type, AG, true);
                    parser?.GetInfo(title, type, AG, false);
                    WatchingButton.Source = "anime.png";
                    break;
                case "Фильм":
                case "Сериал":
                case "Дорама":
                case "Мультсериал":
                case "Прочее":
                    parser?.GetInfo(title, type, WIK, false);
                    parser?.GetInfo(title, type, WIK, true);
                    WatchingButton.Source = "movie.png";
                    //WatchingButton.Source = "dorama.png";
                    break;
                default:
                    WatchingButton.Source = "movie.png";
                    break;
            }
            parser?.GetInfo(title, type, YT, false);
            parser?.GetInfo(title, type, DE, false);

            DescriptionLabel.Text = parser?.description;
            PosterImage.Source = ImageSource.FromUri(new Uri(parser?.image));
            Background.Source = ImageSource.FromUri(new Uri(parser?.image));
            TrailerWeb.Source = parser?.youTube;
            NextEpisodeReleaseDateEntry.Text = parser?.nextEpisodeReleaseDate;
            CountLabel.Text = parser?.countLabel;
        }

        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                // Открываем ссылку в браузере*-*+9
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Уведомление", $"Вы уверены, что хотите удалить {content.Title}?", "Да", "Нет");

            if (result)
            {
                DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);

                databaseService.DeleteContent(content);

                databaseService.CloseConnection();

                await Navigation.PopAsync();
            }
        }

        

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            if (isEditing)
            {
                // Если уже в режиме редактирования, то нужно сохранить изменения
                SaveChanges();
            }
            else
            {
                // Если не в режиме редактирования, то переключиться в этот режим
                StartEditing();
            }
        }

        private void StartEditing()
        {
            isEditing = true;
            EditButton.Text = "Сохранить";
            CancelButton.IsVisible = true; // Отобразить кнопку "Отмена"

            // Разблокировать поля ввода
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = false;
            TrailerWebBorder.IsVisible = false;


            DecriptionBorder.IsVisible = false;


            TypePicker.IsVisible = true;
            TypePicker.SelectedItem = content.Type;
            WatchStatusPicker.IsVisible = true;
            WatchStatusPicker.SelectedItem = content.WatchStatus;
            ViewContent.IsVisible = false;

            TypeEntry.IsVisible = false;
            TypeLabel.IsVisible = false;
            WatchStatusEntry.IsVisible = false;
            WatchStatusLabel.IsVisible = false;

            TitleEntry.IsReadOnly = false;
            DubbingEntry.IsReadOnly = false;
            LastWatchedSeriesEntry.IsReadOnly = false;
            LastWatchedSeasonEntry.IsReadOnly = false;


            DataLabel.IsVisible = false;
            NextEpisodeReleaseDateEntry.IsVisible = false;
            CountLabel.IsVisible = false;

            WatchStatusEntry.IsReadOnly = false;
        }

        private void SaveChanges()
        {
            isEditing = false;
            EditButton.Text = "Изменить";
            CancelButton.IsVisible = false; // Скрыть кнопку "Отмена"

            // Блокировать поля ввода
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = false;
            TrailerWebBorder.IsVisible = true;

            ViewContent.IsVisible = true;
            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;
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
                case "Аниме":
                    content.Link = "https://animego.org/search/all?q=" + TitleEntry.Text;
                    break;
                case "Дорама":
                    content.Link = "https://dorama.land/search?q=" + TitleEntry.Text;
                    break;
                case "Сериал":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case "Мультсериал":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case "Фильм":
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

            // Блокируем поля ввода
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = false;
            DecriptionBorder.IsVisible = true;


            ViewContent.IsVisible = true;


            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;
            TrailerWebBorder.IsVisible = true;
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
            

            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            content = databaseService.GetContentById(content.Id);
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);

            content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            content.LastWatchedSeries = int.Parse(LastWatchedSeriesEntry.Text);

            databaseService.UpdateContent(content);
            databaseService.CloseConnection();
        }

        private void StepperSeason_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newValue = e.NewValue;
            LastWatchedSeasonEntry.Text = newValue.ToString();
            

            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);
            content = databaseService.GetContentById(content.Id);
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);
            content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);
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

  
