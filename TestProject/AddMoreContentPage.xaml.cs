using System;
using System.Collections.Generic;
using SQLite;
using DataContent;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace TestProject
{
    public partial class AddMoreContentPage : ContentPage
    {
        private DatabaseServiceContent _databaseService;
        string LinkYotube;
        public SQLiteConnection CreateDatabase(string databasePath)
        {
            SQLiteConnection connection = new SQLiteConnection(databasePath);
            connection.CreateTable<Content>();
            return connection;
        }

        public AddMoreContentPage()
        {
            InitializeComponent();

            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");
            _databaseService = new DatabaseServiceContent(databasePath);
            SQLiteConnection connection = CreateDatabase(databasePath);
            LastWatchedSeriesEntry.TextChanged += LastWatchedSeriesEntry_TextChanged;
            LastWatchedSeasonEntry.TextChanged += LastWatchedSeasonEntry_TextChanged;

        }
        private void LastWatchedSeriesEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ��������� ������ ��������� ������ � ����
            foreach (char c in e.NewTextValue)
            {
                // ���� ������ �� �������� ������, ������� ��� �� ������
                if (!char.IsDigit(c))
                {
                    LastWatchedSeriesEntry.Text = LastWatchedSeriesEntry.Text.Remove(LastWatchedSeriesEntry.Text.Length - 1);
                    break;
                }
            }
        }

        private void LastWatchedSeasonEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ��������� ������ ��������� ������ � ����
            foreach (char c in e.NewTextValue)
            {
                // ���� ������ �� �������� ������, ������� ��� �� ������
                if (!char.IsDigit(c))
                {
                    LastWatchedSeasonEntry.Text = LastWatchedSeasonEntry.Text.Remove(LastWatchedSeasonEntry.Text.Length - 1);
                    break;
                }
            }
        }
        private async void OnAddClicked(object sender, EventArgs e)
        {
            string m_type = TypePicker.SelectedItem?.ToString();
            string m_status = WatchStatusPicker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(TitleEntry.Text) || string.IsNullOrEmpty(m_type) || string.IsNullOrEmpty(m_status))
            {
                await DisplayAlert("������", "��������� ��������, ��� � ������ ��������� ��� ����������� ����������", "OK");
                return;
            }

            // �������� ��������� ���� �� DatePicker
            DateTime selectedDate = NextEpisodeReleaseDatePicker.Date;
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);

            // ������������ ��������� ���� � ������ � ������ ��������
            string formattedDate = selectedDate.ToString("dd.MM.yyyy"); // ��������, "01.02.2024"

            // ��������� ������ � ����������� �� ���������� ���� �������� � ��������� ���� LinkEntry
            string link = "";
            if (string.IsNullOrEmpty(LinkEntry.Text))
            {
                switch (m_type)
                {
                    case "�����":
                        link = "https://animego.org/search/all?q=" + TitleEntry.Text;
                        break;
                    case "������":
                        link = "https://dorama.land/search?q=" + TitleEntry.Text;
                        break;
                    case "������":
                        link = "https://kinogo.biz/search/" + TitleEntry.Text;
                        break;
                    case "�����������":
                        link = "https://kinogo.biz/search/" + TitleEntry.Text;
                        break;
                    case "�����":
                        link = "https://kinogo.biz/search/" + TitleEntry.Text;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                link = LinkEntry.Text;
            }
           


            string m_title = TitleEntry.Text;
            string m_dubbing = DubbingEntry.Text;
            
            string m_data = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            int m_lastWatchedSeries=0;
            int m_lastWatchedSeason=0;
            //GetTrailer(m_title);
            string url = "https://www.youtube.com/results?search_query=" + m_title + " �������";
            string extractedLink;
            string m_linkTrailer="";
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

                        string pattern = "\"videoId\":\"(.*?)\"";

                        Match match = Regex.Match(htmlDocument.DocumentNode.OuterHtml, pattern);

                        if (match.Success)
                        {
                            extractedLink = match.Groups[1].Value;
                            extractedLink = " https://www.youtube.com/watch?v=" + extractedLink;
                            m_linkTrailer = extractedLink;
                          
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(LastWatchedSeriesEntry.Text))
            {
                m_lastWatchedSeries = int.Parse(LastWatchedSeriesEntry.Text);
            }
            else
            {
                
            }

            if (!string.IsNullOrEmpty(LastWatchedSeasonEntry.Text))
            {
                m_lastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);
            }
            else
            {
                
            }
         
            // ������� ����� ��������� ��������
            var newContent = new Content
            {

                Title = m_title,
                Type = m_type,
                Dubbing = m_dubbing,
                LastWatchedSeries = m_lastWatchedSeries,
                LastWatchedSeason = m_lastWatchedSeason,
                NextEpisodeReleaseDate = formattedDate,
                WatchStatus = m_status,
                Link = link,
                LinkTrailer = m_linkTrailer,
                DateAdded = m_data,
                SeriesChangeDate = ""
            };

            // ��������� ����� ������� � ���� ������
            _databaseService.InsertContent(newContent);
            TypePicker.SelectedItem = null;
            WatchStatusPicker.SelectedItem = null;
            NextEpisodeReleaseDatePicker = null;
            TitleEntry.Text = null;
            DubbingEntry.Text = null;
            LastWatchedSeriesEntry.Text = null;
            LastWatchedSeasonEntry.Text = null;
            LinkEntry.Text = null;
            await DisplayAlert("�����", "���� ������ ���������", "OK");
            return;
        }

    }
}
