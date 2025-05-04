using SQLite;

namespace CineChronicle.Tables
{
    public class Content
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }                                                 // Айди
        public string CountLabel { get; set; } = string.Empty;                      // Количество серий??
        public string DateAdded { get; set; } = string.Empty;                       // Дата добавления
        public string Description { get; set; } = string.Empty;                     // Описание контента
        public string Dubbing { get; set; } = string.Empty;                         // Озвучка
        public string DateRelease { get; set; } = string.Empty;                     // Строка даты выхода?
        public string Image { get; set; } = string.Empty;                           // Постер
        public int LastWatchedSeason { get; set; } = 0;                             // Последний просмотренный сезон
        public int LastWatchedSeries { get; set; } = 0;                             // Последняя просмотренная серия
        public string NextEpisodeReleaseDate { get; set; } = string.Empty;          // Дата выхода следующего эпизода
        public string SeriesChangeDate { get; set; } = string.Empty;                // Дата изменения контента
        public string SourceLink { get; set; } = string.Empty;                      // Ссылка
        [NotNull]
        public string Title { get; set; }                                           // Название
        [NotNull]
        public string Type { get; set; }                                            // Тип
        public string WatchStatus { get; set; } = string.Empty;                     // Статус просмотра
        public string YouTubeLink { get; set; } = string.Empty;                     // Ссылка на YouTube
    }
}
