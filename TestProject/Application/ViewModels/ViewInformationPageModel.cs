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

        public bool HaveAthorizedUser = false;
        #endregion

        #region [Ctor's]
        public ViewInformationPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);
            CheckedAuthUser();
            //Task.Run(InitUserData);
        }
        #endregion

        #region [Methods]
        private async Task InitUserData()
        {
            CheckedAuthUser();
        }

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
