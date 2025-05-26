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
        #endregion

        #region [Ctor's]
        public ViewContentAllPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);

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
            await Task.Delay(200);
            ContentAll = new ObservableCollection<Content>(_databaseService.GetAllContent().Take(9).ToList());
            ContentAllall = _databaseService.GetContentCount();
            OnPropertyChanged(nameof(IsAllVisible));
            OnPropertyChanged(nameof(ContentAll));
            OnPropertyChanged(nameof(ContentAllall));
        }
        public async Task InitializeAsyncSerial()
        {
            await Task.Delay(200);
            ContentSerial = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.SERIAL).Take(9).ToList());
            ContentSerialall = _databaseService.GetContentCountByType(ContentTypes.SERIAL);
            OnPropertyChanged(nameof(IsSerialsVisible));
            OnPropertyChanged(nameof(ContentSerial));
            OnPropertyChanged(nameof(ContentSerialall));
        }
        public async Task InitializeAsyncAnime()
        {
            await Task.Delay(200);
            ContentAnime = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.ANIME).Take(9).ToList());
            ContentAnimeall = _databaseService.GetContentCountByType(ContentTypes.ANIME);
            OnPropertyChanged(nameof(IsAnimeVisible));
            OnPropertyChanged(nameof(ContentAnime));
            OnPropertyChanged(nameof(ContentAnimeall));
        }
        public async Task InitializeAsyncFilm()
        {
            await Task.Delay(200);
            ContentFilm = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.FILM).Take(9).ToList());
            ContentFilmall = _databaseService.GetContentCountByType(ContentTypes.FILM);
            OnPropertyChanged(nameof(IsFilmVisible));
            OnPropertyChanged(nameof(ContentFilm));
            OnPropertyChanged(nameof(ContentFilmall));
        }
        public async Task InitializeAsyncDorama()
        {
            await Task.Delay(200);
            ContentDorama = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.DORAMA).Take(9).ToList());
            ContentDoramaall = _databaseService.GetContentCountByType(ContentTypes.DORAMA);
            OnPropertyChanged(nameof(IsDoramaVisible));
            OnPropertyChanged(nameof(ContentDorama));
            OnPropertyChanged(nameof(ContentDoramaall));
        }
        public async Task InitializeAsyncMult()
        {
            await Task.Delay(200);
            ContentMult = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.CARTOON).Take(9).ToList());
            ContentMultall = _databaseService.GetContentCountByType(ContentTypes.CARTOON);
            OnPropertyChanged(nameof(IsMultVisible));
            OnPropertyChanged(nameof(ContentMult));
            OnPropertyChanged(nameof(ContentMultall));
        }
        public async Task InitializeAsyncDocum()
        {
            await Task.Delay(200);
            ContentDocum = new ObservableCollection<Content>(_databaseService.GetContentByType("Документалка").Take(9).ToList());
            ContentDocumall = _databaseService.GetContentCountByType("Документалка");
            OnPropertyChanged(nameof(IsDocumVisible));
            OnPropertyChanged(nameof(ContentDocum));
            OnPropertyChanged(nameof(ContentDocumall));
        }
        public async Task InitializeAsyncOther()
        {
            await Task.Delay(200);
            ContentOther = new ObservableCollection<Content>(_databaseService.GetContentByType(ContentTypes.OTHER).Take(9).ToList());
            ContentOtherall = _databaseService.GetContentCountByType(ContentTypes.OTHER);
            OnPropertyChanged(nameof(IsOtherVisible));
            OnPropertyChanged(nameof(ContentOther));
            OnPropertyChanged(nameof(ContentOtherall));
        }
        public async Task InitializeAsyncViewed()
        {
            await Task.Delay(200);
            ContentViewed = new ObservableCollection<Content>(_databaseService.GetContentByWatchStatus("Просмотрено").Take(9).ToList());
            ContentViewedall = _databaseService.GetContentCountByWatchStatus("Просмотрено");
            OnPropertyChanged(nameof(IsViewedVisible));
            OnPropertyChanged(nameof(ContentViewed));
            OnPropertyChanged(nameof(ContentViewedall));
        }
        public async Task InitializeAsyncProcess()
        {
            await Task.Delay(200);
            ContentProcess = new ObservableCollection<Content>(_databaseService.GetContentByWatchStatus("Смотрю").Take(9).ToList());
            ContentProcessall = _databaseService.GetContentCountByWatchStatus("Смотрю");
            OnPropertyChanged(nameof(IsProcessVisible));
            OnPropertyChanged(nameof(ContentProcess));
            OnPropertyChanged(nameof(ContentProcessall));
        }
        public async Task InitializeAsyncNotStart()
        {
            await Task.Delay(200);
            ContentNotStart = new ObservableCollection<Content>(_databaseService.GetContentByWatchStatus("Не начинал").Take(9).ToList());
            ContentNotStartall = _databaseService.GetContentCountByWatchStatus("Не начинал");
            OnPropertyChanged(nameof(IsNotStartVisible));
            OnPropertyChanged(nameof(ContentNotStart));
            OnPropertyChanged(nameof(ContentNotStartall));
        }
        #endregion

        #region [Query]
        public static Content GetContentById(int id)
        {
            return _databaseService.GetContentById(id);
        }

        public static void DeleteBaseContent()
        {
            _databaseService.DeleteContent(_databaseService.GetContentByTitle("Нажмите, чтобы добавить контент")[0]);
        }
        public void UpdateContentsByQuery(string searchQuery)
        {
            ContentSort = new ObservableCollection<Content>(_databaseService.GetAllContent().Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList());
            OnPropertyChanged(nameof(ContentSort));
        }
        private static List<Content> GetContentStatus(string type)
        {
            return _databaseService.GetContentByWatchStatus(type).ToList();
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
                return _databaseService.GetAllContent().ToList();
            }
            else
            {
                return _databaseService.GetContentByType(type).ToList();
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
