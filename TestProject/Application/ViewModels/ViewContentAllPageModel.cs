using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CineChronicle.Application.ViewModels
{
    public class ViewContentAllPageModel : ObservableObject
    {
        #region [Private Fields]
        private static DatabaseServiceContent _databaseService;

        public ObservableCollection<Content> ContentSort { get; set; }
        public ObservableCollection<Content> ContentAll { get; set; }
        public ObservableCollection<Content> ContentSerial { get; set; }
        public ObservableCollection<Content> ContentAnime { get; set; }
        public ObservableCollection<Content> ContentFilm { get; set; }
        public ObservableCollection<Content> ContentDorama { get; set; }
        public ObservableCollection<Content> ContentMult { get; set; }
        public ObservableCollection<Content> ContentDocum { get; set; }
        public ObservableCollection<Content> ContentOther { get; set; }
        public ObservableCollection<Content> ContentViewed { get; set; }
        public ObservableCollection<Content> ContentProcess { get; set; }
        public ObservableCollection<Content> ContentNotStart { get; set; }

        // Новые свойства для видимости
        public bool IsAllVisible => ContentAll?.Count > 0;
        public bool IsSerialsVisible => ContentSerial?.Count > 0;
        public bool IsAnimeVisible => ContentAnime?.Count > 0;
        public bool IsFilmVisible => ContentFilm?.Count > 0;
        public bool IsDoramaVisible => ContentDorama?.Count > 0;
        public bool IsMultVisible => ContentMult?.Count > 0;
        public bool IsDocumVisible => ContentDocum?.Count > 0;
        public bool IsOtherVisible => ContentOther?.Count > 0;
        public bool IsViewedVisible => ContentViewed?.Count > 0;
        public bool IsProcessVisible => ContentProcess?.Count > 0;
        public bool IsNotStartVisible => ContentNotStart?.Count > 0;

        public int ContentAllall { get; set; }
        public int ContentSerialall { get; set; }
        public int ContentAnimeall { get; set; }
        public int ContentFilmall { get; set; }
        public int ContentDoramaall { get; set; }
        public int ContentMultall { get; set; }
        public int ContentDocumall { get; set; }
        public int ContentOtherall { get; set; }
        public int ContentViewedall { get; set; }
        public int ContentProcessall { get; set; }
        public int ContentNotStartall { get; set; }

        public static string[] _categoryData = new string[2];
        private static int[] usersContent;
        #endregion

        #region [Ctor's]
        public ViewContentAllPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);
            usersContent = DeviceInfo.GetContentUser();
            Task.Run(InitializeAllDataAsync);
        }
        #endregion

        #region [Async Init]
        private async Task InitializeAllDataAsync()
        {
            await InitializeAsyncAll();
            await InitializeAsyncSerial();
            await InitializeAsyncAnime();
            await InitializeAsyncFilm();
            await InitializeAsyncDorama();
            await InitializeAsyncMult();
            await InitializeAsyncDocum();
            await InitializeAsyncOther();
            await InitializeAsyncViewed();
            await InitializeAsyncProcess();
            await InitializeAsyncNotStart();
        }
        public async Task InitializeAsyncAll()
        {
            await Task.Delay(100);
            ContentAllall = _databaseService.GetContentCount(usersContent);
            if (ContentAllall != 0)
            {
                ContentAll = new ObservableCollection<Content>(_databaseService.GetAllContent(usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentAll));
                OnPropertyChanged(nameof(ContentAllall));
            }
            OnPropertyChanged(nameof(IsAllVisible));
        }
        public async Task InitializeAsyncSerial()
        {
            await Task.Delay(100);
            ContentSerialall = _databaseService.GetContentCountByType(ContentTypes.SERIAL, usersContent);
            if (ContentSerialall != 0)
            {
                ContentSerial = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.SERIAL, usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentSerial));
                OnPropertyChanged(nameof(ContentSerialall));
            }
            OnPropertyChanged(nameof(IsSerialsVisible));
        }
        public async Task InitializeAsyncAnime()
        {
            await Task.Delay(100);
            ContentAnimeall = _databaseService.GetContentCountByType(ContentTypes.ANIME, usersContent);
            if (ContentAnimeall != 0)
            {
                ContentAnime = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.ANIME, usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentAnime));
                OnPropertyChanged(nameof(ContentAnimeall));
            }
            OnPropertyChanged(nameof(IsAnimeVisible));
        }
        public async Task InitializeAsyncFilm()
        {
            await Task.Delay(100);
            ContentFilmall = _databaseService.GetContentCountByType(ContentTypes.FILM, usersContent);
            if (ContentFilmall != 0)
            {
                ContentFilm = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.FILM, usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentFilm));
                OnPropertyChanged(nameof(ContentFilmall));
            }
            OnPropertyChanged(nameof(IsFilmVisible));
        }
        public async Task InitializeAsyncDorama()
        {
            await Task.Delay(100);
            ContentDoramaall = _databaseService.GetContentCountByType(ContentTypes.DORAMA, usersContent);
            if (ContentDoramaall != 0)
            {
                ContentDorama = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.DORAMA, usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentDorama));
                OnPropertyChanged(nameof(ContentDoramaall));
            }
            OnPropertyChanged(nameof(IsDoramaVisible));
        }
        public async Task InitializeAsyncMult()
        {
            await Task.Delay(100);
            ContentMultall = _databaseService.GetContentCountByType(ContentTypes.CARTOON, usersContent);
            if (ContentMultall != 0)
            {
                ContentMult = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.CARTOON, usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentMult));
                OnPropertyChanged(nameof(ContentMultall));
            }
            OnPropertyChanged(nameof(IsMultVisible));
        }
        public async Task InitializeAsyncDocum()
        {
            await Task.Delay(100);
            ContentDocumall = _databaseService.GetContentCountByType("Документалка", usersContent);
            if (ContentDocumall != 0)
            {
                ContentDocum = new ObservableCollection<Content>(_databaseService.GetContentByType("Документалка", usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentDocum));
                OnPropertyChanged(nameof(ContentDocumall));
            }
            OnPropertyChanged(nameof(IsDocumVisible));
        }
        public async Task InitializeAsyncOther()
        {
            await Task.Delay(100);
            ContentOtherall = _databaseService.GetContentCountByType(ContentTypes.OTHER, usersContent);
            if (ContentOtherall != 0)
            {
                ContentOther = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.OTHER, usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentOther));
                OnPropertyChanged(nameof(ContentOtherall));
            }
            OnPropertyChanged(nameof(IsOtherVisible));
        }
        public async Task InitializeAsyncViewed()
        {
            await Task.Delay(100);
            ContentViewedall = _databaseService.GetContentCountByWatchStatus("Просмотрено", usersContent);
            if (ContentViewedall != 0)
            {
                ContentViewed = new ObservableCollection<Content>(_databaseService.GetContentByWatchStatus("Просмотрено", usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentViewed));
                OnPropertyChanged(nameof(ContentViewedall));
            }
            OnPropertyChanged(nameof(IsViewedVisible));
        }
        public async Task InitializeAsyncProcess()
        {
            await Task.Delay(100);
            ContentProcessall = _databaseService.GetContentCountByWatchStatus("Смотрю", usersContent);
            if (ContentProcessall != 0)
            {
                ContentProcess = new ObservableCollection<Content>(_databaseService.GetContentByWatchStatus("Смотрю", usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentProcess));
                OnPropertyChanged(nameof(ContentProcessall));
            }
            OnPropertyChanged(nameof(IsProcessVisible));
        }
        public async Task InitializeAsyncNotStart()
        {
            await Task.Delay(100);
            ContentNotStartall = _databaseService.GetContentCountByWatchStatus("Не начинал", usersContent);
            if (ContentNotStartall != 0)
            {
                ContentNotStart = new ObservableCollection<Content>(_databaseService.GetContentByWatchStatus("Не начинал", usersContent).Take(9).ToList());
                OnPropertyChanged(nameof(ContentNotStart));
                OnPropertyChanged(nameof(ContentNotStartall));
            }
            OnPropertyChanged(nameof(IsNotStartVisible));
        }
        #endregion

        #region [Query]
        public static Content GetContentById(int id)
        {
            return _databaseService.GetContentById(id);
        }

        public static void DeleteBaseContent()
        {
            _databaseService.DeleteContent(_databaseService.GetContentByTitle("Нажмите, чтобы добавить контент", usersContent)[0]);
        }
        public void UpdateContentsByQuery(string searchQuery)
        {
            ContentSort = new ObservableCollection<Content>(_databaseService.GetAllContent(usersContent).Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
            OnPropertyChanged(nameof(ContentSort));
        }
        private static List<Content> GetContentStatus(string type)
        {
            return _databaseService.GetContentByWatchStatus(type, usersContent).ToList();
        }
        #endregion

        #region [Somebody Methods]
        public static string GetName(object sender)
        {
            string labelName = "";
            if (sender is StackLayout layout) // StackLayout, Grid и т.д.
            {
                foreach (var child in layout.Children)
                {
                    if (child is Label label)
                    {
                        labelName = label.Text; // Возвращаем текст первого найденного Label
                    }
                }
            }
            string nameSelected = labelName;

            _categoryData = GetTypeCategory(nameSelected);
            return _categoryData[1];
        }

        public static List<Content> PrepareCategory(object sender)
        {
            var label = sender as Label;
            string nameSelected = label.Text;

            _categoryData = GetTypeCategory(nameSelected);

            switch (_categoryData[0])
            {
                case "Просмотрено":
                case "Смотрю":
                case "Не начинал":
                    return GetContentStatus(_categoryData[0]);
                default:
                    return GetContent(_categoryData[0]);       // Тип и название категории
            }
        }
        public static string[] GetTypeCategory(string nameSelected)
        {
            if (nameSelected == "Сериалы")
            {
                return [ContentTypes.SERIAL, "Все ваши Сериалы"];
            }
            if (nameSelected == "Аниме")
            {
                return [ContentTypes.ANIME, "Всё ваше Аниме"];
            }
            if (nameSelected == "Фильмы")
            {
                return [ContentTypes.FILM, "Все ваши Фильмы"];
            }
            if (nameSelected == "Дорамы")
            {
                return [ContentTypes.DORAMA, "Все ваши Дорамы"];
            }
            if (nameSelected == "Мультсериалы")
            {
                return [ContentTypes.CARTOON, "Все ваши Мультсериалы"];
            }
            if (nameSelected == "Документалки")
            {
                return ["Документалки", "Все ваши Документальные фильмы"];
            }
            if (nameSelected == "Прочее")
            {
                return [ContentTypes.OTHER, "Ваш прочий контент"];
            }
            if (nameSelected == "Просмотрено")
            {
                return ["Просмотрено", "Просмотренный контент"];
            }
            if (nameSelected == "Вы смотрите")
            {
                return ["Смотрю", "Контент, который вы начали смотреть"];
            }
            if (nameSelected == "Вы еще не начали смотреть")
            {
                return ["Не начинал", "Не начатый контент"];
            }

            return ["Весь", "Весь ваш контент"];
        }

        private static List<Content> GetContent(string type)
        {
            if (type == "Весь")
            {
                return _databaseService.GetAllContent(usersContent).ToList();
            }
            else
            {
                return _databaseService.GetContentByType(type, usersContent).ToList();
            }
        }

        public bool[] GetVisibleProperties()
        {
            return new bool[]
            {
                ContentAll?.Count > 0,
                ContentSerial?.Count > 0,
                ContentAnime?.Count > 0,
                ContentFilm?.Count > 0,
                ContentDorama?.Count > 0,
                ContentMult?.Count > 0,
                ContentDocum?.Count > 0,
                ContentOther?.Count > 0,
                ContentViewed?.Count > 0,
                ContentProcess?.Count > 0,
                ContentNotStart?.Count > 0
            };
        }
        #endregion
    }
}
