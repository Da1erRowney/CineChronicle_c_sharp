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
            _connection.CreateTable<User>();
            _connection.CreateTable<Authorized>();
            _connection.CreateTable<UserSettings>();
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


}
