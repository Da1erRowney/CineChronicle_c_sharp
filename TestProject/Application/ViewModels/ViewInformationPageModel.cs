using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

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

            Task.Run(InitUserData);
        }
        #endregion

        #region [Methods]
        private async Task InitUserData()
        {
            await CheckedAuthUser();
        }

        private async Task CheckedAuthUser() // Поиск авторизованного пользователя
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
            }
            else
            {
                HaveAthorizedUser = false;

                User = new User();
                User.NameIcon = "nonicon.png";
                User.NickName = "Пользователь отсутствует";
            }

            OnPropertyChanged(nameof(User));
            OnPropertyChanged(nameof(HaveAthorizedUser));
        }

        public async void ExitAccount() // Выход из аккаунта
        {
            Authorized.IsAuthenticated = false;
            _databaseService.UpdateAuth(Authorized);

            await CheckedAuthUser();
        }
        #endregion
    }

}
