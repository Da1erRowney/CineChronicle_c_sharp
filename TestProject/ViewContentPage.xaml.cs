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

        // ��� ����� ��� �������� ������ �� ����� YouTube
        private async void OpenYouTubeVideo(string videoId)
        {
            var youtubeUrl = $"https://www.youtube.com/watch?v={videoId}";

            try
            {
                await Launcher.OpenAsync(youtubeUrl);
            }
            catch (Exception ex)
            {
                // ��������� ������, ���� URL-����� �� ����� ���� ������
                Console.WriteLine($"Unable to open YouTube video: {ex.Message}");
            }

        }
     

        public ViewContentPage(Content content)
        {
            this.content = content;
            InitializeComponent();
            BindingContext = content; // ����������� ������ Content � BindingContext ��������
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

                            // ���� ��� �������� ������ (���� <li>) ������ �������� � id="mw-content-text"
                            var listItems = htmlDocument.DocumentNode.SelectNodes("//div[@id='mw-content-text']//li");
                            var linkNode = htmlDocument.DocumentNode.SelectSingleNode("//a[@title='���� ��������� (����������)']");
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

                                        if (response.IsSuccessStatusCode)
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

                                        if (listItemText.Contains("����������") || listItemText.Contains("������") || listItemText.Contains("�����������"))
                                        {
                                            listItemText = HtmlEntity.DeEntitize(listItemText);
                                            DescriptionLabel.Text = listItemText;
                                            return;
                                        }
                                    }
                                }

                                DescriptionLabel.Text = "���������� � ������� �� �������1";

                            }
                        }
                    }

                    else
                    {

                        DescriptionLabel.Text = "���������� � ������� �� �������2";


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
                        // ���������� ������ �� �����������
                        HtmlNode imageDiv = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='anime-grid-lazy lazy']");
                        if (imageDiv != null)
                        {
                            string imageUrl = imageDiv.GetAttributeValue("data-original", "");
                            // ���������� ����������� �� �����
                            PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
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
                    // ��������� ������
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
                    // ��������� ������
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




        //public void MediaPlayerApi(string link)
        //{
        //    string apiKey = "YOUR_API_KEY";

        //    // �������� ������ YouTube Data API
        //    YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        //    {
        //        ApiKey = apiKey,
        //        ApplicationName = "YourAppName"
        //    });

           
        //}


        private async void SetupLabelTappedEvents()
        {
            // ��� ������������� � �����, ��� ��� � ��� ������ ���� �����
            // ����� ������ �������� ���������� ��� ���� �����

            if (LinkLabel != null)
            {
                LinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = OpenLinkCommand,
                    CommandParameter = (BindingContext as Content)?.Link // �������� ������ � �������� ��������� �������
                });

            }

            // �������� ����� ��� ��������� ���������� � ��������� ��� �������� ��������
            string title = (BindingContext as Content)?.Title;
            string type = (BindingContext as Content)?.Type;
            switch (type)
            {
                case "�����":
                    GetAnemeGoInfo(title);
                    GetAnimeGoImage(title);
                    break;
                case "�����":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;
                case "������":
                    GetWikipediaInfo(title);
                   // GetWikipediaImage(title);
                    break;
                case "������":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;
                case "�����������":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
                    break;
                case "������":
                    GetWikipediaInfo(title);
                    GetWikipediaImage(title);
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
            EditButton.Text = "��������";
            CancelButton.IsVisible = false; // ������ ������ "������"

            // ����������� ���� �����
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
                DateAddedEntry.Text = content.DateAdded;
            }

            // ��������� ���� �����
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
                // ���������, �������� �� ���������� ���������
                if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                {
                    // ��������� ������ �� ������� � �������������� �������������� ���������� YouTube
                    await Launcher.OpenAsync(new Uri("vnd.youtube://" + trailerUrl.Split('=')[1]));
                }
                else
                {
                    // ��������� ������ �� ������� �� ������� ��������
                    await Launcher.OpenAsync(new Uri(trailerUrl));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ��� �������� ��������: {ex.Message}");
            }
        }



        private async void TrailerButton_Clicked(object sender, EventArgs e)
        {
            string url = (BindingContext as Content)?.LinkTrailer;
            OpenLink(url);




        }

    }

}

  
