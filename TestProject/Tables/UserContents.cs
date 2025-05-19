using SQLite;

namespace CineChronicle.Tables
{
    public class UserContents
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ContentId { get; set; }
    
        // Поле для хранения ID пользователей, которым доступен контент
        public string AccessibleUserIds { get; set; } = null; // По умолчанию null
    }
}
