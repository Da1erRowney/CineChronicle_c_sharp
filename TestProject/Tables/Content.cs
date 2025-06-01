using SQLite;
using System.ComponentModel;

namespace CineChronicle.Tables
{
    public class Content : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }                                                 // Айди
        private string countLabel;                                                  // Количество серий??
        private string dateAdded;                                                   // Дата добавления
        private string dateRelease = null;                                          // Строка даты выхода?
        private string description;                                                 // Описание контента
        private string dubbing;                                                     // Озвучка
        private string emailUser = "";                                              // Пользователь 
        private string image;                                                       // Постер
        private int lastWatchedSeason = 0;                                          // Последний просмотренный сезон
        private int lastWatchedSeries = 0;                                          // Последняя просмотренная серия
        private string nextEpisodeReleaseDate;                                      // Дата выхода следующего эпизода
        private string originalTitle;                                               // Оригинальное название
        private string seriesChangeDate;                                            // Дата изменения контента
        private string sourceLink;                                                  // Ссылка
        private string userLink;
        private string title;                                                       // Название
        private string type;                                                        // Тип
        private string watchStatus;                                                 // Статус просмотра
        private string youTubeLink;                                                 // Ссылка на YouTube
        private string youTubeBackground;
        


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    title = char.ToUpper(value[0]) + value.Substring(1);
                }
                else
                {
                    title = string.Empty;
                }
                OnPropertyChanged(nameof(Title));
            }
        }
        public string CountLabel
        {
            get => countLabel;
            set
            {
                countLabel = value;
                OnPropertyChanged(nameof(CountLabel));
            }
        }
        public string DateAdded
        {
            get => dateAdded;
            set
            {
                dateAdded = value;
                OnPropertyChanged(nameof(DateAdded));
            }
        }
        public string DateRelease
        {
            get => dateRelease;
            set
            {
                dateRelease = value;
                OnPropertyChanged(nameof(DateRelease));
            }
        }
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string Dubbing
        {
            get => dubbing;
            set
            {
                dubbing = value;
                OnPropertyChanged(nameof(Dubbing));
            }
        }
        public string EmailUser
        {
            get => emailUser;
            set
            {
                emailUser = value;
                OnPropertyChanged(nameof(EmailUser));
            }
        }
        public string Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }
        public int LastWatchedSeason
        {
            get => lastWatchedSeason;
            set
            {
                lastWatchedSeason = value;
                OnPropertyChanged(nameof(LastWatchedSeason));
            }
        }
        public int LastWatchedSeries
        {
            get => lastWatchedSeries;
            set
            {
                lastWatchedSeries = value;
                OnPropertyChanged(nameof(LastWatchedSeries));
            }
        }
        public string NextEpisodeReleaseDate
        {
            get => nextEpisodeReleaseDate;
            set
            {
                nextEpisodeReleaseDate = value;
                OnPropertyChanged(nameof(NextEpisodeReleaseDate));
            }
        }
        public string OriginalTitle
        {
            get => originalTitle;
            set
            {
                originalTitle = value;
                OnPropertyChanged(nameof(OriginalTitle));
            }
        }
        public string SeriesChangeDate
        {
            get => seriesChangeDate;
            set
            {
                seriesChangeDate = value;
                OnPropertyChanged(nameof(SeriesChangeDate));
            }
        }
        public string SourceLink
        {
            get => sourceLink;
            set
            {
                sourceLink = value;
                OnPropertyChanged(nameof(SourceLink));
            }
        }
        public string UserLink
        {
            get => userLink;
            set
            {
                userLink = value;
                OnPropertyChanged(nameof(UserLink));
            }
        }
        public string Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        public string WatchStatus
        {
            get => watchStatus;
            set
            {
                watchStatus = value;
                OnPropertyChanged(nameof(WatchStatus));
            }
        }
        public string YouTubeBackground
        {
            get => youTubeBackground;
            set
            {
                youTubeBackground = value;
                OnPropertyChanged(nameof(YouTubeBackground));
            }
        }
        public string YouTubeLink
        {
            get => youTubeLink;
            set
            {
                youTubeLink = value;
                OnPropertyChanged(nameof(YouTubeLink));
            }
        }
     
       
    }
}
