using CineChronicle.Application.MainPage;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace CineChronicle.Application.ViewModels
{
    public class ViewContentMainPageModel : ObservableObject
    {
        #region [Private Fields]
        private GetContentRecommendation ContentRecommendationRead = new();
        private DeviceInfo _device = new();

        private static DatabaseServiceContent _databaseService;

        public static bool[] isContentNull = new bool[3];

        public List<ContentRecommendation> ContentRecommendation { get; set; }
        public List<Content> ContentAdded { get; set; }
        public List<Content> ContentChange { get; set; }
        public List<Content> ContentRelease { get; set; }
        #endregion

        public ViewContentMainPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);

            InitializeSyncData();
        }

        public async Task InitializeAsyncRecom()
        {
            await Task.Delay(1500); // Даёт время на первоначальный рендеринг
            await CheckInternetConnectionAsync();
        }

        public async Task InitializeAsyncChange()
        {
            await Task.Delay(1500); // Даёт время на первоначальный рендеринг

            // Недавно измененный
            ContentChange = _databaseService.GetAllContent()
                .OrderByDescending(c => c.SeriesChangeDate)
                .Take(8)
                .ToList();

            OnPropertyChanged(nameof(ContentChange));

            isContentNull[1] = ContentRelease?.Count == 0;
        }

        public async Task InitializeAsyncRelease()
        {
            await Task.Delay(1500);

            // Лучше использовать отдельный метод в DatabaseService
            ContentRelease = _databaseService.GetAllContent()
                .Take(8)
                .ToList();

            OnPropertyChanged(nameof(ContentRelease));
            isContentNull[2] = ContentRelease?.Count == 0;
        }

        private void InitializeSyncData()
        {
            // Недавно добавленный контент
            ContentAdded = _databaseService.GetAllContent()
                .OrderByDescending(c => c.DateAdded)
                .Take(8)
                .ToList();

            isContentNull[0] = ContentRelease?.Count == 0;

            if (ContentAdded == null || !ContentAdded.Any())
            {
                Content ifContentNull = new Content
                {
                    Title = "Нажмите, чтобы добавить контент",
                    Type = "Ваш контент",
                    Image = "plus.png"
                };
                _databaseService.InsertContent(ifContentNull);

                // Недавно добавленный контента
                ContentAdded = _databaseService.GetAllContent()
                    .OrderByDescending(c => c.DateAdded)
                    .Take(8)
                    .ToList();
            }
        }

        private async Task CheckInternetConnectionAsync()
        {
            if (!_device.CheckInternetConnection())
            {
                if (ContentAdded != null)
                {
                    for (int i = 0; i < ContentAdded.Count; i++)
                    {
                        if (string.IsNullOrEmpty(ContentAdded[i]!.Image))
                        {
                            ContentAdded[i]!.Image = "notwificonnection.jpg";
                        }
                    }
                }
                if (ContentChange != null)
                {
                    for (int i = 0; i < ContentChange.Count; i++)
                    {
                        if (string.IsNullOrEmpty(ContentChange[i]!.Image))
                        {
                            ContentChange[i]!.Image = "notwificonnection.jpg";
                        }
                    }
                }
                if (ContentRelease != null)
                {
                    for (int i = 0; i < ContentRelease.Count; i++)
                    {
                        if (string.IsNullOrEmpty(ContentRelease[i]!.Image))
                        {
                            ContentRelease[i]!.Image = "notwificonnection.jpg";
                        }
                    }
                }
                ContentRecommendation = _databaseService.GetAllRecomContent();
                
            }
            else
            {
                ContentRecommendation = await ContentRecommendationRead.GetRecommendationsAsync();
            }
            OnPropertyChanged(nameof(ContentRecommendation));
        }

        public static void DeleteBaseContent()
        {
           _databaseService.DeleteContent( _databaseService.GetContentByTitle("Нажмите, чтобы добавить контент")[0]);
        }

        public static Content GetContentById(int id)
        {
           return _databaseService.GetContentById(id);
        }

        public static List<Content> FindSameContent(string title)
        {
            title = title.TrimEnd();
            return _databaseService.GetContentByTitle(title);
        }
    }
}
