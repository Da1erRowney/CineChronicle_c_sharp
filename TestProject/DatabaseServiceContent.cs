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

        //База
        public void CloseConnection()
        {
            _connection?.Close();
        }
        public void CreateTables()
        {
            _connection.CreateTable<Content>();
            _connection.CreateTable<DateExit>();
            _connection.CreateTable<User>();
            _connection.CreateTable<Authorized>();
            _connection.CreateTable<UserSettings>();
        }

        //Контент
        public void InsertContent(Content content)
        {
            _connection.Insert(content);
        }

        public Content GetContentByTitle(string title)
        {
            return _connection.Table<Content>().FirstOrDefault(c => c.Title == title);
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

        //Дата выхода
        public void InsertDate(DateExit data)
        {
            _connection.Insert(data);
        }
        public void UpdateContent(DateExit data)
        {
            _connection.Update(data);
        }
        public void DeleteDateExit(DateExit data)
        {
            _connection.Delete(data);
        }
        public DateExit GetDateByTitle(string title)
        {
            return _connection.Table<DateExit>().FirstOrDefault(c => c.Title == title);
        }


        // Пользователь
        public void InsertUser(User user)
        {
            _connection.Insert(user);
        }

        //Авторизованный пользователь
        public void InsertAuth(Authorized authorized)
        {
            _connection.Insert(authorized);
        }
        public void UpdateAuth(Authorized authorized)
        {
            _connection.Update(authorized);
        }
        public Authorized GetAuthorizedByAuth(bool status)
        {
            return _connection.Table<Authorized>().FirstOrDefault(c => c.IsAuthenticated == status);
        }
    }

    public class Content
    {
        [PrimaryKey]
        public string Email { get; set; }
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
        public string Email { get; set; }
        public string Title { get; set; }
        public string DateRelease { get; set; }
        public string SendStatus { get; set; }
    }
    public class User
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class Authorized
    {
        [PrimaryKey]
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
    public class UserSettings
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Theme { get; set; }
    }
}
