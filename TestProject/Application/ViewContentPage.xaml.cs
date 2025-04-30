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
        private const string WIK = "Âèêèïåäèÿ";
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

        private bool isEditing = false; // Ôëàã, óêàçûâàþùèé, â ðåæèìå ðåäàêòèðîâàíèÿ èëè íåò

        #endregion

        #region [Ctor's]
        public ViewContentPage(Content content)
        {
            this.content = content;
            InitializeComponent();
            BindingContext = content; // Ïðèâÿçûâàåì îáúåêò Content ê BindingContext ñòðàíèöû
            SetupLabelTappedEvents();
            OpenLinkCommand = new Command<string>(OpenLink);
        }

        public ViewContentPage(ContentRecommendation content)
        {
            this.recom = content;
            InitializeComponent();
            content.Type = recom.Type;
            BindingContext = content; // Ïðèâÿçûâàåì îáúåêò Content ê BindingContext ñòðàíèöû
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
         await Browser.OpenAsync(new Uri(parser?.GetTrailer(query,type)), BrowserLaunchMode.SystemPreferred); 
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

                    // Èçâëå÷åíèå ññûëêè èç HTML
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

                            // Èçâëå÷åíèå ññûëêè èç HTML
                            HtmlNode linkNodeIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_3']/em");
                            HtmlNode linkNodeCount = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_0']");
                            if (linkNodeIn != null || linkNodeCount != null)
                            {
                                string exitEpisod = linkNodeIn.InnerText;
                                string countText = linkNodeCount.InnerText.Trim();

                                int startIndex = exitEpisod.IndexOf("îñòàëîñü") + "îñòàëîñü".Length; // Èíäåêñ ïîñëå ñëîâà "îñòàëîñü"
                                int daysIndex = exitEpisod.IndexOf("äíåé", startIndex); // Èíäåêñ ñëîâà "äíåé" ïîñëå startIndex

                                if (daysIndex == -1)
                                {
                                    daysIndex = exitEpisod.IndexOf("äíÿ", startIndex); // Èíäåêñ ñëîâà "äíÿ" ïîñëå startIndex
                                }

                                if (daysIndex == -1)
                                {
                                    daysIndex = exitEpisod.IndexOf("äåíü", startIndex); // Èíäåêñ ñëîâà "äåíü" ïîñëå startIndex
                                }

                                if (daysIndex != -1)
                                {
                                    // Èçâëåêàåì ïîäñòðîêó ìåæäó startIndex è daysIndex
                                    string daysString = exitEpisod.Substring(startIndex, daysIndex - startIndex).Trim();

                                    if (int.TryParse(daysString, out int days))
                                    {
                                        DateTime releaseDate = DateTime.Today.AddDays(days);

                                        // Ôîðìèðóåì ñòðîêó äëÿ âûâîäà
                                        string output = $"Îñòàëîñü {days} äíåé äî âûõîäà ({releaseDate.ToShortDateString()})";

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
                                NextEpisodeReleaseDateEntry.Text = $"Èíôîðìàöèÿ î {query} íå íàéäåíà";
                            }

                            if (type == "Ñåðèàë" || type == "Äîðàìà" || type == "Ìóëüòñåðèàë")
                            {
                                HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                                if (imgIn != null)
                                {
                                    string imageUrl = imgIn.GetAttributeValue("src", "");

                                    // Ïðîâåðÿåì, ñîäåðæèò ëè URL ïðåôèêñ "https://"
                                    if (!imageUrl.StartsWith("https://"))
                                    {
                                        // Äîáàâëÿåì ïðåôèêñ "https://", åñëè åãî íåò
                                        imageUrl = "https:" + imageUrl;
                                    }
                                    // Óñòàíàâëèâàåì èçîáðàæåíèå â ýëåìåíòû UI
                                    PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                                    Background.Source = ImageSource.FromUri(new Uri(imageUrl));

                                    // Îáíîâëÿåì ññûëêó íà èçîáðàæåíèå â áàçå äàííûõ
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
                                    //Console.WriteLine("Èçîáðàæåíèå íå íàéäåíî.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Íå óäàëîñü âûïîëíèòü çàïðîñ ê ñàéòó.");
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

                                // Èçâëå÷åíèå ññûëêè èç HTML
                                HtmlNode linkNodeIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_3']/em");
                                HtmlNode linkNodeCount = htmlDocumentIn.DocumentNode.SelectSingleNode("//p[@class='mb_0']");
                                if (linkNodeIn != null || linkNodeCount != null)
                                {
                                    string exitEpisod = linkNodeIn.InnerText;
                                    string countText = linkNodeCount.InnerText.Trim();

                                    int startIndex = exitEpisod.IndexOf("îñòàëîñü") + "îñòàëîñü".Length; // Èíäåêñ ïîñëå ñëîâà "îñòàëîñü"
                                    int daysIndex = exitEpisod.IndexOf("äíåé", startIndex); // Èíäåêñ ñëîâà "äíåé" ïîñëå startIndex

                                    if (daysIndex == -1)
                                    {
                                        daysIndex = exitEpisod.IndexOf("äíÿ", startIndex); // Èíäåêñ ñëîâà "äíÿ" ïîñëå startIndex
                                    }

                                    if (daysIndex == -1)
                                    {
                                        daysIndex = exitEpisod.IndexOf("äåíü", startIndex); // Èíäåêñ ñëîâà "äåíü" ïîñëå startIndex
                                    }

                                    if (daysIndex != -1)
                                    {
                                        // Èçâëåêàåì ïîäñòðîêó ìåæäó startIndex è daysIndex
                                        string daysString = exitEpisod.Substring(startIndex, daysIndex - startIndex).Trim();

                                        if (int.TryParse(daysString, out int days))
                                        {
                                            // Âû÷èñëÿåì äàòó ÷åðåç óêàçàííîå êîëè÷åñòâî äíåé
                                            DateTime releaseDate = DateTime.Today.AddDays(days);

                                            // Ôîðìèðóåì ñòðîêó äëÿ âûâîäà
                                            string output = $"Îñòàëîñü {days} äíåé äî âûõîäà ({releaseDate.ToShortDateString()})";

                                            // Óñòàíàâëèâàåì ñòðîêó â NextEpisodeReleaseDateEntry
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

                                        // Çàìåíÿåì êàæäóþ òî÷êó íà òî÷êó ñ îòñòóïîì è ñèìâîë ïåðåâîäà ñòðîêè
                                        exitEpisod = exitEpisod.Replace(".", ".");

                                        // Óñòàíàâëèâàåì îòôîðìàòèðîâàííóþ ñòðîêó â NextEpisodeReleaseDateEntry
                                        NextEpisodeReleaseDateEntry.Text = exitEpisod;
                                        CountLabel.Text = countText;


                                    }
                                }
                                else
                                {
                                    NextEpisodeReleaseDateEntry.Text = $"Èíôîðìàöèÿ î {query} íå íàéäåíà";
                                }

                                if (type == "Ñåðèàë" || type == "Äîðàìà" || type == "Ìóëüòñåðèàë")
                                {
                                    HtmlNode imgIn = htmlDocumentIn.DocumentNode.SelectSingleNode("//div[@class='imgWrapper']/img");
                                    if (imgIn != null)
                                    {
                                        string imageUrl = imgIn.GetAttributeValue("src", "");

                                        // Ïðîâåðÿåì, ñîäåðæèò ëè URL ïðåôèêñ "https://"
                                        if (!imageUrl.StartsWith("https://"))
                                        {
                                            // Äîáàâëÿåì ïðåôèêñ "https://", åñëè åãî íåò
                                            imageUrl = "https:" + imageUrl;
                                        }
                                        // Óñòàíàâëèâàåì èçîáðàæåíèå â ýëåìåíòû UI
                                        PosterImage.Source = ImageSource.FromUri(new Uri(imageUrl));
                                        Background.Source = ImageSource.FromUri(new Uri(imageUrl));
                                        if (content != null)
                                        {
                                            // Îáíîâëÿåì ññûëêó íà èçîáðàæåíèå â áàçå äàííûõ
                                            
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
                                        //Console.WriteLine("Èçîáðàæåíèå íå íàéäåíî.");
                                    }
                                }









                            }
                            else
                            {
                                Console.WriteLine("Íå óäàëîñü âûïîëíèòü çàïðîñ ê ñàéòó.");
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Íå óäàëîñü âûïîëíèòü çàïðîñ ê ñàéòó.");
                }
            }
        }

        private async void SetupLabelTappedEvents()
        {
            // Âûçûâàåì ìåòîä äëÿ ïîëó÷åíèÿ èíôîðìàöèè ñ Âèêèïåäèè ïðè çàãðóçêå ñòðàíèöû
            string title = (BindingContext as Content)?.Title;
            string type = (BindingContext as Content)?.Type;

            GetInfo(type, title);
        }

        private async void SetupLabelTappedEventsRecom()
        {
            // Âûçûâàåì ìåòîä äëÿ ïîëó÷åíèÿ èíôîðìàöèè ñ Âèêèïåäèè ïðè çàãðóçêå ñòðàíèöû
            string title = (BindingContext as ContentRecommendation)?.Title;
            string type = (BindingContext as ContentRecommendation)?.Type;

            GetInfo(type, title);
        }

        private void GetInfo(string type, string title)
        {
            switch (type)
            {
                case "Àíèìå":
                    parser?.GetInfo(title, type, AG, true);
                    parser?.GetInfo(title, type, AG, false);
                    WatchingButton.Source = "anime.png";
                    break;
                case "Ôèëüì":
                case "Ñåðèàë":
                case "Äîðàìà":
                case "Ìóëüòñåðèàë":
                case "Ïðî÷åå":
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

            DescriptionLabel.Text = parser?.Description;
            PosterImage.Source = ImageSource.FromUri(new Uri(parser?.Image));
            Background.Source = ImageSource.FromUri(new Uri(parser?.Image));
            TrailerWeb.Source = parser?.YouTube;
            NextEpisodeReleaseDateEntry.Text = parser?.NextEpisodeReleaseDate;
            CountLabel.Text = parser?.CountLabel;
        }

        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                // Îòêðûâàåì ññûëêó â áðàóçåðå*-*+9
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Óâåäîìëåíèå", $"Âû óâåðåíû, ÷òî õîòèòå óäàëèòü {content.Title}?", "Äà", "Íåò");

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
                // Åñëè óæå â ðåæèìå ðåäàêòèðîâàíèÿ, òî íóæíî ñîõðàíèòü èçìåíåíèÿ
                SaveChanges();
            }
            else
            {
                // Åñëè íå â ðåæèìå ðåäàêòèðîâàíèÿ, òî ïåðåêëþ÷èòüñÿ â ýòîò ðåæèì
                StartEditing();
            }
        }

        private void StartEditing()
        {
            isEditing = true;
            EditButton.Text = "Ñîõðàíèòü";
            CancelButton.IsVisible = true; // Îòîáðàçèòü êíîïêó "Îòìåíà"

            // Ðàçáëîêèðîâàòü ïîëÿ ââîäà
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
            EditButton.Text = "Èçìåíèòü";
            CancelButton.IsVisible = false; // Ñêðûòü êíîïêó "Îòìåíà"

            // Áëîêèðîâàòü ïîëÿ ââîäà
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



            

            // Ñîçäàåì ýêçåìïëÿð ñåðâèñà áàçû äàííûõ
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
                case "Àíèìå":
                    content.Link = "https://animego.org/search/all?q=" + TitleEntry.Text;
                    break;
                case "Äîðàìà":
                    content.Link = "https://dorama.land/search?q=" + TitleEntry.Text;
                    break;
                case "Ñåðèàë":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case "Ìóëüòñåðèàë":
                    content.Link = "https://kinogo.biz/search/" + TitleEntry.Text;
                    break;
                case "Ôèëüì":
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

            // Îáíîâèòü äàííûå â ÁÄ
            // Âàø êîä äëÿ îáíîâëåíèÿ äàííûõ â ÁÄ
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // Îòìåíèòü èçìåíåíèÿ è ïåðåêëþ÷èòüñÿ èç ðåæèìà ðåäàêòèðîâàíèÿ
            isEditing = false;
            EditButton.Text = "Èçìåíèòü";
            CancelButton.IsVisible = false; // Ñêðûòü êíîïêó "Îòìåíà"

            // Ïîëó÷àåì ïóòü ê áàçå äàííûõ
            

            // Ñîçäàåì ýêçåìïëÿð ñåðâèñà áàçû äàííûõ
            DatabaseServiceContent databaseService = new DatabaseServiceContent(MainPage._databasePath);

            // Ïîëó÷àåì äàííûå èç áàçû äàííûõ ïî ID
            content = databaseService.GetContentById(content.Id);

            // Ïðîâåðÿåì íàëè÷èå äàííûõ â îáúåêòå content
            if (content != null)
            {
                // Çàïîëíÿåì ïîëÿ ââîäà äàííûìè èç îáúåêòà content
                TitleEntry.Text = content.Title;
                TypeEntry.Text = content.Type;
                DubbingEntry.Text = content.Dubbing;
                LastWatchedSeriesEntry.Text = content.LastWatchedSeries.ToString();
                LastWatchedSeasonEntry.Text = content.LastWatchedSeason.ToString();
                NextEpisodeReleaseDateEntry.Text = content.NextEpisodeReleaseDate;
                WatchStatusEntry.Text = content.WatchStatus;

            }

            // Áëîêèðóåì ïîëÿ ââîäà
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
            

            // Ñîçäàåì ýêçåìïëÿð ñåðâèñà áàçû äàííûõ
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
            

            // Ñîçäàåì ýêçåìïëÿð ñåðâèñà áàçû äàííûõ
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

  
