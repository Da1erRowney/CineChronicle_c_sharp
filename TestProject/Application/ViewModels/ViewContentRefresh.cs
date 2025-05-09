using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineChronicle.Application.ViewModel
{
    public class YourViewModel : ObservableObject
    {
        #region [Content]
        private string countLabel;                                                  // Количество серий??
        private string dateAdded;                                                   // Дата добавления
        private string description;                                                 // Описание контента
        private string dubbing;                                                     // Озвучка
        private string dateRelease;                                                 // Строка даты выхода?
        private string image;                                                       // Постер
        private int lastWatchedSeason = 0;                                          // Последний просмотренный сезон
        private int lastWatchedSeries = 0;                                          // Последняя просмотренная серия
        private string nextEpisodeReleaseDate;                                      // Дата выхода следующего эпизода
        private string seriesChangeDate;                                            // Дата изменения контента
        private string sourceLink;                                                  // Ссылка
        private string title;                                                       // Название
        private string type;                                                        // Тип
        private string watchStatus;                                                 // Статус просмотра
        private string youTubeLink;                                                 // Ссылка на YouTube
        #endregion

        public Content Content { get; }

        public YourViewModel(Content content)
        {
            Content = content;
        }
    }
}
