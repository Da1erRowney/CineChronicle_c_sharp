using CineChronicle.Application.MainPage;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CineChronicle.Application.ViewModels
{
    public class ViewContentMainPageModel : ObservableObject
    {
        #region [Private Fields]
        private GetContentRecommendation ContentRecommendationRead = new();
        private static DatabaseServiceContent _databaseService;
        private DeviceInfo _device = new();

        public ObservableCollection<ContentRecommendation> ContentRecommendation { get; set; }
        public ObservableCollection<Content> ContentAdded { get; set; }
        public ObservableCollection<Content> ContentChange { get; set; }
        public ObservableCollection<Content> ContentRelease { get; set; }

        public bool IsContentAddedVisible => ContentAdded?.Count > 0;
        public bool IsContentChangeVisible => ContentChange?.Count > 0;
        public bool IsContentReleaseVisible => ContentRelease?.Count > 0;
        public bool IsContentRecommendationVisible => ContentRecommendation?.Count > 0;

        public static bool[] isContentNull = new bool[3];
        private static int[] usersContent;
        #endregion

        #region [Ctor's]
        public ViewContentMainPageModel()
        {
            usersContent = DeviceInfo.GetContentUser();
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);
            Task.Run(InitializeAllDataAsync);
        }
        #endregion

        #region [Async Init]
        private async Task InitializeAllDataAsync()
        {
            await InitializeAsyncAdded();
            await InitializeAsyncChange();
            await InitializeAsyncRelease();
            await InitializeAsyncRecom();
        }

        public async Task InitializeAsyncRecom()
        {
            await CheckInternetConnectionAsync();
        }

        public async Task InitializeAsyncChange()
        {
            // Недавно измененный
            ContentChange = new ObservableCollection<Content>(_databaseService.GetAllContent(usersContent)
                .Where(c => c.SeriesChangeDate != "")
                .Take(8)
                .ToList());

            OnPropertyChanged(nameof(ContentChange));
            OnPropertyChanged(nameof(IsContentChangeVisible));

            isContentNull[1] = ContentRelease?.Count == 0;
        }

        public async Task InitializeAsyncRelease()
        {
            // Лучше использовать отдельный метод в DatabaseService
            ContentRelease = new ObservableCollection<Content>(_databaseService.GetAllContent(usersContent).
                Where(c => c.DateRelease != null && c.DateRelease != string.Empty)
                .Take(8)
                .ToList());

            OnPropertyChanged(nameof(ContentRelease));
            OnPropertyChanged(nameof(IsContentReleaseVisible));
            isContentNull[2] = ContentRelease?.Count == 0;
        }

        private async Task InitializeAsyncAdded()
        {
            // Недавно добавленный контент
            ContentAdded = new ObservableCollection<Content>(_databaseService.GetAllContent(usersContent)
                .OrderByDescending(c => c.DateAdded)
                .Take(8)
                .ToList());

            isContentNull[0] = ContentRelease?.Count == 0;

            //if (ContentAdded == null || !ContentAdded.Any())
            //{
            //    Content ifContentNull = new Content
            //    {
            //        Title = "Нажмите, чтобы добавить контент",
            //        Type = "Ваш контент",
            //        Image = "pluscontent.png"
            //    };
            //    _databaseService.InsertContent(ifContentNull);

            //    // Недавно добавленный контента
            //    ContentAdded = new ObservableCollection<Content>(_databaseService.GetAllContent(usersContent)
            //        .OrderByDescending(c => c.DateAdded)
            //        .Take(8)
            //        .ToList());
            //}
            OnPropertyChanged(nameof(ContentAdded));
            OnPropertyChanged(nameof(IsContentAddedVisible));
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
                ContentRecommendation = new ObservableCollection<ContentRecommendation>(_databaseService.GetAllRecomContent());
                
            }
            else
            {
                ContentRecommendation = new ObservableCollection<ContentRecommendation>(await ContentRecommendationRead.GetRecommendationsAsync());
            }
            OnPropertyChanged(nameof(ContentRecommendation));
            OnPropertyChanged(nameof(IsContentRecommendationVisible));
        }
        #endregion

        #region [Query]
        public static void DeleteBaseContent()
        {
           _databaseService.DeleteContent( _databaseService.GetContentByTitle("Нажмите, чтобы добавить контент", usersContent)[0]);
        }

        public static Content GetContentById(int id)
        {
           return _databaseService.GetContentById(id);
        }

        public static List<Content> FindSameContent(string title)
        {
            title = title.TrimEnd();
            return _databaseService.GetContentByTitle(title, usersContent);
        }
        #endregion
    }
}
