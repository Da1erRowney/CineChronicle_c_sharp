using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CineChronicle.Application.ViewModels
{
    public partial class ViewContentCategoryPageModel : ObservableObject
    {
        private static DatabaseServiceContent _databaseService;
        public ObservableCollection<Content> ContentCategory { get; set; }
        public static int CurrentPage { get; private set; } = 0;
        public int SelectPageForUser { get; set; }
        public static int ItemsPerPage { get; } = 10;
        public bool HasMoreItems { get; private set; } = true;
        public ICommand LoadMoreCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        [ObservableProperty]
        private bool isBusy;

        public ViewContentCategoryPageModel(string name, int page = 0)
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);
            CurrentPage = page;

            string nameType = GetTypeCategory(name);
            IEnumerable<Content> contentItems;

            switch (nameType)
            {
                case "Просмотрено":
                case "Смотрю":
                case "Не начинал":
                    contentItems = GetContentStatus(nameType);
                    break;
                default:
                    contentItems = GetContent(nameType);
                    break;
            }

            // Загружаем элементы для текущей страницы
            ContentCategory = new ObservableCollection<Content>(
                contentItems.Skip(CurrentPage * ItemsPerPage).Take(ItemsPerPage));
            SelectPageForUser = CurrentPage + 1;
            OnPropertyChanged(nameof(SelectPageForUser));
            // Проверяем, есть ли еще элементы
            HasMoreItems = contentItems.Count() > (CurrentPage + 1) * ItemsPerPage;
            // LoadMoreCommand = new Command(() => LoadNextPage(name));
            NextPageCommand = new Command(() => LoadNextPage(name), () => HasMoreItems);
            PreviousPageCommand = new Command(() => LoadPreviousPage(name), () => CurrentPage > 0);
        }

        // Метод для загрузки предыдущей страницы
        public void LoadPreviousPage(string name)
        {
            if (CurrentPage <= 0) return;

            CurrentPage--;
            string nameType = GetTypeCategory(name);
            IEnumerable<Content> items;

            switch (nameType)
            {
                case "Просмотрено":
                case "Смотрю":
                case "Не начинал":
                    items = GetContentStatus(nameType);
                    break;
                default:
                    items = GetContent(nameType);
                    break;
            }

            ContentCategory.Clear();
            var itemsToAdd = items
                .Skip(CurrentPage * ItemsPerPage)
                .Take(ItemsPerPage);

            foreach (var item in itemsToAdd)
            {
                ContentCategory.Add(item);
            }

            HasMoreItems = items.Count() > (CurrentPage + 1) * ItemsPerPage;
            OnPropertyChanged(nameof(ContentCategory));
            SelectPageForUser = CurrentPage + 1;
            OnPropertyChanged(nameof(SelectPageForUser));
            // Обновляем состояние команд
            (NextPageCommand as Command)?.ChangeCanExecute();
            (PreviousPageCommand as Command)?.ChangeCanExecute();
        }

        // Метод для загрузки следующей страницы
        public void LoadNextPage(string name)
        {
            if (!HasMoreItems || IsBusy) return;
            IsBusy = true;
            try
            {
                CurrentPage++;
            string nameType = GetTypeCategory(name);
            IEnumerable<Content> newItems;

            switch (nameType)
            {
                case "Просмотрено":
                case "Смотрю":
                case "Не начинал":
                    newItems = GetContentStatus(nameType);
                    break;
                default:
                    newItems = GetContent(nameType);
                    break;
            }

            var itemsToAdd = newItems
                .Skip(CurrentPage * ItemsPerPage)
                .Take(ItemsPerPage);

                    ContentCategory = new ObservableCollection<Content>(
                        newItems.Skip(CurrentPage * ItemsPerPage).Take(ItemsPerPage)); 


            HasMoreItems = newItems.Count() > (CurrentPage + 1) * ItemsPerPage;
            OnPropertyChanged(nameof(ContentCategory));
                SelectPageForUser = CurrentPage + 1;
                OnPropertyChanged(nameof(SelectPageForUser));
                (NextPageCommand as Command)?.ChangeCanExecute();
            (PreviousPageCommand as Command)?.ChangeCanExecute();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static string GetTypeCategory(string nameSelected)
        {
            if (nameSelected == "Все ваши Сериалы")
            {
                return ContentTypes.SERIAL;
            }
            if (nameSelected == "Всё ваше Аниме")
            {
                return ContentTypes.ANIME;
            }
            if (nameSelected == "Все ваши Фильмы")
            {
                return ContentTypes.FILM;
            }
            if (nameSelected == "Все ваши Дорамы")
            {
                return ContentTypes.DORAMA;
            }
            if (nameSelected == "Все ваши Мультсериалы")
            {
                return ContentTypes.CARTOON;
            }
            if (nameSelected == "Все ваши Документальные фильмы")
            {
                return "Документалки";
            }
            if (nameSelected == "Ваш прочий контент")
            {
                return ContentTypes.OTHER;
            }
            if (nameSelected == "Просмотренный контент")
            {
                return "Просмотрено";
            }
            if (nameSelected == "Контент, который вы начали смотреть")
            {
                return "Смотрю";
            }
            if (nameSelected == "Не начатый контент")
            {
                return "Не начинал";
            }

            return "Весь";
        }
        private static List<Content> GetContentStatus(string type)
        {
            return _databaseService.GetContentByWatchStatus(type).ToList();
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

        public static Content GetContentById(int id)
        {
            return _databaseService.GetContentById(id);
        }

        public static void DeleteBaseContent()
        {
            _databaseService.DeleteContent(_databaseService.GetContentByTitle("Нажмите, чтобы добавить контент")[0]);
        }
    }
}
