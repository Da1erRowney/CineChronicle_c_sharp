using CineChronicle.Tables;

namespace CineChronicle.Application
{
    public class DeviceInfo
    {
        #region [Private Fields]
        public string TypeDevice { get; set; }
        public bool IsDarkTheme { get; set; }

        private static DatabaseServiceContent _databaseService;
        public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content1.db");
        public static int UserId { get; set; }
        #endregion

        #region [Ctor's]
        public DeviceInfo()
        {
            TypeDevice = GetDeviceType();
        }
        #endregion

        #region [Methods]
        private string GetDeviceType()
        {
            return Device.RuntimePlatform.ToString();
        }

        public bool CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;
            return current == NetworkAccess.Internet;
        }

        public static int[] GetContentUser()
        {
            _databaseService = new DatabaseServiceContent(_databasePath);
            var AuthUser = _databaseService.GetAuthorizedByAuth(true);
            if (AuthUser == null)
            {
                List<int> usersContentList = _databaseService.GetUnlinkedContentIds();
                return usersContentList.ToArray();
            }
            else
            {
                UserId = _databaseService.GetUserIdByEmail(AuthUser.Email);

                var usersContentClass = _databaseService.GetUserContentByUserId(UserId);
                return usersContentClass.GetContentIdArray();
            }
        }
        #endregion
    }
}