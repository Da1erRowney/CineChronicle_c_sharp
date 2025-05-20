using SQLite;

namespace CineChronicle.Tables
{
    public class ContentRecommendation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateChange {  get; set; }
       
    }
}
