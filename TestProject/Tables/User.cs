using SQLite;

namespace CineChronicle.Tables
{
    public class User
    {
        [PrimaryKey,AutoIncrement]
        public int Id {get; set; }
        public string Email { get; set; }
        public string NickName {get; set;}
        public string Password { get; set; }
        public string NameIcon { get; set; }
    }
}
