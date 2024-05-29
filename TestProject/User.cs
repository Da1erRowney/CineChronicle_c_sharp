using SQLite;

namespace DataContent
{
    public class User
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Password { get; set; }
    }


}
