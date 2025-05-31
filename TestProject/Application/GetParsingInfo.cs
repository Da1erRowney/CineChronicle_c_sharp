using CineChronicle.Application.SupportClass;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CineChronicle.Application
{

    public class GetParsingInfo
    {
        #region [Private Fields]
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string YouTubeLink { get; set; } = string.Empty;
        public string YouTubeBackground { get; set; } = string.Empty;
        public string NextEpisodeReleaseDate { get; set; } = string.Empty;
        public string CountLabel { get; set; } = string.Empty;
        public string DateRelease { get; set; } = string.Empty;
        public string RealTitle { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string ExtractUrlWatch { get; set; } = string.Empty;

        private const string Warning = "Ошибка получения информации. Такое случается когда вы неправильно указали название или тип своего медиа-контента Будьте внимательными :)";
        private const string FinalyWarning = "Похоже система не обнаружила текущий контент. Такое бывает, но крайне редко. Приносим свои извинения. Попробуйте обновить контент свайпом. Если проблема не пропала, то свяжитесь с нами - cine.chronicle.sup@gmail.com";


        private int countRead = 0;
        #endregion

        #region [Prepare Parser]
        public async Task<bool> GetData(string type, string title)
        {
            try
            {
                await PreparingParser(type, title);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task PreparingParser(string type, string title)
        {
            string typePars = string.Empty;
            switch (type)
            {
                case ContentTypes.ANIME:
                    typePars = SourceTypes.AG;
                    break;
                case ContentTypes.FILM:
                case ContentTypes.SERIAL:
                case ContentTypes.CARTOON:
                case ContentTypes.DORAMA:
                case ContentTypes.OTHER:
                    typePars = SourceTypes.WIK;
                    break;
            }

            await PushParser(type, title, typePars);
        }
        #endregion

        #region [Start/Finaly Parser]
        //Запуск парсера
        private async Task PushParser(string type, string title, string typePars)
        {
            try
            {
                // Создаем список задач динамически
                var tasks = new List<Task>();

                // Добавляем обязательные задачи
                tasks.Add(GetInfo(title, type, typePars, false));
                tasks.Add(GetInfo(title, type, typePars, true));
                tasks.Add(GetInfo(title, type, SourceTypes.YT, false));

                // Добавляем условную задачу
                if (type != ContentTypes.FILM)
                {
                    tasks.Add(GetInfo(title, type, SourceTypes.DE, false));
                }

                // Ожидаем завершения всех задач, игнорируя null
                await Task.WhenAll(tasks.Where(t => t != null));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга: {ex.Message}");
                throw; 
            }

            if (NextEpisodeReleaseDate.Contains("Нет, продолжения не будет."))
            {
                string newStr = $"Будет ли продолжение {title}? {NextEpisodeReleaseDate}";
                NextEpisodeReleaseDate = newStr;
            }
            else if(string.IsNullOrEmpty(NextEpisodeReleaseDate) || NextEpisodeReleaseDate.Contains("не найдена"))
            {
                NextEpisodeReleaseDate =  $"Информация об {title} не найдена. "+ FinalyWarning;
            }
            else
            {
                string newStr = $"Статус продолжения {title}.\n{NextEpisodeReleaseDate}";
                NextEpisodeReleaseDate = newStr;
            }

            if (Description == Warning && !string.IsNullOrEmpty(OriginalTitle))
            {
                await GetInfo(OriginalTitle, type, typePars, true);
            }
            else if(Description == Warning)
            {
                Description = FinalyWarning;
            }

            Description = Regex.Replace(Description, @"&nbsp;", " ");
            Description = Regex.Replace(Description, @"\[\d+\]", string.Empty);

            // Удаляем числовые ссылки в круглых скобках (если нужно)
            // text = Regex.Replace(text, @"\(\d+\)", string.Empty);

            // Удаляем множественные пробелы, которые могли образоваться
            Description = Regex.Replace(Description, @"\s+", " ");
        }
        #endregion

        #region [Main Parser]
        /// <summary>
        /// Получаем описание, постер, трейлеры и дату выхода контента
        /// </summary>
        /// <param name="query">Искомый запрос(название контента)</param>
        /// <param name="type">Тип контента</param>
        /// <param name="sourcePars">Источник из которого берем инфу</param>
        /// <param name="isInfo">Если информация, иначе картинка(постер)</param>
        private async Task GetInfo(string query, string type, string sourcePars, bool isInfo, bool debug = false)
        {
            string url = "";
            //Формируем ссылку в зависимости от источника
            switch (sourcePars, isInfo)
            {
                case (SourceTypes.WIK, true):
                    url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)} (телесериал)";
                    break;
                case (SourceTypes.WIK, false):
                    url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";
                    break;
                case (SourceTypes.KO, true):
                    url = $"https://kinogo.biz/search/{Uri.EscapeDataString(query)}";
                    break;
                case (SourceTypes.JS, true):
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    url = $"https://jut.su/search/?searchid=1893616&text={Uri.EscapeDataString(query)}";
                    break;
                case (SourceTypes.AG, true):
                    url = "https://animego.org/search/all?q=" + query;
                    break;
                case (SourceTypes.AG, false):
                    url = $"https://animego.org/search/all?q={Uri.EscapeDataString(query)}";
                    break;
                case (SourceTypes.PG, false):
                    url = $"https://premier.one/search?query={Uri.EscapeDataString(query)}";
                    break;
                case (SourceTypes.YT, _):
                    url = $"https://www.youtube.com/results?search_query={query}+{type}+трейлер";
                    break;
                case (SourceTypes.LF, false):
                    url = $"https://www.google.by/search?q= {Uri.EscapeDataString(query)} Постер&tbm=isch&ved=2ahUKEwiZtra589-EAxW8if0HHa5CCkYQ2-cCegQIABAA&oq=а&gs_lp=EgNpbWciAtCwSJwUUJsSWKoTcAB4AJABAJgBsAGgAbABqgEDMC4xuAEDyAEA-AEBigILZ3dzLXdpei1pbWeoAgDCAgoQABiABBiKBRhDiAYB&sclient=img&ei=-4ToZdnMOryT9u8ProWpsAQ";
                    break;
                case (SourceTypes.DE, _):
                    url = $"https://www.toramp.com/ru/search/?q={query}";
                    break;
                default:
                    url = "";
                    break;
            }

            //Парсим информацию
            if (!string.IsNullOrEmpty(url))
            {
                using (HttpClient client = new HttpClient())    //Общая часть для всех парсеров
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            string htmlContent = await response.Content.ReadAsStringAsync();
                            HtmlDocument htmlDocument = new HtmlDocument();
                            htmlDocument.LoadHtml(htmlContent);

                            HtmlNode node;
                            string extractedText = "";
                            string extractedLink = "";
                            //Обрабатываем индивидуальные парсинги источников
                            switch (sourcePars, isInfo)
                            {
                                case (SourceTypes.WIK, true):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//p");
                                    await WikInfoIsSuccess(node, htmlDocument, query);
                                    break;
                                case (SourceTypes.WIK, false):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                                    if (node != null)
                                    {
                                        if (string.IsNullOrEmpty(Image))
                                        {
                                            string ImageUrl = node.GetAttributeValue("src", "");

                                            // Проверяем, содержит ли URL префикс "https://"
                                            if (!ImageUrl.StartsWith("https://"))
                                            {
                                                // Добавляем префикс "https://", если его нет
                                                ImageUrl = "https:" + ImageUrl;
                                            }

                                            Image = ImageUrl;
                                        }
                                    }
                                    break;
                                case (SourceTypes.KO, true):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='excerpt']");
                                    if (node != null)
                                    {
                                        Description = node.InnerText.Trim();
                                    }
                                    else
                                    {
                                        Description = Warning;
                                    }
                                    break;
                                case (SourceTypes.JS, true):
                                    byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
                                    string htmlContents = Encoding.GetEncoding("windows-1251").GetString(responseBytes);

                                    // Поиск ссылки с помощью регулярного выражения
                                    Match match = Regex.Match(htmlContents, @"<yass-span class=""b-serp-url__item"">(.*?)</yass-span>");

                                    if (match.Success)
                                    {
                                        Description = match.Groups[1].Value;
                                    }
                                    else
                                    {
                                        Description = Warning;
                                    }
                                    break;
                                case (SourceTypes.AG, true):
                                    HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='h5 font-weight-normal mb-2 card-title text-truncate']/a");

                                    if (nodes != null && nodes.Count > 0)
                                    {
                                        foreach (HtmlNode aNode in nodes)
                                        {
                                            if (string.IsNullOrEmpty(Description))
                                            {
                                                extractedText = aNode.InnerText.Trim();
                                               extractedLink = aNode.GetAttributeValue("href", "");
                                               
                                               url = extractedLink;

                                           
                                                using (HttpClient clientы = new HttpClient())
                                                {
                                                    HttpResponseMessage responseы = await clientы.GetAsync(url);

                                                    if (responseы.IsSuccessStatusCode)
                                                    {
                                                        string htmlContentSearch = await responseы.Content.ReadAsStringAsync();

                                                        string aPattern = @"<div data-readmore=""content"">\s+(.*?)\s+</div></div></div><div class=""mt-3"">";
                                                        Match matchs = Regex.Match(htmlContentSearch, aPattern, RegexOptions.Singleline);

                                                        if (matchs.Success)
                                                        {
                                                            extractedText = matchs.Groups[1].Value.Trim();
                                                            extractedText = HtmlEntity.DeEntitize(extractedText);
                                                            Description = Regex.Replace(extractedText, "<.*?>", String.Empty);
                                                            ExtractUrlWatch = url;
                                                            return;
                                                        }
                                                        else if (!string.IsNullOrEmpty(htmlContentSearch))
                                                        {
                                                            string description = ExtractDescription(htmlContentSearch);
                                                            if (!string.IsNullOrEmpty(description))
                                                            {
                                                                Description = description;
                                                                ExtractUrlWatch = url;
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            string patterns = @"<div data-readmore=""content"">(.*?)</div></div></div><div class=""mt-3"">";
                                                            Match matchss = Regex.Match(htmlContentSearch, patterns, RegexOptions.Singleline);

                                                            if (matchss.Success)
                                                            {
                                                                string extractedTexts = matchss.Groups[1].Value;
                                                                extractedTexts = HtmlEntity.DeEntitize(extractedTexts);
                                                                Description = Regex.Replace(extractedTexts, "<.*?>", String.Empty);
                                                                ExtractUrlWatch = url;
                                                                return;
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case (SourceTypes.AG, false):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='anime-grid-lazy lazy']");
                                    if (node != null)
                                    {
                                        Image = node.GetAttributeValue("data-original", "");
                                    }
                                    break;
                                case (SourceTypes.PG, false):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'e-poster__Image-wrap')]/figure/img");

                                    if (node != null)
                                    {
                                        Image = node.GetAttributeValue("src", "");
                                    }
                                    break;
                                case (SourceTypes.YT, _):
                                    try
                                    {
                                        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                                        {
                                            ApiKey = "AIzaSyCI4_NOPQS-6Vzg2-wTZNhqMTW38KCvogU",
                                            ApplicationName = "CineChronicle"
                                        });

                                        // 1. Поиск видео
                                        var searchListRequest = youtubeService.Search.List("snippet");
                                        searchListRequest.Q = $"{query} {type} трейлер";
                                        searchListRequest.MaxResults = 5;
                                        searchListRequest.Type = "video";
                                        searchListRequest.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;

                                        var searchListResponse = await searchListRequest.ExecuteAsync();
                                        if (searchListResponse.Items.Count == 0)
                                        {
                                            await GetUniqueVideoIds(query, type);
                                            return;
                                        }

                                        // 2. Проверка статуса видео
                                        var videoIds = searchListResponse.Items.Select(i => i.Id.VideoId).ToList();
                                        var videosRequest = youtubeService.Videos.List("contentDetails,status");
                                        videosRequest.Id = string.Join(",", videoIds);

                                        var videosResponse = await videosRequest.ExecuteAsync();
                                        var validVideos = videosResponse.Items
                                            .Where(v => v.Status?.Embeddable == true &&
                                                       v.Status?.PrivacyStatus == "public" &&
                                                       (v.ContentDetails?.RegionRestriction == null ||
                                                        !v.ContentDetails.RegionRestriction.Blocked.Contains("RU")))
                                            .OrderByDescending(v => v.Status?.UploadStatus == "processed")
                                            .ToList();

                                        if (validVideos.Count == 0)
                                        {
                                            await GetUniqueVideoIds(query, type);
                                            return;
                                        }

                                        // 3. Выбор лучшего видео
                                        var bestVideo = validVideos.First();
                                        YouTubeLink = $"https://www.youtube.com/embed/{bestVideo.Id}";

                                        // 4. Для фона берем следующее подходящее видео (если есть)
                                        if (validVideos.Count > 1)
                                        {
                                            var backgroundVideo = validVideos[1];
                                            YouTubeBackground = $"https://www.youtube.com/embed/{backgroundVideo.Id}?" +
                                                "autoplay=1&mute=1&loop=1&controls=0&rel=0&" +
                                                $"playlist={backgroundVideo.Id}&enablejsapi=1&fs=0&vq=hd1080";
                                        }
                                        else
                                        {
                                            // Если только одно видео - используем его для фона тоже
                                            YouTubeBackground = $"https://www.youtube.com/embed/{bestVideo.Id}?" +
                                                "autoplay=1&mute=1&loop=1&controls=0&rel=0&" +
                                                $"playlist={bestVideo.Id}&enablejsapi=1&fs=0&vq=hd1080";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Ошибка YouTube API: {ex.Message}");
                                        await GetUniqueVideoIds(query, type);
                                    }
                                    break;
                                case (SourceTypes.LF, false):
                                    // Использовалось раньше для взятия картинки, возможны проблемы с источником, пересмотреть логику снизу
                                    break;
                                case (SourceTypes.DE, _):
                                    node = htmlDocument.DocumentNode.SelectSingleNode($"//div[@class='content']//a[contains(., '{query}')]");
                                    await DateExitIsSuccess(node, query, type, htmlDocument);
                                    break;
                            }
                        }
                        else
                        {
                            switch (sourcePars, isInfo)
                            {
                                case (SourceTypes.WIK, true):
                                    countRead = 0;
                                    await WikInfoIsWrong(query, type);
                                    break;
                                default:
                                    if (string.IsNullOrEmpty(Description))
                                    {
                                        Description = Warning;
                                    }
                                    break;
                            }
                            if (debug)
                            {
                                Debug.WriteLine($"Ошибка при взятии из {sourcePars}, информация: {isInfo}, название контента: {query}");
                                Debug.WriteLine($"Ссылка: {url}");
                            }
                        }
                    }
                    catch
                    {
                        if (debug)
                        {
                            Debug.WriteLine($"Ошибка при взятии из {sourcePars}, информация: {isInfo}, название контента: {query}");
                            Debug.WriteLine($"Ссылка: {url}");
                        }
                    }
                }
            }
        }
        #endregion

        #region [Support Wik Parser]
        private async Task WikInfoIsSuccess(HtmlNode node, HtmlDocument htmlDocument, string query)
        {
            string nameContent = $"{query}:\n";

            if (node != null && node.InnerText != nameContent && !node.InnerText.Trim().EndsWith(":"))
            {
                string firstParagraphText = node.InnerText;
                Description = HtmlEntity.DeEntitize(firstParagraphText); // Находим описание для контента
            }
            else
            {
                // Ищем все элементы списка (теги <li>) внутри элемента с id="mw-content-text"
                var listItems = htmlDocument.DocumentNode.SelectNodes("//div[@id='mw-content-text']//li");
                var linkNode = htmlDocument.DocumentNode.SelectSingleNode($"//a[@title='{Uri.EscapeDataString(query)} (телесериал)']");
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

                            if (responses.IsSuccessStatusCode)
                            {
                                string htmlContents = await responses.Content.ReadAsStringAsync();

                                HtmlDocument htmlDocuments = new HtmlDocument();
                                htmlDocuments.LoadHtml(htmlContents);
                                // Находим элемент img с атрибутом src, содержащим ссылку на изображение
                                HtmlNode ImageNodes = htmlDocuments.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                                if (ImageNodes != null)
                                {
                                    if (string.IsNullOrEmpty(Image))
                                    {
                                        string ImageUrls = ImageNodes.GetAttributeValue("src", "");

                                        // Проверяем, содержит ли URL префикс "https://"
                                        if (!ImageUrls.StartsWith("https://"))
                                        {
                                            // Добавляем префикс "https://", если его нет
                                            ImageUrls = "https:" + ImageUrls;
                                        }
                                        Image = ImageUrls;
                                    }
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

                            if (listItemText.Contains("телесериал") || listItemText.Contains("дорама") || listItemText.Contains("мультсериал") || listItemText.Contains("фильм"))
                            {
                                listItemText = HtmlEntity.DeEntitize(listItemText); //Находим описание
                                Description = listItemText;
                                return;
                            }
                        }
                    }
                    Description = Warning;

                }

                else
                {
                    string url1 = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";

                    using (HttpClient client1 = new HttpClient())
                    {
                        HttpResponseMessage response1 = await client1.GetAsync(url1);

                        if (response1.IsSuccessStatusCode)
                        {
                            string htmlContent1 = await response1.Content.ReadAsStringAsync();

                            HtmlDocument htmlDocument1 = new HtmlDocument();
                            htmlDocument1.LoadHtml(htmlContent1);
                            HtmlNode firstParagrap1h = htmlDocument1.DocumentNode.SelectSingleNode("//p");

                            string firstParagraphText = firstParagrap1h.InnerText;
                            Description = HtmlEntity.DeEntitize(firstParagraphText);
                        }
                    }
                }
            }
        }

        private async Task WikInfoIsWrong(string query, string type)
        {
            if (countRead == 3) return;
            string url1 = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";

            using (HttpClient client1 = new HttpClient())
            {
                HttpResponseMessage response1 = await client1.GetAsync(url1);

                if (response1.IsSuccessStatusCode)
                {
                    string htmlContent1 = await response1.Content.ReadAsStringAsync();

                    HtmlDocument htmlDocument1 = new HtmlDocument();
                    htmlDocument1.LoadHtml(htmlContent1);

                    HtmlNode firstParagrap1h = htmlDocument1.DocumentNode.SelectSingleNode("//p");

                    string nameContent1 = $"{query}:\n";


                    if (firstParagrap1h != null && firstParagrap1h.InnerText != nameContent1 && !firstParagrap1h.InnerText.Trim().EndsWith(":"))
                    {
                        string firstParagraphText = firstParagrap1h.InnerText;
                        Description = HtmlEntity.DeEntitize(firstParagraphText);
                    }
                    else
                    {
                        var listItems = htmlDocument1.DocumentNode.SelectNodes("//div[@id='mw-content-text']//li");

                        if (listItems != null)
                        {
                            foreach (var listItem in listItems)
                            {
                                string listItemText = listItem.InnerText;

                                string search = type;
                                if (search == "Сериал")
                                {
                                    search = "телесериал";
                                }

                                if (listItemText.Contains(search))
                                {
                                    Description = HtmlEntity.DeEntitize(listItemText);
                                    return;
                                } 
                            }
                        }
                        if (string.IsNullOrEmpty(Description))
                        {
                            Description = Warning;
                        }
                        countRead++;
                        if (countRead == 1)
                        {
                            await WikInfoIsWrong($"{query} сериал", type);
                        }
                        if (countRead == 2)
                        {
                            query = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(query.ToLower());
                            await WikInfoIsWrong(query, type);
                        }
                        if(countRead == 3)
                        {
                            return;
                        }
                    }
                }

                else
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        Description = Warning;
                    }
                }
            }
        }
        #endregion

        #region [Support YouTube Parser]

        // Резервный метод получения трейлера с YouTube когда квота превысила лимит
        public async Task GetUniqueVideoIds(string query, string type, int maxCount = 2)
        {
            string searchUrl = $"https://www.youtube.com/results?search_query={WebUtility.UrlEncode(query)}+{WebUtility.UrlEncode(type)}+трейлер&sp=CAASBhABGAEgAQ%253D%253D";
            var uniqueIds = new HashSet<string>();
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
                client.Timeout = TimeSpan.FromSeconds(10);

                string html = await client.GetStringAsync(searchUrl);

                // Основной паттерн для поиска videoId
                var videoMatches = Regex.Matches(html, @"""videoId""\s*:\s*""([a-zA-Z0-9_-]{11})""");

                foreach (Match match in videoMatches)
                {
                    if (!match.Success || uniqueIds.Count >= maxCount) continue;

                    string videoId = match.Groups[1].Value;

                    // Проверка что это не Shorts
                    if (!IsShortsVideo(html, videoId))
                    {
                        uniqueIds.Add(videoId);
                    }
                }
                var urlAddresses = uniqueIds.ToArray();

                YouTubeLink = $"https://www.youtube.com/embed/{urlAddresses[0]}";
                YouTubeBackground = $"https://www.youtube.com/embed/{urlAddresses[1]}?" +
                                                "autoplay=1&" +
                                                "mute=1&" +
                                                "loop=1&" +
                                                "controls=0&" +
                                                "rel=0&" +
                                                $"playlist={urlAddresses[1]}&" +
                                                "enablejsapi=1&" +
                                                "fs=0&" +
                                                "vq=hd1080"; // Принудительное HD 1080p
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        #endregion

        #region [Support DateExit Parser]
        private async Task DateExitIsSuccess(HtmlNode node, string query, string type, HtmlDocument htmlDocument)
        {
            using (HttpClient client = new HttpClient())
            {
                if (node != null)
                {
                    string link = node.GetAttributeValue("href", "");

                    HttpResponseMessage responseIn = await client.GetAsync(link);

                    if (responseIn.IsSuccessStatusCode)
                    {
                        await FormirateGeneralStringDE(type, responseIn);
                    }
                    else
                    {
                        Console.WriteLine("Не удалось выполнить запрос к сайту.");
                    }
                }
                else
                {
                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='content']//a");
                    if (node != null)
                    {
                        string link = node.GetAttributeValue("href", "");
                        HttpResponseMessage responseIn = await client.GetAsync(link);

                        if (responseIn.IsSuccessStatusCode)
                        {
                            await FormirateGeneralStringDE(type, responseIn);
                        }
                        else
                        {
                            Console.WriteLine("Не удалось выполнить запрос к сайту.");
                        }
                    }
                }
            }
        }

        private async Task FormirateGeneralStringDE(string type, HttpResponseMessage responseIn)
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

                        NextEpisodeReleaseDate = output;
                        CountLabel = countText;
                        DateRelease = releaseDate.ToShortDateString();
                    }
                }
                else
                {
                    exitEpisod = FormirateString(exitEpisod, countText);
                }
            }
            else
            {
                NextEpisodeReleaseDate = Warning;
            }

            GetMainDataDE(htmlDocumentIn, type);
        }

        private void GetMainDataDE(HtmlDocument htmlDocumentIn, string type)
        {
            if (type == "Сериал" || type == "Дорама" || type == "Мультсериал" || type == "Аниме")
            {
                HtmlNode scriptNode = htmlDocumentIn.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");
                if (scriptNode != null)
                {
                    try
                    {
                        // Десериализуем JSON
                        var jsonData = JsonConvert.DeserializeObject<dynamic>(scriptNode.InnerText);
                        if (jsonData?.description?.ToString()?.Trim() != null)
                        {
                            Description = jsonData?.description?.ToString()?.Trim();
                        }
                        RealTitle = jsonData?.name;
                        Image = jsonData?.image;
                        OriginalTitle = jsonData?.alternateName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка парсинга JSON: {ex.Message}");
                    }
                };
            }
        }
        #endregion

        #region [Support Methods]
        private bool IsShortsVideo(string html, string videoId)
        {
            // Паттерн 1: Проверка /shorts/ в URL
            if (Regex.IsMatch(html, $@"""webCommandMetadata"":\{{[^}}]*""url"":""/shorts/{videoId}"))
            {
                return true;
            }

            // Паттерн 2: Проверка специальных маркеров Shorts
            if (Regex.IsMatch(html, $@"""isShorts"":\s*true[^}}]*""videoId"":""{videoId}"""))
            {
                return true;
            }

            // Паттерн 3: Проверка в HTML-атрибутах
            if (Regex.IsMatch(html, $@"<a\s[^>]*href=""(/shorts/{videoId}|/watch\?v={videoId}[^""]*\bp=shorts)"""))
            {
                return true;
            }

            return false;
        }

        string ExtractDescription(string htmlContent)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Находим div с классом "description pb-3"
            var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'description') and contains(@class, 'pb-3')]");

            if (descriptionNode != null)
            {
                // Удаляем все HTML-теги и спецсимволы
                string description = descriptionNode.InnerText;

                // Очистка от HTML-сущностей и лишних пробелов
                description = System.Net.WebUtility.HtmlDecode(description);
                description = description.Replace("&nbsp;", " ")
                                        .Replace("&ndash;", "-")
                                        .Replace("&laquo;", "\"")
                                        .Replace("&raquo;", "\"")
                                        .Trim();

                // Удаляем лишние переносы строк и пробелы
                description = string.Join("\n\n",
                    description.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(line => line.Trim()));

                return description;
            }

            return string.Empty;
        }

        private string FormirateString(string exitEpisod, string countText)
        {
            // Заменяем каждую точку на точку с отступом и символ перевода строки
            exitEpisod = exitEpisod.Replace(".", ".");

            // Устанавливаем отформатированную строку в NextEpisodeReleaseDateEntry
            NextEpisodeReleaseDate = exitEpisod;
            CountLabel = countText;
            return exitEpisod;
        }

        #endregion
    }
}


#region [Not Use Parser]
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
//                    string ImageUrl = match.Groups[1].Value;
//                    // Отображаем изображение на форме
//                    PosterImage.Source = ImageSource.FromUri(new Uri(ImageUrl));
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
#endregion