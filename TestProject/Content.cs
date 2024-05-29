using SQLite;

namespace DataContent
{
    public class Content
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Dubbing { get; set; }
        public int LastWatchedSeries { get; set; }
        public int LastWatchedSeason { get; set; }
        public string NextEpisodeReleaseDate { get; set; }
        public string WatchStatus { get; set; }
        public string Link { get; set; }  
        public string DateAdded { get; set; }
        public string SeriesChangeDate { get; set; }
        public string Image { get; set; }
        public string SmallDecription { get; set; }
    }


}
