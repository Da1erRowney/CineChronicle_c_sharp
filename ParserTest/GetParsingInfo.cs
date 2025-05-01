using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserTest
{

    public class GetParsingInfo
    {
        public event Action<DescriptionResult> DescriptionRead;
        public event Action<ImageResult> ImageRead;
        public event Action<YouTubeResult> YouTubeRead;
        public event Action<NextEpisodeReleaseDateResult> NextEpisodeReleaseDateRead;
        public event Action<CountLabelResult> CountLabelRead;
        public event Action<DateReleaseResult> DateReleaseRead;

        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string YouTube { get; set; } = string.Empty;
        public string NextEpisodeReleaseDate { get; set; } = string.Empty;
        public string CountLabel { get; set; } = string.Empty;
        public string DateRelease { get; set; } = string.Empty;

        public void Reset()
        {
            DescriptionResult a = new();
            Image = string.Empty;
            YouTube = string.Empty;
            // ... другие поля ...
        }

        public class DescriptionResult
        {
            public string Description { get; set; }
        }

        public class ImageResult
        {
            public string Image { get; set; }
        }

        public class YouTubeResult
        {
            public string YouTube { get; set; }
        }

        public class NextEpisodeReleaseDateResult
        {
            public string NextEpisodeReleaseDate { get; set; }
        }

        public class CountLabelResult
        {
            public string CountLabel { get; set; }
        }

        public class DateReleaseResult
        {
            public string DateRelease { get; set; }
        }

        /// <summary>
        /// Получаем описание, постер, трейлеры и дату выхода контента
        /// </summary>
        /// <param name="query">Искомый запрос(название контента)</param>
        /// <param name="type">Тип контента</param>
        /// <param name="sourcePars">Источник из которого берем инфу</param>
        /// <param name="isInfo">Если информация, иначе картинка(постер)</param>
        public async Task GetInfo(string query, string type, string sourcePars, bool isInfo, bool debug = false)
        {
            string url = "";
            //Формируем ссылку в зависимости от источника
            switch (sourcePars, isInfo)
            {
                case ("Википедия", true):
                    url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)} (телесериал)";
                    break;
                case ("Википедия", false):
                    url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";
                    break;
                case ("Kinogo", true):
                    url = $"https://kinogo.biz/search/{Uri.EscapeDataString(query)}";
                    break;
                case ("Jutsu", true):
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    url = $"https://jut.su/search/?searchid=1893616&text={Uri.EscapeDataString(query)}";
                    break;
                case ("AnimeGo", true):
                    url = "https://animego.org/search/all?q=" + query;
                    break;
                case ("АнимеГо", false):
                    url = $"https://animego.org/search/all?q={Uri.EscapeDataString(query)}";
                    break;
                case ("PremierGo", false):
                    url = $"https://premier.one/search?query={Uri.EscapeDataString(query)}";
                    break;
                case ("YouTube", _):
                    url = $"https://www.youtube.com/results?search_query={query}+{type}+трейлер";
                    break;
                case ("LordsFilm", false):
                    url = $"https://www.google.by/search?q= {Uri.EscapeDataString(query)} Постер&tbm=isch&ved=2ahUKEwiZtra589-EAxW8if0HHa5CCkYQ2-cCegQIABAA&oq=а&gs_lp=EgNpbWciAtCwSJwUUJsSWKoTcAB4AJABAJgBsAGgAbABqgEDMC4xuAEDyAEA-AEBigILZ3dzLXdpei1pbWeoAgDCAgoQABiABBiKBRhDiAYB&sclient=img&ei=-4ToZdnMOryT9u8ProWpsAQ";
                    break;
                case ("DateExit", _):
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

                        var descriptionResult = new DescriptionResult { };
                        var imageResult = new ImageResult { };
                        var youtubeResult = new YouTubeResult { };
                        var nextEpisodeResult = new NextEpisodeReleaseDateResult { };
                        var countResult = new CountLabelResult { };
                        var dateReleaseResult = new DateReleaseResult { };


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
                                case ("Википедия", true):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//p");
                                    WikInfoIsSuccess(node, htmlDocument, query);
                                    descriptionResult.Description = Description;
                                    DescriptionRead?.Invoke(descriptionResult);
                                    break;
                                case ("Википедия", false):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                                    if (node != null)
                                    {
                                        string ImageUrl = node.GetAttributeValue("src", "");

                                        // Проверяем, содержит ли URL префикс "https://"
                                        if (!ImageUrl.StartsWith("https://"))
                                        {
                                            // Добавляем префикс "https://", если его нет
                                            ImageUrl = "https:" + ImageUrl;
                                        }
                                        Image = ImageUrl;
                                        imageResult.Image = Image;
                                        ImageRead?.Invoke(imageResult);
                                    }
                                    break;
                                case ("Kinogo", true):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='excerpt']");
                                    if (node != null)
                                    {
                                        Description = node.InnerText.Trim();
                                    }
                                    else
                                    {
                                        Description = "Описание не найдено";
                                    }
                                    descriptionResult.Description = Description;
                                    DescriptionRead?.Invoke(descriptionResult);
                                    break;
                                case ("Jutsu", true):
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
                                        Description = $"Информация о {query} не найдена";
                                    }
                                    descriptionResult.Description = Description;
                                    DescriptionRead?.Invoke(descriptionResult);
                                    break;
                                case ("AnimeGo", true):
                                    HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='h5 font-weight-normal mb-2 card-title text-truncate']/a");

                                    if (nodes != null && nodes.Count > 0)
                                    {
                                        foreach (HtmlNode aNode in nodes)
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
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    descriptionResult.Description = Description;
                                    DescriptionRead?.Invoke(descriptionResult);
                                    break;
                                case ("AnimeGo", false):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='anime-grid-lazy lazy']");
                                    if (node != null)
                                    {
                                        Image = node.GetAttributeValue("data-original", "");
                                        imageResult.Image = Image;
                                        ImageRead?.Invoke(imageResult);
                                    }
                                    break;
                                case ("PremierGo", false):
                                    node = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'e-poster__Image-wrap')]/figure/img");

                                    if (node != null)
                                    {
                                        Image = node.GetAttributeValue("src", "");
                                        imageResult.Image = Image;
                                        ImageRead?.Invoke(imageResult);
                                    }
                                    break;
                                case ("YouTube", _):
                                    //try
                                    //{
                                    //    var youtube = new YoutubeClient();
                                    //    var searchResults = await youtube.Search.GetVideosAsync($"{query} {type} трейлер");
                                    //    var firstVideo = searchResults.FirstOrDefault();

                                    //    if (firstVideo != null)
                                    //    {
                                    //        YouTube = $"https://www.youtube.com/embed/{firstVideo.Id}";
                                    //        result.YouTubeUrl = YouTube;
                                    //    }
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    Console.WriteLine($"Ошибка YouTube API: {ex.Message}");
                                    //}
                                    break;
                                case ("DateExit", _):
                                    node = htmlDocument.DocumentNode.SelectSingleNode($"//div[@class='content']//a[contains(., '{query}')]");
                                    DateExitIsSuccess(node, client, query, type, htmlDocument);

                                    nextEpisodeResult.NextEpisodeReleaseDate = NextEpisodeReleaseDate;
                                    NextEpisodeReleaseDateRead?.Invoke(nextEpisodeResult);

                                    countResult.CountLabel = CountLabel;
                                    CountLabelRead?.Invoke(countResult);

                                    dateReleaseResult.DateRelease = DateRelease;
                                    DateReleaseRead?.Invoke(dateReleaseResult);

                                    if (!string.IsNullOrEmpty(Image))
                                    {
                                        imageResult.Image = Image;
                                        ImageRead?.Invoke(imageResult);
                                    }

                                    break;
                                case ("LordsFilm", false):
                                    // Использовалось раньше для взятия картинки, возможны проблемы с источником, пересмотреть логику снизу
                                    break;
                            }
                        }
                        else
                        {
                            switch (sourcePars, isInfo)
                            {
                                case ("Википедия", true):
                                    WikInfoIsWrong(query, type);
                                    break;
                                default:
                                    Description = "Ошибка при получении страницы";
                                    break;
                            }
                            descriptionResult.Description = Description;
                            DescriptionRead?.Invoke(descriptionResult);
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

        public async void WikInfoIsSuccess(HtmlNode node, HtmlDocument htmlDocument, string query)
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
                    Description = $"Информация о {query} не найдена";

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

        public async void WikInfoIsWrong(string query, string type)
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
                        Description = $"Информация о {query} не найдена";
                    }
                }

                else
                {
                    Description = $"Информация о {query} не найдена";
                }
            }
        }

        //Получение трейлера для перехода по кнопке
        public async Task<string> GetTrailer(string query, string type)
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

                        string pattern = "\\\\vi\\\\/([^\\/\\\\\"]+)";
                        Match match = Regex.Match(htmlDocument.DocumentNode.OuterHtml, pattern);

                        if (match.Success)
                        {
                            return $"https://www.youtube.com/watch?v={match.Groups[1].Value}";
                        }
                    }
                    return "https://www.youtube.com/watch?v"; // Возвращаем URL по умолчанию
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async void DateExitIsSuccess(HtmlNode node, HttpClient client, string query, string type, HtmlDocument htmlDocument)
        {
            if (node != null)
            {
                string link = node.GetAttributeValue("href", "");
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

                                NextEpisodeReleaseDate = output;
                                CountLabel = countText;
                                DateRelease = releaseDate.ToShortDateString();
                            }
                        }

                        else
                        {
                            exitEpisod = exitEpisod.Replace(".", ".");

                            NextEpisodeReleaseDate = exitEpisod;
                            CountLabel = countText;
                        }
                    }
                    else
                    {
                        NextEpisodeReleaseDate = $"Информация о {query} не найдена";
                    }

                    if (type == "Сериал" || type == "Дорама" || type == "Мультсериал")
                    {
                        HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                        if (imgIn != null)
                        {
                            string ImageUrl = imgIn.GetAttributeValue("src", "");

                            // Проверяем, содержит ли URL префикс "https://"
                            if (!ImageUrl.StartsWith("https://"))
                            {
                                // Добавляем префикс "https://", если его нет
                                ImageUrl = "https:" + ImageUrl;
                            }
                            Image = ImageUrl;
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
                node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='content']//a");
                if (node != null)
                {
                    string link = node.GetAttributeValue("href", "");
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
                                    NextEpisodeReleaseDate = output;
                                    CountLabel = countText;
                                    DateRelease = releaseDate.ToShortDateString();
                                }
                            }

                            else
                            {
                                // Заменяем каждую точку на точку с отступом и символ перевода строки
                                exitEpisod = exitEpisod.Replace(".", ".");

                                // Устанавливаем отформатированную строку в NextEpisodeReleaseDateEntry
                                NextEpisodeReleaseDate = exitEpisod;
                                CountLabel = countText;
                            }
                        }
                        else
                        {
                            NextEpisodeReleaseDate = $"Информация о {query} не найдена";
                        }

                        if (type == "Сериал" || type == "Дорама" || type == "Мультсериал")
                        {
                            HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                            if (imgIn != null)
                            {
                                string ImageUrl = imgIn.GetAttributeValue("src", "");

                                // Проверяем, содержит ли URL префикс "https://"
                                if (!ImageUrl.StartsWith("https://"))
                                {
                                    // Добавляем префикс "https://", если его нет
                                    ImageUrl = "https:" + ImageUrl;
                                }
                                Image = ImageUrl;
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
