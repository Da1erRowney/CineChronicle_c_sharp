using System;
using System.Collections.Generic;
using SQLite;
using DataContent;

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
        
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TitleEntry.Text) || string.IsNullOrEmpty(TypePicker.SelectedItem.ToString()) || string.IsNullOrEmpty(WatchStatusPicker.SelectedItem.ToString()))
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

            if (DubbingEntry.Text==null)
            {
                DubbingEntry.Text = "";
            }
            if (LastWatchedSeriesEntry.Text == null)
            {
                LastWatchedSeriesEntry.Text = "0";
            }
            if (LastWatchedSeasonEntry.Text == null)
            {
                LastWatchedSeasonEntry.Text = "0";
            }
            if (LinkEntry.Text == null)
            {
                LinkEntry.Text = "";
            }
            var newContent = new Content
            {
                Title = TitleEntry.Text,
                Type = TypePicker.SelectedItem.ToString(),
                Dubbing = DubbingEntry.Text,
                LastWatchedSeries = int.Parse(LastWatchedSeriesEntry.Text),
                LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text),
                NextEpisodeReleaseDate = formattedDate,
                WatchStatus = WatchStatusPicker.SelectedItem.ToString(),
                Link = LinkEntry.Text,
                DateAdded = newDate.ToString("yyyy-MM-dd HH:mm:ss"),
                SeriesChangeDate = ""

            };

            _databaseService.InsertContent(newContent);

            Navigation.PopAsync();
        }
    }
}
