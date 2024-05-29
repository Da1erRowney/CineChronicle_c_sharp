using System;
using System.Collections.Generic;
using SQLite;
using DataContent;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Input;


namespace TestProject { 

    public partial class AddMoreContentPage : ContentPage { 

        private DatabaseServiceContent _databaseService;

        public AddMoreContentPage()
        {
            InitializeComponent();


            
            _databaseService = new DatabaseServiceContent(MainPage._databasePath);
            LastWatchedSeriesEntry.TextChanged += LastWatchedSeriesEntry_TextChanged;
            LastWatchedSeasonEntry.TextChanged += LastWatchedSeasonEntry_TextChanged;

        }
        public AddMoreContentPage(ContentRecommendation data)
        {
            InitializeComponent();
            TitleEntry.Text = data.Title;
            if (data.Type == "������")
            {
                // ����� ������ �������� "������" � ������ ��������� TypePicker
                var selectedType = TypePicker.ItemsSource.Cast<string>().FirstOrDefault(item => item == "������");

                // ��������� ��������� ������ �������� � SelectedItem
                TypePicker.SelectedItem = selectedType;
            }
            else if (data.Type == "�����")
            {
                // ����� ������ �������� "������" � ������ ��������� TypePicker
                var selectedType = TypePicker.ItemsSource.Cast<string>().FirstOrDefault(item => item == "�����");

                // ��������� ��������� ������ �������� � SelectedItem
                TypePicker.SelectedItem = selectedType;
            }
            else if (data.Type == "�����������")
            {
                // ����� ������ �������� "������" � ������ ��������� TypePicker
                var selectedType = TypePicker.ItemsSource.Cast<string>().FirstOrDefault(item => item == "�����������");

                // ��������� ��������� ������ �������� � SelectedItem
                TypePicker.SelectedItem = selectedType;
            }
            
         


            
            _databaseService = new DatabaseServiceContent(MainPage._databasePath);
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
           
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);

            // ������������ ��������� ���� � ������ � ������ ��������
           

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
            List<Content> contents = _databaseService.GetAllContent().ToList();

            List<Content> filteredContents = contents.Where(c => c.Title.IndexOf(m_title, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if( filteredContents.Count !=0 )
            {
                bool result = await DisplayAlert("�����������", $"������ {m_title} ��� ����������, �� �������, ��� ������ ������� �����?", "��", "���");

                if (!result)
                {
                    TitleEntry.Text = "";
                    return;
                }
            }

            // ������� ����� ��������� ��������
            var newContent = new Content
            {

                Title = m_title,
                Type = m_type,
                Dubbing = m_dubbing,
                LastWatchedSeries = m_lastWatchedSeries,
                LastWatchedSeason = m_lastWatchedSeason,
                NextEpisodeReleaseDate = "� ���������� ��������� ����������...",
                WatchStatus = m_status,
                Link = link,
                DateAdded = m_data,
                SeriesChangeDate = "",
                Image = "",
                SmallDecription = ""
            };
            _databaseService.InsertContent(newContent);
            ViewContentPage viewContentPage = new ViewContentPage(newContent);
            switch (m_type)
            {
                case "�����":
                    viewContentPage.GetAnemeGoInfo(m_title);
                    viewContentPage.GetAnimeGoImage(m_title);
                    viewContentPage.DataExitNextEpisod(m_title);
                    break;
                case "�����":
                    viewContentPage.GetWikipediaInfo(m_title);
                    viewContentPage.GetWikipediaImage(m_title);
                    viewContentPage.DataExitNextEpisod(m_title);
                    break;
                case "������":
                    viewContentPage.GetWikipediaInfo(m_title);
                   // viewContentPage.GetWikipediaImage(m_title);
                    viewContentPage.DataExitNextEpisod(m_title);
                    break;
                case "������":
                    viewContentPage.GetWikipediaInfo(m_title);
                  
                    viewContentPage.DataExitNextEpisod(m_title);
                    break;
                case "�����������":
                    viewContentPage.GetWikipediaInfo(m_title);
                   
                    viewContentPage.DataExitNextEpisod(m_title);
                    break;
                case "������":
                    viewContentPage.GetWikipediaInfo(m_title);
                    viewContentPage.GetWikipediaImage(m_title);
                    viewContentPage.DataExitNextEpisod(m_title);
                    break;

            }
            TitleEntry.Text ="";
            DubbingEntry.Text ="";
            LastWatchedSeriesEntry.Text ="";
            LastWatchedSeasonEntry.Text ="";
            LinkEntry.Text ="";

            await DisplayAlert("�����", "���� ������ ���������", "OK");
            await Navigation.PushAsync(new MainPage());
        }

    }
}
