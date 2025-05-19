using CineChronicle.Application.MainPage;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineChronicle.Application.ViewModels
{
    public class ViewContentMainPageModel : ObservableObject
    {
        
        public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content.db");
        private ContentRecommendation ContentRecommendationRead = new();
        private static DatabaseServiceContent _databaseService;
        public List<ContentRecommendation> ContentRecommendation { get; set; }
        public List<Content> ContentAdded { get; set; }
        public List<Content> ContentChange { get; set; }
        public List<Content> ContentRelease { get; set; }
        private DeviceInfo _device = new();

        public ViewContentMainPageModel()
        {
            _databaseService = new DatabaseServiceContent(_databasePath);

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
                    for (int i = 0; i < 5; i++)
                    {
                        if (string.IsNullOrEmpty(ContentAdded[i].Image))
                        {
                            ContentAdded[i].Image = "notwificonnection.jpg";
                        }
                    }
                }
                if (ContentChange != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (string.IsNullOrEmpty(ContentChange[i].Image))
                        {
                            ContentChange[i].Image = "notwificonnection.jpg";
                        }
                    }
                }
                if (ContentRelease != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (string.IsNullOrEmpty(ContentRelease[i].Image))
                        {
                            ContentRelease[i].Image = "notwificonnection.jpg";
                        }
                    }
                }
            }
            else
            {
                if (ContentRecommendation == null)
                {
                    ContentRecommendation = await ContentRecommendationRead.GetRecommendationsAsync();
                    OnPropertyChanged(nameof(ContentRecommendation));
                }
            }
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
