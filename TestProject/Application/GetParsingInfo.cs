using HtmlAgilityPack;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace CineChronicle.Application
{
    public class GetParsingInfo
    {
        //Контент
        public string description { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;

        //Трейлер
        public string youTube { get; set; } = string.Empty;

        //Дата выхода
        public string nextEpisodeReleaseDate { get; set; } = string.Empty;
        public string countLabel { get; set; } = string.Empty;
        public string dateRelease { get; set; } = string.Empty;

        /// <summary>
        /// Находим информацию и постер контента
        /// </summary>
        /// <param name="query">Искомый запрос(название контента)</param>
        /// <param name="type">Тип контента</param>
        /// <param name="sourcePars">Источник из которого берем инфу</param>
        /// <param name="isInfo">Если информация, иначе картинка(постер)</param>
        public async void GetInfo(string query, string type, string sourcePars, bool isInfo, bool debug = false)
        {
            string url = "";
            switch (sourcePars, isInfo)
            {
                case ("Википедия", true):
                    url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)} (телесериал)";
                    break;
                case ("Википедия", false):
                    url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)}";
                    break;
                case ("Киного", true):
                    url = $"https://kinogo.biz/search/{Uri.EscapeDataString(query)}";
                    break;
                case ("Джутсу", true):
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    url = $"https://jut.su/search/?searchid=1893616&text={Uri.EscapeDataString(query)}";
                    break;
                case ("АнимеГо", true):
                    url = "https://animego.org/search/all?q=" + query;
                    break;
                case ("АнимеГо", false):
                    url = $"https://animego.org/search/all?q={Uri.EscapeDataString(query)}";
                    break;
                case ("Премьер", false):
                    url = $"https://premier.one/search?query={Uri.EscapeDataString(query)}";
                    break;
                case ("Трейлер", false):
                    url = $"https://www.youtube.com/results?search_query={query}+{type}+трейлер";
                    break;
                case ("Лордс", false):
                    //url = $"https://www.google.by/search?q= {Uri.EscapeDataString(query)} Постер&tbm=isch&ved=2ahUKEwiZtra589-EAxW8if0HHa5CCkYQ2-cCegQIABAA&oq=а&gs_lp=EgNpbWciAtCwSJwUUJsSWKoTcAB4AJABAJgBsAGgAbABqgEDMC4xuAEDyAEA-AEBigILZ3dzLXdpei1pbWeoAgDCAgoQABiABBiKBRhDiAYB&sclient=img&ei=-4ToZdnMOryT9u8ProWpsAQ";
                    break;
                case ("ДатаВыхода", true):
                    url = $"https://www.toramp.com/ru/search/?q={query}";
                    break;
                default:
                    url = "";
                    break;
            }

            if (!string.IsNullOrEmpty(url))
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
                        HtmlNode node;
                        string extractedText = "";
                        string extractedLink = "";
                        switch (sourcePars, isInfo)
                        {
                            case ("Википедия", true):
                                node = htmlDocument.DocumentNode.SelectSingleNode("//p");
                                WikInfoIsSuccess(node, htmlDocument, query);
                                break;
                            case ("Википедия", false):
                                node = htmlDocument.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                                if (node != null)
                                {
                                    string imageUrl = node.GetAttributeValue("src", "");

                                    // Проверяем, содержит ли URL префикс "https://"
                                    if (!imageUrl.StartsWith("https://"))
                                    {
                                        // Добавляем префикс "https://", если его нет
                                        imageUrl = "https:" + imageUrl;
                                    }
                                    image = imageUrl;
                                }
                                break;
                            case ("Киного", true):
                                node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='excerpt']");
                                if (node != null)
                                {
                                    description = node.InnerText.Trim();
                                }
                                else
                                {
                                    description = "Описание не найдено";
                                }
                                break;
                            case ("Джутсу", true):
                                byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
                                string htmlContents = Encoding.GetEncoding("windows-1251").GetString(responseBytes);

                                // Поиск ссылки с помощью регулярного выражения
                                Match match = Regex.Match(htmlContents, @"<yass-span class=""b-serp-url__item"">(.*?)</yass-span>");

                                if (match.Success)
                                {
                                    description = match.Groups[1].Value;
                                }
                                else
                                {
                                    description = $"Информация о {query} не найдена";
                                }
                                break;
                            case ("АнимеГо", true):
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
                                                    description = Regex.Replace(extractedText, "<.*?>", String.Empty);
                                                }
                                                else
                                                {
                                                    string patterns = @"<div data-readmore=""content"">(.*?)</div></div></div><div class=""mt-3"">";
                                                    Match matchss = Regex.Match(htmlContentSearch, patterns, RegexOptions.Singleline);

                                                    if (matchss.Success)
                                                    {
                                                        string extractedTexts = matchss.Groups[1].Value;
                                                        extractedTexts = HtmlEntity.DeEntitize(extractedTexts);
                                                        description = Regex.Replace(extractedTexts, "<.*?>", String.Empty);
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                break;
                            case ("АнимеГо", false):
                                node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='anime-grid-lazy lazy']");
                                if (node != null)
                                {
                                    image = node.GetAttributeValue("data-original", "");
                                }
                                break;
                            case ("Премьер", false):
                                node = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'e-poster__image-wrap')]/figure/img");

                                if (node != null)
                                {
                                    image = node.GetAttributeValue("src", "");
                                }
                                break;
                            case ("Трейлер", false):
                                extractedLink = "";
                                string pattern = "\\\\/vi\\\\/([^\\/\\\\\"]+)";
                                Match Tmatch = Regex.Match(htmlDocument.DocumentNode.OuterHtml, pattern);

                                if (Tmatch.Success)
                                {
                                    extractedLink = Tmatch.Groups[1].Value;
                                    youTube = $"https://www.youtube.com/embed/{extractedLink}";
                                }
                                break;
                            case ("ДатаВыхода", true):
                                node = htmlDocument.DocumentNode.SelectSingleNode($"//div[@class='content']//a[contains(., '{query}')]");
                                DateExitIsSuccess(node, client, query, type, htmlDocument);
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
                                description = "Ошибка при получении страницы";
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

        public async void WikInfoIsSuccess(HtmlNode node, HtmlDocument htmlDocument, string query)
        {
            string nameContent = $"{query}:\n";

            if (node != null && node.InnerText != nameContent && !node.InnerText.Trim().EndsWith(":"))
            {
                string firstParagraphText = node.InnerText;
                description = HtmlEntity.DeEntitize(firstParagraphText); // Находим описание для контента
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
                                    image = imageUrls;
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
                                description = listItemText;
                                return;
                            }
                        }
                    }
                    description = $"Информация о {query} не найдена";

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
                            description = HtmlEntity.DeEntitize(firstParagraphText);
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
                        description = HtmlEntity.DeEntitize(firstParagraphText);
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
                                    description = HtmlEntity.DeEntitize(listItemText);
                                    return;
                                }
                            }
                        }
                        description = $"Информация о {query} не найдена";
                    }
                }

                else
                {
                    description = $"Информация о {query} не найдена";
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

                                nextEpisodeReleaseDate = output;
                                countLabel = countText;
                                dateRelease = releaseDate.ToShortDateString();
                            }
                        }

                        else
                        {
                            exitEpisod = exitEpisod.Replace(".", ".");

                            nextEpisodeReleaseDate = exitEpisod;
                            countLabel = countText;
                        }
                    }
                    else
                    {
                        nextEpisodeReleaseDate = $"Информация о {query} не найдена";
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
                            image = imageUrl;
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
                                    nextEpisodeReleaseDate = output;
                                    countLabel = countText;
                                    dateRelease = releaseDate.ToShortDateString();
                                }
                            }

                            else
                            {
                                // Заменяем каждую точку на точку с отступом и символ перевода строки
                                exitEpisod = exitEpisod.Replace(".", ".");

                                // Устанавливаем отформатированную строку в NextEpisodeReleaseDateEntry
                                nextEpisodeReleaseDate = exitEpisod;
                                countLabel = countText;
                            }
                        }
                        else
                        {
                            nextEpisodeReleaseDate = $"Информация о {query} не найдена";
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
                                image = imageUrl;
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
