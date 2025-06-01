using SQLite;

namespace CineChronicle.Tables
{
    public class UserSettings
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }
        public bool IsDarkTheme { get; set; }
        public bool IsVideoBackground { get; set; }
    }
}
