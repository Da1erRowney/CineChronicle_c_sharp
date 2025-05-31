using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;

namespace CineChronicle.Application.ViewModels
{

    public partial class ViewInformationPageModel : ObservableObject
    {
        #region [Private Fields]
        private static DatabaseServiceContent _databaseService;
        public Authorized Authorized { get; set; }
        public User User { get; set; }
        public UserContents UserContents { get; set; }
        public Content Content { get; set; }

        public int TotalContentCount { get; set; }
        public int SeriesCount { get; set; }
        public int AnimeCount { get; set; }
        public int MoviesCount { get; set; }
        public int DoramaCount { get; set; }
        public int CartoonCount { get; set; }
        public int DocumentalCount { get; set; }
        public int OtherCount { get; set; }
        public int WatchedCount { get; set; }
        public int WatchNowCount { get; set; }
        public int NotStartedCount { get; set; }

        public int OngoingContentCount { get; set; }
        public int CompletedContentCount { get; set; }

        public string PreferredVoiceOver { get; set; }
        public string FavoriteGenre { get; set; }

        public bool HaveAthorizedUser = false;

        private static int[] usersContent;
        #endregion

        #region [Ctor's]
        public ViewInformationPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);
            CheckedAuthUser();
        }
        #endregion

        #region [Async Find Information Block]
        private async Task InitUserData()
        {
            await InitializeOurStatics();
            await InitializeCategory();
            await InitializeStatusContent();
            await InitializeLikedUser();
        }
        public async Task InitializeOurStatics()
        {
            TotalContentCount = _databaseService.GetContentCount(usersContent);
            NotStartedCount = _databaseService.GetContentCountByWatchStatus("Не начинал", usersContent);
            WatchNowCount = _databaseService.GetContentCountByWatchStatus("Смотрю", usersContent);
            WatchedCount = _databaseService.GetContentCountByWatchStatus("Просмотрено", usersContent);

            OnPropertyChanged(nameof(TotalContentCount));
            OnPropertyChanged(nameof(NotStartedCount));
            OnPropertyChanged(nameof(WatchNowCount));
            OnPropertyChanged(nameof(WatchedCount));
        }
        public async Task InitializeCategory()
        {
            SeriesCount = _databaseService.GetContentCountByType(ContentTypes.SERIAL, usersContent);
            AnimeCount = _databaseService.GetContentCountByType(ContentTypes.ANIME, usersContent);
            MoviesCount = _databaseService.GetContentCountByType(ContentTypes.FILM, usersContent);
            DoramaCount = _databaseService.GetContentCountByType(ContentTypes.DORAMA, usersContent);
            CartoonCount = _databaseService.GetContentCountByType(ContentTypes.CARTOON, usersContent);
            DocumentalCount = _databaseService.GetContentCountByType("Документалка", usersContent);
            OtherCount = _databaseService.GetContentCountByType(ContentTypes.OTHER, usersContent);

            OnPropertyChanged(nameof(SeriesCount));
            OnPropertyChanged(nameof(AnimeCount));
            OnPropertyChanged(nameof(MoviesCount));
            OnPropertyChanged(nameof(DoramaCount));
            OnPropertyChanged(nameof(CartoonCount));
            OnPropertyChanged(nameof(DocumentalCount));
            OnPropertyChanged(nameof(OtherCount));
        }
        public async Task InitializeStatusContent()
        {
            OngoingContentCount = _databaseService.GetAllContent(usersContent).Count(c => c.DateRelease != null && c.DateRelease != string.Empty);
            CompletedContentCount = TotalContentCount - OngoingContentCount;
            OnPropertyChanged(nameof(OngoingContentCount));
            OnPropertyChanged(nameof(CompletedContentCount));
        }
        public async Task InitializeLikedUser()
        {
            PreferredVoiceOver = _databaseService.GetFavoriteDubbing(usersContent);
            FavoriteGenre = _databaseService.GetFavoriteCategory(usersContent);
            OnPropertyChanged(nameof(PreferredVoiceOver));
            OnPropertyChanged(nameof(FavoriteGenre));
        }
        #endregion

        #region [Methods]

        private void CheckedAuthUser() // Поиск авторизованного пользователя
        {

            if (_databaseService.GetAuthorizedByAuth(true) != null)                     // Если есть авторизованный пользователь в системе
            {
                HaveAthorizedUser = true;
                Authorized = _databaseService.GetAuthorizedByAuth(true);

                User = _databaseService.GetUsereByEmail(Authorized.Email);

                if (string.IsNullOrEmpty(User.NickName))
                {
                    int atIndex = User.Email.IndexOf('@');
                    if (atIndex != -1)
                    {
                        User.NickName = User.Email.Substring(0, atIndex);
                    }
                }
                DeviceInfo.UserId = User.Id;
                usersContent = DeviceInfo.GetContentUser();
                Task.Run(InitUserData);
            }
            else
            {
                HaveAthorizedUser = false;

                User = new User();
                User.NameIcon = "nonicon.png";
                User.NickName = "Пользователь отсутствует";
                DeviceInfo.UserId = 0;
            }

            OnPropertyChanged(nameof(User));
            OnPropertyChanged(nameof(HaveAthorizedUser));
        }

        public void ExitAccount() // Выход из аккаунта
        {
            Authorized.IsAuthenticated = false;
            _databaseService.UpdateAuth(Authorized);

            CheckedAuthUser();
        }
        #endregion

        #region [Change user Avatar]
        public async Task ChangeAvatarAsync(string nickName)
        {
            if (User.NameIcon == "nonicon.png") return;
            try
            {
                // Проверяем и запрашиваем разрешения
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Photos>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("Permission Denied", "Can't access photos without permission", "OK");
                    return;
                }

                // Выбираем фото из галереи
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    // Для сохранения выбранного изображения (опционально)
                    var avatarFileName = GenerateRandomAvatarName() + ".jpg"; // Генерируем имя файла
                    var newFile = Path.Combine(FileSystem.CacheDirectory, avatarFileName);
                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFile))
                    {
                        await stream.CopyToAsync(newStream);
                    }
                    var newImage = ImageSource.FromFile(newFile);
                    User.NameIcon = User.NameIcon = newFile;

                    _databaseService.UpdateUser(User);
                    OnPropertyChanged(nameof(User));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private string GenerateRandomAvatarName(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder avatarName = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(validChars.Length);
                avatarName.Append(validChars[index]);
            }

            return avatarName.ToString();
        }
        #endregion
    }

}
