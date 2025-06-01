using SQLite;

namespace CineChronicle.Tables
{
    public class DatabaseServiceContent
    {
        private SQLiteConnection _connection;

        public DatabaseServiceContent(string _databasePath)
        {
            _connection = new SQLiteConnection(_databasePath);
            //DropAllTables();
            CreateTables();
        }

        #region [Таблицы]
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
        #endregion

        #region [Контент}
        public void InsertContent(Content content)
        {
            _connection.Insert(content);

            var AuthUser = GetAuthorizedByAuth(true);
            if (AuthUser == null) { }
            else
            {
                int userId = GetUserIdByEmail(AuthUser.Email);
                AddUserContent(userId, [content.Id]);
            }

        }
        public void UpdateContent(Content content)
        {
            _connection.Update(content);
        }

        public void DeleteContent(Content content)
        {
            _connection.Delete(content);
            var AuthUser = GetAuthorizedByAuth(true);
            if (AuthUser == null) { }
            else
            {
                int userId = GetUserIdByEmail(AuthUser.Email);
                DeleteUserContent(userId, [content.Id]);
            }
        }
        public List<Content> GetAllContent(int[] ids)
        {
            // Проверка на null
            if (ids == null)
            {
                return new List<Content>(); // Возвращаем пустой список, если ids равен null
            }

            return _connection.Table<Content>()
                .Where(c => ids.Contains(c.Id))
                .ToList();
        }
        public Content GetContentById(int id)
        {
            return _connection.Table<Content>().FirstOrDefault(c => c.Id == id);
        }
        public List<Content> GetContentByType(string type, int[] ids)
        {
            return _connection.Table<Content>()
                .Where(c => c.Type == type && ids.Contains(c.Id))
                .ToList();
        }
        public List<Content> GetContentByWatchStatus(string watchStatus, int[] ids)
        {
            return _connection.Table<Content>()
                .Where(c => c.WatchStatus == watchStatus && ids.Contains(c.Id))
                .ToList();
        }
        public List<Content> GetContentByTitle(string title, int[] ids)
        {
            return _connection.Table<Content>()
                .Where(c => c.Title == title && ids.Contains(c.Id))
                .ToList();
        }

        public int GetContentCountByType(string type, int[] ids)
        {
            return _connection.Table<Content>()
                .Count(x => x.Type == type && ids.Contains(x.Id));
        }
        public int GetContentCount(int[] ids)
        {
            // Проверка на null
            if (ids == null || ids.Length == 0)
            {
                return 0; // Возвращаем 0, если массив null или пуст
            }

            return _connection.Table<Content>()
                .Count(x => ids.Contains(x.Id));
        }
        public int GetContentCountByWatchStatus(string watchStatus, int[] ids)
        {
            return _connection.Table<Content>()
                .Count(x => x.WatchStatus == watchStatus && ids.Contains(x.Id));
        }
        public string GetFavoriteDubbing(int[] ids)
        {
            return _connection.Table<Content>()
                .Where(x => ids.Contains(x.Id) && !string.IsNullOrEmpty(x.Dubbing))
                .GroupBy(x => x.Dubbing)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "Не указано";
        }

        public string GetFavoriteCategory(int[] ids)
        {
            return _connection.Table<Content>()
                .Where(x => ids.Contains(x.Id) && !string.IsNullOrEmpty(x.Type))
                .GroupBy(x => x.Type)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "Не указано";
        }
        #endregion

        #region [Дата выхода]
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
        #endregion

        #region [Рекомендованный контент]
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
        #endregion

        #region [Пользователь] 
        public void InsertUser(User user)
        {
            _connection.Insert(user);
        }
        public User GetUsereByEmail(string title)
        {
            return _connection.Table<User>().FirstOrDefault(c => c.Email == title);
        }
        public User GetUsereByNickName(string nickName)
        {
            return _connection.Table<User>().FirstOrDefault(c => c.NickName == nickName);
        }
        public int GetUserIdByEmail(string email)
        {
            var user = _connection.Table<User>().FirstOrDefault(c => c.Email == email);
            return user.Id; // Возвращаем Id или null, если пользователь не найден
        }
        public void UpdateUser(User user)
        {
            _connection.Update(user);
        }
        public List<User> GetAllUser()
        {
            return _connection.Table<User>().ToList();
        }
        #endregion

        #region [Авторизованный пользователь]

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
        #endregion

        #region [Пользовательский контент]
        public List<int> GetUnlinkedContentIds()
        {
            // Получаем все идентификаторы контента из таблицы Content
            var allContentIds = _connection.Table<Content>()
                .Select(c => c.Id)
                .ToList();

            // Получаем все идентификаторы контента, которые связаны в таблице UserContents
            var linkedContentIds = _connection.Table<UserContents>()
                .SelectMany(uc => uc.ContentIds.Split(',')
                    .Select(id => int.TryParse(id, out var contentId) ? contentId : (int?)null))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            // Возвращаем идентификаторы контента, которые не связаны с UserContents
            return allContentIds.Where(id => !linkedContentIds.Contains(id)).ToList();
        }

        // Добавить запись в UserContents
        public void AddUserContent(int userId, List<int> contentId)
        {
            var oldCards = GetUserContentByUserId(userId);
            if (oldCards == null)
            {
                var userContent = new UserContents
                {
                    UserId = userId,

                };
                for (int i = 0; i < contentId.Count; i++)
                {
                    userContent.AddContentId(contentId[i]);
                }
            _connection.Insert(userContent);
            }
            else
            {
                for (int i = 0; i < contentId.Count; i++)
                {
                    oldCards.AddContentId(contentId[i]);
                }
                _connection.Update(oldCards);
            }
                    
        }
        // Удалить запись в UserContents
        public void DeleteUserContent(int userId, List<int> contentId)
        {
            var oldCards = GetUserContentByUserId(userId);
            {
                for (int i = 0; i < contentId.Count; i++)
                {
                    oldCards.RemoveContentId(contentId[i]);
                }
                _connection.Update(oldCards);
            }

        }
        public UserContents GetUserContentByUserId(int userId)
        {
            return _connection.Table<UserContents>()
                .FirstOrDefault(uc => uc.UserId == userId);
        }
        #endregion

        #region [Пользовательские настройки]
        public void InsertUserSetting(UserSettings user)
        {
            _connection.Insert(user);
        }

        public void UpdateUserSetting(UserSettings user)
        {
            _connection.Update(user);
        }

        public UserSettings GetUserSettingById(int id)
        {
            return _connection.Table<UserSettings>().FirstOrDefault(c => c.UserId == id);
        }
        #endregion
    }
}
