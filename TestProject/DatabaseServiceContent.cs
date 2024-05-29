using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace DataContent
{
    public class DatabaseServiceContent
    {
        private SQLiteConnection _connection;

        public DatabaseServiceContent(string _databasePath)
        {
            _connection = new SQLiteConnection(_databasePath);
        }

        public void CreateTables()
        {
            _connection.CreateTable<Content>();
            _connection.CreateTable<DateExit>();
        }

       

        public void InsertContent(Content content)
        {
            _connection.Insert(content);
        }

        public Content GetContentById(int id)
        {
            return _connection.Table<Content>().FirstOrDefault(c => c.Id == id);
        }

        public void UpdateContent(Content content)
        {
            _connection.Update(content);
        }

        public void DeleteContent(Content content)
        {
            _connection.Delete(content);
        }

        public List<Content> GetAllContent()
        {
            return _connection.Table<Content>().ToList();
        }

        public void CloseConnection()
        {
            _connection?.Close();
        }


        public void InsertDate(DateExit data)
        {
            _connection.Insert(data);
        }
        public void UpdateContent(DateExit data)
        {
            _connection.Update(data);
        }

        public void DeleteContent(DateExit data)
        {
            _connection.Delete(data);
        }
        public DateExit GetDateByTitle(string title)
        {
            return _connection.Table<DateExit>().FirstOrDefault(c => c.Title == title);
        }



    }

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
    public class DateExit
    {
        [PrimaryKey]
        public string Title { get; set; }
        public string DateRelease { get; set; }
    }
}
