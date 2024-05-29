using SQLite;

namespace DataContent
{
    public class Authorized
    {
        [PrimaryKey]
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }


}
