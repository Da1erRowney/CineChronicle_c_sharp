using SQLite;

namespace CineChronicle.Tables
{
    public class UserSettings
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Theme { get; set; }
        public bool IsVideoBackground { get; set; } 
    }
}
