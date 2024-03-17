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
            // Проверяем каждый введенный символ в поле
            foreach (char c in e.NewTextValue)
            {
                // Если символ не является цифрой, удаляем его из текста
                if (!char.IsDigit(c))
                {
                    LastWatchedSeriesEntry.Text = LastWatchedSeriesEntry.Text.Remove(LastWatchedSeriesEntry.Text.Length - 1);
                    break;
                }
            }
        }

        private void LastWatchedSeasonEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем каждый введенный символ в поле
            foreach (char c in e.NewTextValue)
            {
                // Если символ не является цифрой, удаляем его из текста
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
                await DisplayAlert("Ошибка", "Заполните Название, Тип и Статус просмотра для возможности сохранения", "OK");
                return;
            }

            // Получите выбранную дату из DatePicker
            DateTime selectedDate = NextEpisodeReleaseDatePicker.Date;
            DateTime currentDate = DateTime.UtcNow;
            DateTime newDate = currentDate.AddHours(+3);

            // Преобразуйте выбранную дату в строку с нужным форматом
            string formattedDate = selectedDate.ToString("dd.MM.yyyy"); // Например, "01.02.2024"

            // Формируем ссылку в зависимости от выбранного типа контента и заполняем поле LinkEntry
            string link = "";
            if (string.IsNullOrEmpty(LinkEntry.Text))
            {
                switch (m_type)
                {
                    case "Аниме":
                        link = "https://animego.org/search/all?q=" + TitleEntry.Text;
                        break;
                    case "Дорама":
                        link = "https://dorama.land/search?q=" + TitleEntry.Text;
                        break;
                    case "Сериал":
                        link = "https://kinogo.biz/search/" + TitleEntry.Text;
                        break;
                    case "Мультсериал":
                        link = "https://kinogo.biz/search/" + TitleEntry.Text;
                        break;
                    case "Фильм":
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
         
            // Создаем новый экземпляр контента
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
                DateAdded = m_data,
                SeriesChangeDate = ""
            };

            // Вставляем новый контент в базу данных
            _databaseService.InsertContent(newContent);
            TitleEntry.Text ="";
            DubbingEntry.Text ="";
            LastWatchedSeriesEntry.Text ="";
            LastWatchedSeasonEntry.Text ="";
            LinkEntry.Text ="";
            await DisplayAlert("Успех", "Ваши данные сохранены", "OK");

        }

    }
}
