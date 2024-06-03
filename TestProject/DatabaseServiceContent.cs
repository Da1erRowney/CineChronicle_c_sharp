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
        //Таблицы
        public void CreateTables()
        {
            _connection.CreateTable<Content>();
            _connection.CreateTable<DateExit>();
            _connection.CreateTable<User>();
            _connection.CreateTable<Authorized>();
            _connection.CreateTable<UserSettings>();
        }
        public void CloseConnection()
        {
            _connection?.Close();
        }
        public void DeleteTable() 
        { 
            _connection.DropTable<User>();
            _connection.DropTable<Authorized>();
        }

        //Контент
        public void InsertContent(Content content)
        {
            _connection.Insert(content);
        }

        public Content GetContentById(int id)
        {
            return _connection.Table<Content>().FirstOrDefault(c => c.Id == id);
        }
        public List<Content> GetContentByType(string type)
        {
            return _connection.Table<Content>().Where(c => c.Type == type).ToList();
        }
        public List<Content> GetContentByWatchStatus(string watchStatus)
        {
            return _connection.Table<Content>().Where(c => c.WatchStatus == watchStatus).ToList();
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


        // Пользователь
        public void InsertUser(User user)
        {
            _connection.Insert(user);
        }
        public User GetUsereByEmail(string title)
        {
            return _connection.Table<User>().FirstOrDefault(c => c.Email == title);
        }
        public void UpdateUser(User user)
        {
            _connection.Update(user);
        }
        public List<User> GetAllUser()
        {
            return _connection.Table<User>().ToList();
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
        public Authorized GetAuthorizedByEmail(string email)
        {
            return _connection.Table<Authorized>().FirstOrDefault(c => c.Email == email);
        }

    }


}
