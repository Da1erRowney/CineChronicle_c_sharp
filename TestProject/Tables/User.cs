using SQLite;

namespace CineChronicle.Tables
{
    public class User
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Password { get; set; }

        public string NameIcon { get; set; }
    }
}
