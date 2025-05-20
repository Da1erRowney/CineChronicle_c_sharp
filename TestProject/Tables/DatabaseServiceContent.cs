using SQLite;

namespace CineChronicle.Tables
{
    public class DatabaseServiceContent
    {
        private SQLiteConnection _connection;

        public DatabaseServiceContent(string _databasePath)
        {
            _connection = new SQLiteConnection(_databasePath);
            CreateTables();
        }

        //Таблицы
        public void CreateTables()
        {
            if (!TableExists<Authorized>()) _connection.CreateTable<Authorized>();
            if (!TableExists<Content>()) _connection.CreateTable<Content>();
            if (!TableExists<ContentRecommendation>()) _connection.CreateTable<ContentRecommendation>();
            if (!TableExists<DateExit>()) _connection.CreateTable<DateExit>();
            if (!TableExists<User>()) _connection.CreateTable<User>();
            if (!TableExists<UserSettings>()) _connection.CreateTable<UserSettings>();
            if (!TableExists<UserContents>()) _connection.CreateTable<UserContents>();
        }
        private bool TableExists<T>()
        {
            var tableName = typeof(T).Name;
            var query = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
            var result = _connection.ExecuteScalar<string>(query);
            return result != null;
        }
        public void DropAllTables()
        {
            _connection.Execute("DROP TABLE IF EXISTS Content");
            _connection.Execute("DROP TABLE IF EXISTS DateExit");
            _connection.Execute("DROP TABLE IF EXISTS User");
            _connection.Execute("DROP TABLE IF EXISTS Authorized");
            _connection.Execute("DROP TABLE IF EXISTS UserSettings");
            _connection.Execute("DROP TABLE IF EXISTS UserContents");
            _connection.Execute("DROP TABLE IF EXISTS ContentRecommendation");

           _connection.Execute("VACUUM");
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
        public List<Content> GetContentByTitle(string title)
        {
            return _connection.Table<Content>().Where(c => c.Title == title).ToList();
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



        //Рекомендованный контент
        public void InsertRecomContent(ContentRecommendation contentRecommendation)
        {
            _connection.Insert(contentRecommendation);
        }
        public List<ContentRecommendation> GetAllRecomContent()
        {
            return _connection.Table<ContentRecommendation>().ToList();
        }
        public bool IsRecomContentValid()
        {
            var recommendations = _connection.Table<ContentRecommendation>().ToList();

            // Возвращаем true, если список пуст или прошло 7 дней с даты изменения
            return !recommendations.Any() || recommendations.Any(cr => (DateTime.Now - cr.DateChange).TotalDays >= 7);
        }
        public void ClearRecomContent()
        {
            _connection.DeleteAll<ContentRecommendation>();
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
