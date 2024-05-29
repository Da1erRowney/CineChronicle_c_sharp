using SQLite;

namespace DataContent
{
    public class DateExit
    {
        [PrimaryKey]
        public string Title { get; set; }
        public string Email { get; set; }
        public string DateRelease { get; set; }
        public string SendStatus { get; set; }
    }


}
