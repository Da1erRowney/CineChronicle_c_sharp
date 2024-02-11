using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace DataContent
{
    public class DatabaseServiceContent
    {
        private SQLiteConnection _connection;

        public DatabaseServiceContent(string databasePath)
        {
            _connection = new SQLiteConnection(databasePath);
            _connection.CreateTable<Content>();
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
    }

    public class Content
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Dubbing { get; set; }
        public int LastWatchedEpisode { get; set; }
        public string NextEpisodeReleaseDate { get; set; }
        public string WatchStatus { get; set; }
    }
}
