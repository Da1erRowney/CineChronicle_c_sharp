namespace CineChronicle.Application
{
    public class DeviceInfo
    {
        public string TypeDevice { get; set; }

        public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content1.db");

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
    }
}