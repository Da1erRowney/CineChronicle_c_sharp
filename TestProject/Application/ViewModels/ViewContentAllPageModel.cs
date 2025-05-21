using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineChronicle.Application.ViewModels
{
    public class ViewContentAllPageModel : ObservableObject
    {
        #region [Private Fields]
        private static DatabaseServiceContent _databaseService;
        public List<Content> ContentAll { get; set; }
        public List<Content> ContentSerial { get; set; }
        public List<Content> ContentAnime { get; set; }
        public List<Content> ContentFilm { get; set; }
        public List<Content> ContentDorama { get; set; }
        public List<Content> ContentMult { get; set; }
        public List<Content> ContentDocum { get; set; }
        public List<Content> ContentOther { get; set; }
        public List<Content> ContentViewed { get; set; }
        public List<Content> ContentProcess { get; set; }
        public List<Content> ContentNotStart { get; set; }

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

        public ViewContentAllPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);

            InitializeSyncData();
        }

        private void InitializeSyncData()
        {
            // Сам контент
            ContentAll = _databaseService.GetAllContent().Take(8).ToList();
            ContentAllall = _databaseService.GetContentCount();
            OnPropertyChanged(nameof(IsAllVisible));

            ContentSerial = _databaseService.GetContentByType(ContentTypes.SERIAL).Take(8).ToList();
            ContentSerialall = _databaseService.GetContentCountByType(ContentTypes.SERIAL);
            OnPropertyChanged(nameof(IsSerialsVisible));

            ContentAnime = _databaseService.GetContentByType(ContentTypes.ANIME).Take(8).ToList();
            ContentAnimeall = _databaseService.GetContentCountByType(ContentTypes.ANIME);
            OnPropertyChanged(nameof(IsAnimeVisible));

            ContentFilm = _databaseService.GetContentByType(ContentTypes.FILM).Take(8).ToList();
            ContentFilmall = _databaseService.GetContentCountByType(ContentTypes.FILM);
            OnPropertyChanged(nameof(IsFilmVisible));

            ContentDorama = _databaseService.GetContentByType(ContentTypes.DORAMA).Take(8).ToList();
            ContentDoramaall = _databaseService.GetContentCountByType(ContentTypes.DORAMA);
            OnPropertyChanged(nameof(IsDoramaVisible));

            ContentMult = _databaseService.GetContentByType(ContentTypes.CARTOON).Take(8).ToList();
            ContentMultall = _databaseService.GetContentCountByType(ContentTypes.CARTOON);
            OnPropertyChanged(nameof(IsMultVisible));

            ContentDocum = _databaseService.GetContentByType("Документалка").Take(8).ToList();
            ContentDocumall = _databaseService.GetContentCountByType("Документалка");
            OnPropertyChanged(nameof(IsDocumVisible));

            ContentOther = _databaseService.GetContentByType(ContentTypes.OTHER).Take(8).ToList();
            ContentOtherall = _databaseService.GetContentCountByType(ContentTypes.OTHER);
            OnPropertyChanged(nameof(IsOtherVisible));


            ContentViewed = _databaseService.GetContentByWatchStatus("Просмотрено").Take(8).ToList();
            ContentViewedall = _databaseService.GetContentCountByWatchStatus("Просмотрено");
            OnPropertyChanged(nameof(IsViewedVisible));

            ContentProcess = _databaseService.GetContentByWatchStatus("Смотрю").Take(8).ToList();
            ContentProcessall = _databaseService.GetContentCountByWatchStatus("Смотрю");
            OnPropertyChanged(nameof(IsProcessVisible));

            ContentNotStart = _databaseService.GetContentByWatchStatus("Не начинал").Take(8).ToList();
            ContentNotStartall = _databaseService.GetContentCountByWatchStatus("Не начинал");
            OnPropertyChanged(nameof(IsNotStartVisible));  
        }


        public static Content GetContentById(int id)
        {
            return _databaseService.GetContentById(id);
        }
        public static void DeleteBaseContent()
        {
            _databaseService.DeleteContent(_databaseService.GetContentByTitle("Нажмите, чтобы добавить контент")[0]);
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
        public static List<Content> GetContentsByQuery(string searchQuery)
        {
            return _databaseService.GetAllContent().Where(c => c.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
        private static List<Content> GetContentStatus(string type)
        {
            return _databaseService.GetContentByWatchStatus(type).ToList();
        }
    }
}
