using DataContent;
using HtmlAgilityPack;
using System.Net.Http;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Windows.Input;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ViewContentPage : ContentPage
    {
       

        public ICommand OpenLinkCommand { get; private set; }
        private Content content;

        // Ваш метод для открытия ссылки на видео YouTube
        private async void OpenYouTubeVideo(string videoId)
        {
            var youtubeUrl = $"https://www.youtube.com/watch?v={videoId}";

            try
            {
                await Launcher.OpenAsync(youtubeUrl);
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если URL-адрес не может быть открыт
                Console.WriteLine($"Unable to open YouTube video: {ex.Message}");
            }

        }
     

        public ViewContentPage(Content content)
        {
            this.content = content;
            InitializeComponent();
            BindingContext = content; // Привязываем объект Content к BindingContext страницы
            OpenLinkCommand = new Command<string>(OpenLink);
            //OpenYouTubeVideo("2tDOsJq0VCY");
            SetupLabelTappedEvents();



        }

        private async void GetWikipediaInfo(string query)
        {
            string url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";

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

                        HtmlNode firstParagraph = htmlDocument.DocumentNode.SelectSingleNode("//p");

                        string nameContent = $"{query}:\n";

                        if (firstParagraph != null && firstParagraph.InnerText != nameContent && !firstParagraph.InnerText.Trim().EndsWith(":"))
                        {

                            string firstParagraphText = firstParagraph.InnerText;
                            firstParagraphText = HtmlEntity.DeEntitize(firstParagraphText);
                            DescriptionLabel.Text = firstParagraphText;
                            GetWikipediaImage(query);
                        }

                        else
                        {

                            // Ищем все элементы списка (теги <li>) внутри элемента с id="mw-content-text"
                            var listItems = htmlDocument.DocumentNode.SelectNodes("//div[@id='mw-content-text']//li");
                            var linkNode = htmlDocument.DocumentNode.SelectSingleNode("//a[@title='Игра престолов (телесериал)']");
                            // Проверяем, что элемент найден
                            if (linkNode != null)
                            {
                                // Получаем значение атрибута href
                                string hrefValue = linkNode.GetAttributeValue("href", "");
                                // Сохраняем ссылку в отдельную переменную
                                var urls = hrefValue;

                                using (HttpClient clients = new HttpClient())
                                {
                                    try
                                    {
                                        HttpResponseMessage responses = await clients.GetAsync(urls);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            string htmlContents = await responses.Content.ReadAsStringAsync();

                                            HtmlDocument htmlDocuments = new HtmlDocument();
                                            htmlDocuments.LoadHtml(htmlContents);

                                            // Находим элемент img с атрибутом src, содержащим ссылку на изображение
                                            HtmlNode imageNodes = htmlDocuments.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                                            if (imageNodes != null)
                                            {
                                                string imageUrls = imageNodes.GetAttributeValue("src", "");

                                                // Проверяем, содержит ли URL префикс "https://"
                                                if (!imageUrls.StartsWith("https://"))
                                                {
                                                    // Добавляем префикс "https://", если его нет
                                                    imageUrls = "https:" + imageUrls;
                                                }



                                                // Отображаем изображение на форме
                                                PosterImage.Source = ImageSource.FromUri(new Uri(imageUrls));

                                            }
                                        }
                                        else
                                        {
                                            // Обработка ошибок при выполнении запроса
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // Обработка ошибок
                                    }

                                }

                                if (listItems != null)
                                {
                                    foreach (var listItem in listItems)
                                    {
                                        string listItemText = listItem.InnerText;

                                        if (listItemText.Contains("телесериал") || listItemText.Contains("дорама") || listItemText.Contains("мультсериал"))
                                        {
                                            listItemText = HtmlEntity.DeEntitize(listItemText);
                                            DescriptionLabel.Text = listItemText;
                                            return;
                                        }
                                    }
                                }

                                DescriptionLabel.Text = "Информация о сериале не найдена1";

                            }
                        }
                    }

                    else
                    {

                        DescriptionLabel.Text = "Информация о сериале не найдена2";


                    }
                }
                catch (Exception ex)
                {
                    DescriptionLabel.Text = $"Ошибка: {ex.Message}";
                }
            }
        }

        private async void GetKinogoInfo(string query)
        {
            string url = $"https://kinogo.biz/search/{Uri.EscapeDataString(query)}";

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

                        HtmlNode excerptNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='excerpt']");

                        if (excerptNode != null)
                        {
                            string excerptText = excerptNode.InnerText.Trim();
                            DescriptionLabel.Text = excerptText;
                        }
                        else
                        {
                            DescriptionLabel.Text = "Описание не найдено";
                        }
                    }
                    else
                    {
                        DescriptionLabel.Text = "Ошибка при получении страницы";
                    }
                }
                catch (Exception ex)
                {
                    DescriptionLabel.Text = $"Ошибка: {ex.Message}";
                }
            }
        }



        private async void GetJutsuInfo(string query)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string url = $"https://jut.su/search/?searchid=1893616&text={Uri.EscapeDataString(query)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
                        string htmlContent = Encoding.GetEncoding("windows-1251").GetString(responseBytes);

                        // Поиск ссылки с помощью регулярного выражения
                        Match match = Regex.Match(htmlContent, @"<yass-span class=""b-serp-url__item"">(.*?)</yass-span>");

                        if (match.Success)
                        {
                            string urlFromHtml = match.Groups[1].Value;
                            DescriptionLabel.Text = urlFromHtml;
                        }
                        else
                        {
                            // Если ссылка не найдена, обработать соответствующим образом
                            DescriptionLabel.Text = "Description not found.";
                        }
                    }
                    else
                    {
                        // Обработка ошибок HTTP-запроса
                        DescriptionLabel.Text = "Error fetching data from the website.";
                    }
                }
                catch (Exception ex)
                {
                    // Обработка исключений
                    DescriptionLabel.Text = $"An error occurred: {ex.Message}";
                }
            }
        }




        private async void GetAnimeGoImage(string query)
        {
            string url = $"https://animego.org/search/all?q={Uri.EscapeDataString(query)}";

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
                        // Извлечение ссылки на изображение
                        HtmlNode imageDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='anime-grid-lazy lazy']");
                        if (imageDiv != null)
                        {
                            string imageUrl = imageDiv.GetAttributeValue("data-original", "");
                            // Отображаем изображение на форме
                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                        }
                    }
                    else
                    {
                        // Обработка ошибок при выполнении запроса
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                }
            }
        }


        private async void GetWikipediaImage(string query)
        {
            string url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";

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

                        // Находим элемент img с атрибутом src, содержащим ссылку на изображение
                        HtmlNode imageNode = htmlDocument.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                        if (imageNode != null)
                        {
                            string imageUrl = imageNode.GetAttributeValue("src", "");

                            // Проверяем, содержит ли URL префикс "https://"
                            if (!imageUrl.StartsWith("https://"))
                            {
                                // Добавляем префикс "https://", если его нет
                                imageUrl = "https:" + imageUrl;
                            }



                            // Отображаем изображение на форме
                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));

                        }
                    }
                    else
                    {
                        // Обработка ошибок при выполнении запроса
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                }
            }
        }

        private async void GetAnemeGoInfo(string query)
        {
 
            string url = "https://animego.org/search/all?q=" + query;
            string extractedText = "";
            string extractedLink = "";

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

                        HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='h5 font-weight-normal mb-2 card-title text-truncate']/a");
                        if (nodes != null && nodes.Count > 0)
                        {
                            foreach (HtmlNode node in nodes)
                            {
                                extractedText = node.InnerText.Trim();
                                extractedLink = node.GetAttributeValue("href", "");
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    // Обработка ошибок
                }
            }

            url = extractedLink;

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

                        HtmlNode descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='description pb-3']");
                        if (descriptionNode != null)
                        {
                            extractedText = descriptionNode.InnerText.Trim();
                            extractedText = HtmlEntity.DeEntitize(extractedText);
                            DescriptionLabel.Text = extractedText;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine(ex.Message);
                }
            }
        }

        
        //private async void GetLordsFilmImage(string query)
        //{
        //    string url = $"https://www.google.by/search?q= {Uri.EscapeDataString(query)} Постер&tbm=isch&ved=2ahUKEwiZtra589-EAxW8if0HHa5CCkYQ2-cCegQIABAA&oq=а&gs_lp=EgNpbWciAtCwSJwUUJsSWKoTcAB4AJABAJgBsAGgAbABqgEDMC4xuAEDyAEA-AEBigILZ3dzLXdpei1pbWeoAgDCAgoQABiABBiKBRhDiAYB&sclient=img&ei=-4ToZdnMOryT9u8ProWpsAQ";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage response = await client.GetAsync(url);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string htmlContent = await response.Content.ReadAsStringAsync();

        //                // Извлечение ссылки на изображение
        //                Match match = Regex.Match(htmlContent, @"<img\s+src\s*=\s*""([^""]+)""");


        //                if (match.Success)
        //                {
        //                    string imageUrl = match.Groups[1].Value;
        //                    // Отображаем изображение на форме
        //                    PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
        //                }
        //                else
        //                {
        //                    // Обработка случая, когда ссылка на изображение не найдена
        //                }
        //            }
        //            else
        //            {
        //                HttpStatusCode statusCode = response.StatusCode;
        //                // Обработка ошибки на основе кода состояния
        //                Console.WriteLine($"HTTP Error: {statusCode}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Обработка ошибок
        //        }
        //    }
        //}




        //public void MediaPlayerApi(string link)
        //{
        //    string apiKey = "YOUR_API_KEY";

        //    // Создание службы YouTube Data API
        //    YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        //    {
        //        ApiKey = apiKey,
        //        ApplicationName = "YourAppName"
        //    });

           
        //}


        private async void SetupLabelTappedEvents()
        {
            // Нет необходимости в цикле, так как у вас только одна метка
            // Можно просто добавить обработчик для этой метки

            if (LinkLabel != null)
            {
                LinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = OpenLinkCommand,
                    CommandParameter = (BindingContext as Content)?.Link // Передаем ссылку в качестве параметра команды
                });

            }

            // Вызываем метод для получения информации с Википедии при загрузке страницы
            string title = (BindingContext as Content)?.Title;
            string type = (BindingContext as Content)?.Type;
            switch (type)
            {
                case "Аниме":
                    GetAnemeGoInfo(title);
                    GetAnimeGoImage(title);
                    break;
                case "Фильм":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;
                case "Сериал":
                    GetWikipediaInfo(title);
                   // GetWikipediaImage(title);
                    break;
                case "Дорама":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;
                case "Мультсериал":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;
                case "Прочее":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;

            }
           




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
                string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

                DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

                databaseService.DeleteContent(content);

                databaseService.CloseConnection();

                await Navigation.PushAsync(new MainPage());
            }
        }



        private bool isEditing = false; // Флаг, указывающий, в режиме редактирования или нет

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
            LinkSecondLabel.IsVisible = true;
            LinkEntry.IsVisible = true;
            LinkEntry.IsReadOnly = false;
            LinkLabel.IsVisible = false;
            DateAddedLabel.IsVisible = false;
            DateAddedEntry.IsVisible = false;
            SeriesChangeDateLabel.IsVisible = false;
            SeriesChangeDateEntry.IsVisible = false;

            TypePicker.IsVisible = true;
            TypePicker.SelectedItem = content.Type;
            WatchStatusPicker.IsVisible = true;
            WatchStatusPicker.SelectedItem = content.WatchStatus;


            TypeEntry.IsVisible = false;
            TypeLabel.IsVisible = false;
            WatchStatusEntry.IsVisible = false;
            WatchStatusLabel.IsVisible = false;

            TitleEntry.IsReadOnly = false;
            DubbingEntry.IsReadOnly = false;
            LastWatchedSeriesEntry.IsReadOnly = false;
            LastWatchedSeasonEntry.IsReadOnly = false;
            NextEpisodeReleaseDateEntry.IsReadOnly = false;
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
            LinkEntry.IsReadOnly = true;
            LinkLabel.IsVisible = true;
            DateAddedLabel.IsVisible = true;
            DateAddedEntry.IsVisible = true;
            SeriesChangeDateLabel.IsVisible = true;
            SeriesChangeDateEntry.IsVisible = true;

            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;


            TypeEntry.IsVisible = true;
            TypeLabel.IsVisible = true;
            WatchStatusEntry.IsVisible = true; 
            WatchStatusLabel.IsVisible = true;

            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            NextEpisodeReleaseDateEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
          


            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
            content = databaseService.GetContentById(content.Id);
            content.Title = TitleEntry.Text;
            content.Type = TypePicker.SelectedItem.ToString();
            content.Dubbing=  DubbingEntry.Text;
            if (LastWatchedSeriesEntry.Text != content.LastWatchedSeries.ToString() || LastWatchedSeasonEntry.Text != content.LastWatchedSeason.ToString())
            {
                DateTime currentDate = DateTime.UtcNow;
                DateTime newDate = currentDate.AddHours(+3);
                content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
            content.SeriesChangeDate = SeriesChangeDateEntry.Text;

            }
            content.Link = LinkEntry.Text;
            content.LastWatchedSeries =  int.Parse(LastWatchedSeriesEntry.Text);
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);

            content.NextEpisodeReleaseDate = NextEpisodeReleaseDateEntry.Text;
            content.WatchStatus = WatchStatusPicker.SelectedItem.ToString() ;
            content.DateAdded = DateAddedEntry.Text;
            databaseService.UpdateContent(content);
            databaseService.CloseConnection();


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
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // Создаем экземпляр сервиса базы данных
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

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
                DateAddedEntry.Text = content.DateAdded;
            }

            // Блокируем поля ввода
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = true;
            LinkLabel.IsVisible = true;
            DateAddedLabel.IsVisible = true;
            DateAddedEntry.IsVisible = true;
            SeriesChangeDateLabel.IsVisible = true;
            SeriesChangeDateEntry.IsVisible = true;
            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;

            TypeEntry.IsVisible = true;
            TypeLabel.IsVisible = true;

            WatchStatusEntry.IsVisible = true;
            WatchStatusLabel.IsVisible = true;

            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            NextEpisodeReleaseDateEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
           

        }
        private async Task OpenTrailer(string trailerUrl)
        {
            try
            {
                // Проверяем, является ли устройство мобильным
                if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                {
                    // Открываем ссылку на трейлер с использованием установленного приложения YouTube
                    await Launcher.OpenAsync(new Uri("vnd.youtube://" + trailerUrl.Split('=')[1]));
                }
                else
                {
                    // Открываем ссылку на трейлер во внешнем браузере
                    await Launcher.OpenAsync(new Uri(trailerUrl));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при открытии трейлера: {ex.Message}");
            }
        }



        private async void TrailerButton_Clicked(object sender, EventArgs e)
        {
            string url = (BindingContext as Content)?.LinkTrailer;
            OpenLink(url);




        }

    }

}

  
