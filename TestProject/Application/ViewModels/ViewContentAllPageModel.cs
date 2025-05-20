using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineChronicle.Application.ViewModels
{
    public class ViewContentAllPageModel : ObservableObject
    {
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

        public List<Content> ContentAllall { get; set; }
        public List<Content> ContentSerialall { get; set; }
        public List<Content> ContentAnimeall { get; set; }
        public List<Content> ContentFilmall { get; set; }
        public List<Content> ContentDoramaall { get; set; }
        public List<Content> ContentMultall { get; set; }
        public List<Content> ContentDocumall { get; set; }
        public List<Content> ContentOtherall { get; set; }
        public List<Content> ContentViewedall { get; set; }
        public List<Content> ContentProcessall { get; set; }
        public List<Content> ContentNotStartall { get; set; }

        public static string[] _categoryData = new string[2];

        public ViewContentAllPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);

            InitializeSyncData();
        }

        private void InitializeSyncData()
        {
            ContentAll = _databaseService.GetAllContent().Take(8).ToList();
            ContentAllall = _databaseService.GetAllContent().ToList();

            ContentSerialall = _databaseService.GetContentByType("Сериал").ToList();
            ContentAnimeall = _databaseService.GetContentByType("Аниме").ToList();
            ContentFilmall = _databaseService.GetContentByType("Фильм").ToList();
            ContentDoramaall = _databaseService.GetContentByType("Дорама").ToList();
            ContentMultall = _databaseService.GetContentByType("Мультсериал").ToList();
            ContentDocumall = _databaseService.GetContentByType("Документалка").ToList();
            ContentOtherall = _databaseService.GetContentByType("Прочее").ToList();

            ContentViewedall = _databaseService.GetContentByWatchStatus("Просмотрено").ToList();
            ContentProcessall = _databaseService.GetContentByWatchStatus("Смотрю").ToList();
            ContentNotStartall = _databaseService.GetContentByWatchStatus("Не начинал").ToList();


            ContentSerial = ContentSerialall.Take(8).ToList();
            ContentAnime = ContentAnimeall.Take(8).ToList();
            ContentFilm = ContentFilmall.Take(8).ToList();
            ContentDorama = ContentDoramaall.Take(8).ToList();
            ContentMult = ContentMultall.Take(8).ToList();
            ContentDocum = ContentDocumall.Take(8).ToList();
            ContentOther = ContentOtherall.Take(8).ToList();
            ContentViewed = ContentViewedall.Take(8).ToList();
            ContentProcess = ContentProcessall.Take(8).ToList();
            ContentNotStart = ContentNotStartall.Take(8).ToList();
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

        private static List<Content> GetContentStatus(string type)
        {
            return _databaseService.GetContentByWatchStatus(type).ToList();
        }
    }
}
