using CineChronicle.Application.MainPage;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

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

        public async Task InitializeAsync()
        {
            await CheckInternetConnectionAsync();
        }

        private void InitializeSyncData()
        {
            // Недавно добавленный контента
            ContentAdded = _databaseService.GetAllContent()
                .OrderByDescending(c => c.DateAdded)
                .Take(8)
                .ToList();
            
            // Недавно измененный
            ContentChange = _databaseService.GetAllContent()
                .OrderByDescending(c => c.SeriesChangeDate)
                .Take(8)
                .ToList();

            // Скоро выйдет
            ContentRelease = _databaseService.GetAllContent()
                .Select(c => new {
                    Content = c,
                    ParsedDate = DateTime.TryParse(c.DateRelease, out var date) ? date : (DateTime?)null
                })
                .Where(x => x.ParsedDate != null)
                .OrderBy(x => x.ParsedDate)
                .Select(x => x.Content)
                .Take(8)
                .ToList();

            if (ContentAdded.Count == 0)
            {
                isContentNull[0] = true;
            }
            else
            {
                isContentNull[0] = false;
            }

            if (ContentChange.Count == 0)
            {
                isContentNull[1] = true;
            }
            else
            {
                isContentNull[1] = false;
            }

            if (ContentRelease.Count == 0)
            {
                isContentNull[2] = true;
            }
            else
            {
                isContentNull[2] = false;
            }

            if (ContentAdded == null || !ContentAdded.Any())
            {
                Content ifContentNull = new Content
                {
                    Title = "Нажмите, чтобы добавить контент",
                    Type = "Ваш контент",
                    Image = "plus.png"
                };
                _databaseService.InsertContent(ifContentNull);
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
                OnPropertyChanged(nameof(ContentRecommendation));
            }
            else
            {
                ContentRecommendation = await ContentRecommendationRead.GetRecommendationsAsync();
                OnPropertyChanged(nameof(ContentRecommendation));
            }
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
