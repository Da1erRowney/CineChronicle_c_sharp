using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineChronicle.Application.ViewModels
{

    public partial class ViewInformationPageModel : ObservableObject
    {
        private static DatabaseServiceContent _databaseService;
        public Authorized Authorized { get; set; }
        public User User { get; set; }
        public UserContents UserContents { get; set; }
        public Content Content { get; set; }

        public bool HaveAthorizedUser = false;

        public ViewInformationPageModel()
        {
            _databaseService = new DatabaseServiceContent(DeviceInfo._databasePath);

            Task.Run(InitUserData);
        }

        private async Task InitUserData()
        {
            await CheckedAuthUser();
        }

        private async Task CheckedAuthUser()
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

        public async void ExitAccount()
        {
            Authorized.IsAuthenticated = false;
            _databaseService.UpdateAuth(Authorized);

            await CheckedAuthUser();
        }
    }
    
}
