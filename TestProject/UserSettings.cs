using SQLite;

namespace DataContent
{
    public class UserSettings
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Theme { get; set; }
    }


}
