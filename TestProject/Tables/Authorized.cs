using SQLite;

namespace CineChronicle.Tables
{
    public class Authorized
    {
        [PrimaryKey]
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
