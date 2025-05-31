using CineChronicle.Tables;

namespace CineChronicle.Application
{
    public class DeviceInfo
    {
        public string TypeDevice { get; set; }

        private static DatabaseServiceContent _databaseService;
        public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content1.db");
        public static int UserId { get; set; }

        public DeviceInfo()
        {
            TypeDevice = GetDeviceType();
        }

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
    }
}