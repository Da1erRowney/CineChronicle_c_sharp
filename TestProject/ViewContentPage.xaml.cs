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
using System.Xml;


namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ViewContentPage : ContentPage
    {
        public ICommand OpenLinkCommand { get; private set; }
        private Content content;
        string videoUrl=null;



        public ViewContentPage(Content content)
        {
            this.content = content;
            InitializeComponent();
            BindingContext = content; // ����������� ������ Content � BindingContext ��������
            SetupLabelTappedEvents();
            OpenLinkCommand = new Command<string>(OpenLink);

        }

        public async void GetWikipediaInfo(string query)
        {
            string url = $"https://ru.wikipedia.org/wiki/{Uri.EscapeDataString(query)} (����������)";

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
                            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                            content = databaseService.GetContentById(content.Id);
                            content.SmallDecription = firstParagraphText;
                            databaseService.UpdateContent(content);
                            databaseService.CloseConnection();
                            //GetWikipediaImage(query+" (����������)");
                        }

                        else
                        {
                            // ���� ��� �������� ������ (���� <li>) ������ �������� � id="mw-content-text"
                            var listItems = htmlDocument.DocumentNode.SelectNodes("//div[@id='mw-content-text']//li");
                            var linkNode = htmlDocument.DocumentNode.SelectSingleNode($"//a[@title='{Uri.EscapeDataString(query)} (����������)']");
                            // ���������, ��� ������� ������
                            if (linkNode != null)
                            {
                                // �������� �������� �������� href
                                string hrefValue = linkNode.GetAttributeValue("href", "");
                                // ��������� ������ � ��������� ����������
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

                                            // ������� ������� img � ��������� src, ���������� ������ �� �����������
                                            HtmlNode imageNodes = htmlDocuments.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                                            if (imageNodes != null)
                                            {
                                                string imageUrls = imageNodes.GetAttributeValue("src", "");

                                                // ���������, �������� �� URL ������� "https://"
                                                if (!imageUrls.StartsWith("https://"))
                                                {
                                                    // ��������� ������� "https://", ���� ��� ���
                                                    imageUrls = "https:" + imageUrls;
                                                }
                                                // ���������� ����������� �� �����
                                                PosterImage.Source = ImageSource.FromUri(new Uri(imageUrls));
                                                Background.Source = ImageSource.FromUri(new Uri(imageUrls));

                                                string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                                DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                                content = databaseService.GetContentById(content.Id);
                                                content.Image = imageUrls;
                                                databaseService.UpdateContent(content);
                                                databaseService.CloseConnection();
                                            }
                                        }
                                        else
                                        {
                                            // ��������� ������ ��� ���������� �������
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // ��������� ������
                                    }

                                }
                                if (listItems != null)
                                {
                                    foreach (var listItem in listItems)
                                    {
                                        string listItemText = listItem.InnerText;

                                        if (listItemText.Contains("����������") || listItemText.Contains("������") || listItemText.Contains("�����������") || listItemText.Contains("�����"))
                                        {
                                            listItemText = HtmlEntity.DeEntitize(listItemText);
                                            DescriptionLabel.Text = listItemText;
                                            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                            content = databaseService.GetContentById(content.Id);
                                            content.SmallDecription = listItemText;
                                            databaseService.UpdateContent(content);
                                            databaseService.CloseConnection();
                                            return;
                                        }
                                    }
                                }
                                DescriptionLabel.Text = "���������� � ������� �� �������";

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

                                        string nameContent1 = $"{query}:\n";



                                        string firstParagraphText = firstParagrap1h.InnerText;
                                        firstParagraphText = HtmlEntity.DeEntitize(firstParagraphText);
                                        DescriptionLabel.Text = firstParagraphText;
                                        string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                        DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                        content = databaseService.GetContentById(content.Id);
                                        content.SmallDecription = firstParagraphText;
                                        databaseService.UpdateContent(content);
                                        databaseService.CloseConnection();
                                        //GetWikipediaImage(query);

                                    }
                                }
                            }
                        }
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

                                string nameContent1 = $"{query}:\n";


                                if (firstParagrap1h != null && firstParagrap1h.InnerText != nameContent1 && !firstParagrap1h.InnerText.Trim().EndsWith(":"))
                                {
                                    string firstParagraphText = firstParagrap1h.InnerText;
                                    firstParagraphText = HtmlEntity.DeEntitize(firstParagraphText);
                                    DescriptionLabel.Text = firstParagraphText;
                                    string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                    DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                    content = databaseService.GetContentById(content.Id);
                                    content.SmallDecription = firstParagraphText;
                                    databaseService.UpdateContent(content);
                                    databaseService.CloseConnection();
                                    //GetWikipediaImage(query);
                                }
                                else
                                {
                                    var listItems = htmlDocument1.DocumentNode.SelectNodes("//div[@id='mw-content-text']//li");

                                        if (listItems != null)
                                        {
                                            foreach (var listItem in listItems)
                                            {
                                                string listItemText = listItem.InnerText;

                                            string search = content.Type;
                                            if (search == "������")
                                            {
                                                 search = "����������";
                                            }
                                           

                                                if (listItemText.Contains(search))
                                                {
                                                    listItemText = HtmlEntity.DeEntitize(listItemText);
                                                    DescriptionLabel.Text = listItemText;
                                                    string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                                    DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                                    content = databaseService.GetContentById(content.Id);
                                                    content.SmallDecription = listItemText;
                                                    databaseService.UpdateContent(content);
                                                    databaseService.CloseConnection();
                                                    return;
                                                }
                                            }
                                        }
                                        DescriptionLabel.Text = "���������� � ������� �� �������";

                                    }
                                }

                            
                            else
                            {
                                DescriptionLabel.Text = "���������� � ������� �� �������2";
                            }
                        }
                       
                    }
                }
                catch (Exception ex)
                {
                    DescriptionLabel.Text = $"������: {ex.Message}";
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
                            DescriptionLabel.Text = "�������� �� �������";
                        }
                    }
                    else
                    {
                        DescriptionLabel.Text = "������ ��� ��������� ��������";
                    }
                }
                catch (Exception ex)
                {
                    DescriptionLabel.Text = $"������: {ex.Message}";
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

                        // ����� ������ � ������� ����������� ���������
                        Match match = Regex.Match(htmlContent, @"<yass-span class=""b-serp-url__item"">(.*?)</yass-span>");

                        if (match.Success)
                        {
                            string urlFromHtml = match.Groups[1].Value;
                            DescriptionLabel.Text = urlFromHtml;
                        }
                        else
                        {
                            // ���� ������ �� �������, ���������� ��������������� �������
                            DescriptionLabel.Text = "Description not found.";
                        }
                    }
                    else
                    {
                        // ��������� ������ HTTP-�������
                        DescriptionLabel.Text = "Error fetching data from the website.";
                    }
                }
                catch (Exception ex)
                {
                    // ��������� ����������
                    DescriptionLabel.Text = $"An error occurred: {ex.Message}";
                }
            }
        }

        public async void GetAnimeGoImage(string query)
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
                        // ���������� ������ �� �����������
                        HtmlNode imageDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='anime-grid-lazy lazy']");
                        if (imageDiv != null)
                        {
                            string imageUrl = imageDiv.GetAttributeValue("data-original", "");
                            // ���������� ����������� �� �����
                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                            Background.Source= ImageSource.FromUri(new Uri(imageUrl));

                            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

                            // ������� ��������� ������� ���� ������
                            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                            content = databaseService.GetContentById(content.Id);
                            content.Image = imageUrl;
                            databaseService.UpdateContent(content);
                            databaseService.CloseConnection();
                        }
                    }
                    else
                    {
                        // ��������� ������ ��� ���������� �������
                    }
                }
                catch (Exception ex)
                {
                    // ��������� ������
                }
            }
        }
        private async void GetPremierImage(string query)
        {
            string url = $"https://premier.one/search?query={Uri.EscapeDataString(query)}";

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

                        // ���������� ������ �� �����������
                        HtmlNode imageTag = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'e-poster__image-wrap')]/figure/img");

                        if (imageTag != null)
                        {
                            string imageUrl = imageTag.GetAttributeValue("src", "");

                            // ���������� ����������� �� �����
                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                            Background.Source = ImageSource.FromUri(new Uri(imageUrl));
                        }
                    }
                    else
                    {
                        // ��������� ������ ��� ���������� �������
                    }
                }
                catch (Exception ex)
                {
                    // ��������� ������
                }
            }
        }


        public async void GetWikipediaImage(string query)
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

                        // ������� ������� img � ��������� src, ���������� ������ �� �����������
                        HtmlNode imageNode = htmlDocument.DocumentNode.SelectSingleNode("//img[contains(@src, 'upload.wikimedia.org')]");

                        if (imageNode != null)
                        {
                            string imageUrl = imageNode.GetAttributeValue("src", "");

                            // ���������, �������� �� URL ������� "https://"
                            if (!imageUrl.StartsWith("https://"))
                            {
                                // ��������� ������� "https://", ���� ��� ���
                                imageUrl = "https:" + imageUrl;
                            }
                            // ���������� ����������� �� �����
                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                            Background.Source = ImageSource.FromUri(new Uri(imageUrl));

                            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                            content = databaseService.GetContentById(content.Id);
                            content.Image = imageUrl;
                            databaseService.UpdateContent(content);
                            databaseService.CloseConnection();
                        }
                    }
                    else
                    {
                        // ��������� ������ ��� ���������� �������
                    }
                }
                catch (Exception ex)
                {
                    // ��������� ������
                }
            }
        }

        public async void GetAnemeGoInfo(string query)
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

                                url = extractedLink;

                                using (HttpClient client� = new HttpClient())
                                {
                                    try
                                    {
                                        HttpResponseMessage response� = await client�.GetAsync(url);

                                        if (response�.IsSuccessStatusCode)
                                        {
                                            string htmlContentSearch = await response�.Content.ReadAsStringAsync();

                                            string pattern = @"<div data-readmore=""content"">\s+(.*?)\s+</div></div></div><div class=""mt-3"">";
                                            Match match = Regex.Match(htmlContentSearch, pattern, RegexOptions.Singleline);

                                            if (match.Success)
                                            {
                                                extractedText = match.Groups[1].Value.Trim();
                                                extractedText = HtmlEntity.DeEntitize(extractedText);
                                                extractedText = Regex.Replace(extractedText, "<.*?>", String.Empty);
                                                DescriptionLabel.Text = extractedText;

                                                string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                                DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                                content = databaseService.GetContentById(content.Id);
                                                content.SmallDecription = extractedText;
                                                databaseService.UpdateContent(content);
                                                databaseService.CloseConnection();
                                            }
                                            else
                                            {
                                                string patterns = @"<div data-readmore=""content"">(.*?)</div></div></div><div class=""mt-3"">";
                                                Match matchs = Regex.Match(htmlContentSearch, patterns, RegexOptions.Singleline);

                                                if (matchs.Success)
                                                {
                                                    string extractedTexts = matchs.Groups[1].Value;
                                                    extractedTexts = HtmlEntity.DeEntitize(extractedTexts);
                                                    extractedTexts = Regex.Replace(extractedTexts, "<.*?>", String.Empty);
                                                    DescriptionLabel.Text = extractedTexts;

                                                    string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                                    DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                                    content = databaseService.GetContentById(content.Id);
                                                    content.SmallDecription = extractedTexts;
                                                    databaseService.UpdateContent(content);
                                                    databaseService.CloseConnection();
                                                }
                                            }
                                           
                                        }
                                        else
                                        {
                                            DescriptionLabel.Text = $"�������� ��� {query} �� ���� �������";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // ��������� ������
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    // ��������� ������
                }
            }

        }

        private async void GetTrailer(string query, string type)
        {
            string url = $"https://www.youtube.com/results?search_query={query}+{type}+�������";
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

                        string pattern = "\\\\/vi\\\\/([^\\/\\\\\"]+)";
                        Match match = Regex.Match(htmlDocument.DocumentNode.OuterHtml, pattern);

                        if (match.Success)
                        {
                            extractedLink = match.Groups[1].Value;
                            videoUrl = $"https://www.youtube.com/embed/{extractedLink}";

                            TrailerWeb.Source = videoUrl;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private async void GetTrailers(string query, string type)
        {
            string url = $"https://www.youtube.com/results?search_query={query}+{type}+�������";
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

                        string pattern = "\\\\/vi\\\\/([^\\/\\\\\"]+)";
                        Match match = Regex.Match(htmlDocument.DocumentNode.OuterHtml, pattern);

                        if (match.Success)
                        {
                            extractedLink = match.Groups[1].Value;
                            videoUrl = $"https://www.youtube.com/watch?v={extractedLink}";

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

        //private async void GetLordsFilmImage(string query)
        //{
        //    string url = $"https://www.google.by/search?q= {Uri.EscapeDataString(query)} ������&tbm=isch&ved=2ahUKEwiZtra589-EAxW8if0HHa5CCkYQ2-cCegQIABAA&oq=�&gs_lp=EgNpbWciAtCwSJwUUJsSWKoTcAB4AJABAJgBsAGgAbABqgEDMC4xuAEDyAEA-AEBigILZ3dzLXdpei1pbWeoAgDCAgoQABiABBiKBRhDiAYB&sclient=img&ei=-4ToZdnMOryT9u8ProWpsAQ";

        //    using (HttpClient client = new HttpClient())
        //    {
        //        try
        //        {
        //            HttpResponseMessage response = await client.GetAsync(url);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                string htmlContent = await response.Content.ReadAsStringAsync();

        //                // ���������� ������ �� �����������
        //                Match match = Regex.Match(htmlContent, @"<img\s+src\s*=\s*""([^""]+)""");


        //                if (match.Success)
        //                {
        //                    string imageUrl = match.Groups[1].Value;
        //                    // ���������� ����������� �� �����
        //                    PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
        //                }
        //                else
        //                {
        //                    // ��������� ������, ����� ������ �� ����������� �� �������
        //                }
        //            }
        //            else
        //            {
        //                HttpStatusCode statusCode = response.StatusCode;
        //                // ��������� ������ �� ������ ���� ���������
        //                Console.WriteLine($"HTTP Error: {statusCode}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // ��������� ������
        //        }
        //    }
        //}
        public async void DataExitNextEpisod(string query)
        {
            string url = $"https://www.toramp.com/ru/search/?q={query}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(htmlContent);

                    // ���������� ������ �� HTML
                    HtmlNode linkNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='content']//a");
                    if (linkNode != null)
                    {
                        string link = linkNode.GetAttributeValue("href", "");
                        HttpResponseMessage responseIn = await client.GetAsync(link);

                        if (responseIn.IsSuccessStatusCode)
                        {
                            string htmlContentIn = await responseIn.Content.ReadAsStringAsync();

                            HtmlDocument htmlDocumentIn = new HtmlDocument();
                            htmlDocumentIn.LoadHtml(htmlContentIn);

                            // ���������� ������ �� HTML
                            HtmlNode linkNodeIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_3']/em");
                            if (linkNodeIn != null)
                            {
                                string exitEpisod = linkNodeIn.InnerText;

                                int startIndex = exitEpisod.IndexOf("��������") + "��������".Length; // ������ ����� ����� "��������"
                                int daysIndex = exitEpisod.IndexOf("����", startIndex); // ������ ����� "����" ����� startIndex

                                if (daysIndex == -1)
                                {
                                    daysIndex = exitEpisod.IndexOf("���", startIndex); // ������ ����� "����" ����� startIndex
                                }

                                if (daysIndex != -1)
                                {
                                    // ��������� ��������� ����� startIndex � daysIndex
                                    string daysString = exitEpisod.Substring(startIndex, daysIndex - startIndex).Trim();

                                    if (int.TryParse(daysString, out int days))
                                    {
                                        // ��������� ���� ����� ��������� ���������� ����
                                        DateTime releaseDate = DateTime.Today.AddDays(days);

                                        // ��������� ������ ��� ������
                                        string output = $"�������� {days} ���� - ({releaseDate.ToShortDateString()})";

                                        // ������������� ������ � NextEpisodeReleaseDateEntry
                                        NextEpisodeReleaseDateEntry.Text = output;
                                    }
                                }
                                else
                                {

                                    // �������� ������ ����� �� ����� � �������� � ������ �������� ������
                                    exitEpisod = exitEpisod.Replace(".", ". \n");

                                    // ������������� ����������������� ������ � NextEpisodeReleaseDateEntry
                                    NextEpisodeReleaseDateEntry.Text = exitEpisod;


                                    // ������������� ����������������� ������ � NextEpisodeReleaseDateEntry
                                    NextEpisodeReleaseDateEntry.Text = exitEpisod;

                                }
                            }
                            else
                            {
                                Console.WriteLine("�� ������� ����� �����.");
                            }

                            if (content.Type == "������" || content.Type == "������" || content.Type == "����������" )
                            {
                                        HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                                        if (imgIn != null)
                                        {
                                            string imageUrl = imgIn.GetAttributeValue("src", "");

                                            // ���������, �������� �� URL ������� "https://"
                                            if (!imageUrl.StartsWith("https://"))
                                            {
                                                // ��������� ������� "https://", ���� ��� ���
                                                imageUrl = "https:" + imageUrl;
                                            }
                                            // ������������� ����������� � �������� UI
                                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                                            Background.Source = ImageSource.FromUri(new Uri(imageUrl));

                                            // ��������� ������ �� ����������� � ���� ������
                                            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
                                            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
                                            content = databaseService.GetContentById(content.Id);
                                            content.Image = imageUrl;
                                            databaseService.UpdateContent(content);
                                            databaseService.CloseConnection();
                                        }
                                        else
                                        {
                                            //Console.WriteLine("����������� �� �������.");
                                        }
                            }






                                
                            

                        }
                        else
                        {
                            Console.WriteLine("�� ������� ��������� ������ � �����.");
                        }

                    }
                    else
                    {
                        Console.WriteLine("������ �� �������.");
                    }
                }
                else
                {
                    Console.WriteLine("�� ������� ��������� ������ � �����.");
                }
            }
        }

        private async void SetupLabelTappedEvents()
        {

            // �������� ����� ��� ��������� ���������� � ��������� ��� �������� ��������
            string title = (BindingContext as Content)?.Title;
            string type = (BindingContext as Content)?.Type;
            GetTrailer(title, type);
            switch (type)
            {
                case "�����":
                    GetAnemeGoInfo(title);
                    GetAnimeGoImage(title);
                    DataExitNextEpisod(title);
                    WatchingButton.Source = "anime.png";
                    break;
                case "�����":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    DataExitNextEpisod(title);
                    WatchingButton.Source = "movie.png";
                    break;
                case "������":
                    GetWikipediaInfo(title);
                    //GetWikipediaImage(title);
                    DataExitNextEpisod(title);
                    //GetPremierImage(title);
                    WatchingButton.Source = "movie.png";
                    break;
                case "������":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    DataExitNextEpisod(title);
                    WatchingButton.Source = "dorama.png";
                    break;
                case "�����������":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    DataExitNextEpisod(title);
                    WatchingButton.Source = "movie.png";
                    break;
                case "������":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    DataExitNextEpisod(title);
                    WatchingButton.Source = "movie.png";
                    break;
                default:
                    WatchingButton.Source = "movie.png";
                    break;
            }

        }

        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                // ��������� ������ � ��������*-*+9
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("�����������", $"�� �������, ��� ������ ������� {content.Title}?", "��", "���");

            if (result)
            {
                string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

                DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

                databaseService.DeleteContent(content);

                databaseService.CloseConnection();

                await Navigation.PushAsync(new MainPage());
            }
        }



        private bool isEditing = false; // ����, �����������, � ������ �������������� ��� ���

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            if (isEditing)
            {
                // ���� ��� � ������ ��������������, �� ����� ��������� ���������
                SaveChanges();
            }
            else
            {
                // ���� �� � ������ ��������������, �� ������������� � ���� �����
                StartEditing();
            }
        }

        private void StartEditing()
        {
            isEditing = true;
            EditButton.Text = "���������";
            CancelButton.IsVisible = true; // ���������� ������ "������"

            // �������������� ���� �����
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
            NextEpisodeReleaseDateEntry.IsReadOnly = false;
            WatchStatusEntry.IsReadOnly = false;
        }

        private void SaveChanges()
        {
            isEditing = false;
            EditButton.Text = "��������";
            CancelButton.IsVisible = false; // ������ ������ "������"

            // ����������� ���� �����
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

            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            NextEpisodeReleaseDateEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
          


            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // ������� ��������� ������� ���� ������
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
            content.SeriesChangeDate = content.SeriesChangeDate;

            }
            switch (content.Type)
            {
                case "�����":
                    content.Link = "https://animego.org/search/all?q=" + TitleEntry.Text;
                    break;
                case "������":
                    content.Link = "https://dorama.land/search?q=" + TitleEntry.Text;
                    break;
                case "������":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case "�����������":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case "�����":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                default:
                    break;
            }
           
            content.LastWatchedSeries =  int.Parse(LastWatchedSeriesEntry.Text);
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);

            content.NextEpisodeReleaseDate = NextEpisodeReleaseDateEntry.Text;
            content.WatchStatus = WatchStatusPicker.SelectedItem.ToString() ;
            content.DateAdded = content.DateAdded;
            databaseService.UpdateContent(content);
            databaseService.CloseConnection();


            // �������� ������ � ��
            // ��� ��� ��� ���������� ������ � ��
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // �������� ��������� � ������������� �� ������ ��������������
            isEditing = false;
            EditButton.Text = "��������";
            CancelButton.IsVisible = false; // ������ ������ "������"

            // �������� ���� � ���� ������
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // ������� ��������� ������� ���� ������
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

            // �������� ������ �� ���� ������ �� ID
            content = databaseService.GetContentById(content.Id);

            // ��������� ������� ������ � ������� content
            if (content != null)
            {
                // ��������� ���� ����� ������� �� ������� content
                TitleEntry.Text = content.Title;
                TypeEntry.Text = content.Type;
                DubbingEntry.Text = content.Dubbing;
                LastWatchedSeriesEntry.Text = content.LastWatchedSeries.ToString();
                LastWatchedSeasonEntry.Text = content.LastWatchedSeason.ToString();
                NextEpisodeReleaseDateEntry.Text = content.NextEpisodeReleaseDate;
                WatchStatusEntry.Text = content.WatchStatus;
                
            }

            // ��������� ���� �����
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

            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            NextEpisodeReleaseDateEntry.IsReadOnly = true;
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
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // ������� ��������� ������� ���� ������
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
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
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // ������� ��������� ������� ���� ������
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
            content = databaseService.GetContentById(content.Id);
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);
            content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);
            databaseService.UpdateContent(content);
            databaseService.CloseConnection();
        }
    }

}

  
