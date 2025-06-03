using SQLite;

namespace CineChronicle.Tables
{
    public class DatabaseServiceContent
    {
        //private SQLiteConnection _context;
        private readonly CineChronicleContext _context;
        public DatabaseServiceContent()
        {
            _context = new CineChronicleContext();
            _context.Database.EnsureCreated(); // Создает БД, если ее нет
        }

        #region [Таблицы]
        public void CloseConnection()
        {
            //_context?.Close();
        }
        #endregion

        #region [Контент}
        public void InsertContent(Content content)
        {
            _context.Content.Add(content);
            _context.SaveChanges(); // Добавлено сохранение изменений

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
            _context.Content.Update(content); // Добавлено уточнение Content
            _context.SaveChanges(); // Добавлено сохранение изменений
        }

        public void DeleteContent(Content content)
        {
            _context.Content.Remove(content); // Изменено с Delete на Remove
            _context.SaveChanges(); // Добавлено сохранение изменений

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

            return _context.Content
                .Where(c => ids.Contains(c.Id))
                .ToList();
        }
        public Content GetContentById(int id)
        {
            return _context.Content.FirstOrDefault(c => c.Id == id);
        }
        public List<Content> GetContentByType(string type, int[] ids)
        {
            return _context.Content
                .Where(c => c.Type == type && ids.Contains(c.Id))
                .ToList();
        }
        public List<Content> GetContentByWatchStatus(string watchStatus, int[] ids)
        {
            return _context.Content
                .Where(c => c.WatchStatus == watchStatus && ids.Contains(c.Id))
                .ToList();
        }
        public List<Content> GetContentByTitle(string title, int[] ids)
        {
            return _context.Content
                .Where(c => c.Title == title && ids.Contains(c.Id))
                .ToList();
        }

        public int GetContentCountByType(string type, int[] ids)
        {
            return _context.Content
                .Count(x => x.Type == type && ids.Contains(x.Id));
        }
        public int GetContentCount(int[] ids)
        {
            // Проверка на null
            if (ids == null || ids.Length == 0)
            {
                return 0; // Возвращаем 0, если массив null или пуст
            }

            return _context.Content
                .Count(x => ids.Contains(x.Id));
        }
        public int GetContentCountByWatchStatus(string watchStatus, int[] ids)
        {
            return _context.Content
                .Count(x => x.WatchStatus == watchStatus && ids.Contains(x.Id));
        }
        public string GetFavoriteDubbing(int[] ids)
        {
            return _context.Content
                .Where(x => ids.Contains(x.Id) && !string.IsNullOrEmpty(x.Dubbing))
                .GroupBy(x => x.Dubbing)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "Не указано";
        }

        public string GetFavoriteCategory(int[] ids)
        {
            return _context.Content
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
            _context.DateExit.Add(data);
            _context.SaveChanges();
        }
        public void UpdateContent(DateExit data)
        {
            _context.DateExit.Update(data);
            _context.SaveChanges();
        }

        public void DeleteContent(DateExit data)
        {
            _context.DateExit.Remove(data);
            _context.SaveChanges();
        }
        public DateExit GetDateByTitle(string title)
        {
            return _context.DateExit.FirstOrDefault(c => c.Title == title);
        }
        #endregion

        #region [Рекомендованный контент]
        //Рекомендованный контент
        public void InsertRecomContent(ContentRecommendation contentRecommendation)
        {
            _context.ContentRecommendation.Add(contentRecommendation);
            _context.SaveChanges();
        }
        public List<ContentRecommendation> GetAllRecomContent()
        {
            return _context.ContentRecommendation.ToList();
        }
        public bool IsRecomContentValid()
        {
            var recommendations = _context.ContentRecommendation.ToList();

            // Возвращаем true, если список пуст или прошло 7 дней с даты изменения
            return !recommendations.Any() || recommendations.Any(cr => (DateTime.Now - cr.DateChange).TotalDays >= 7);
        }
        public void ClearRecomContent()
        {
            var allRecommendations = _context.ContentRecommendation.ToList();
            _context.ContentRecommendation.RemoveRange(allRecommendations);
            _context.SaveChanges();
        }
        #endregion

        #region [Пользователь] 
        public void InsertUser(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }
        public User GetUsereByEmail(string title)
        {
            return _context.User.FirstOrDefault(c => c.Email == title);
        }
        public User GetUsereByNickName(string nickName)
        {
            return _context.User.FirstOrDefault(c => c.NickName == nickName);
        }
        public int GetUserIdByEmail(string email)
        {
            var user = _context.User.FirstOrDefault(c => c.Email == email);
            return user.Id; // Возвращаем Id или null, если пользователь не найден
        }
        public void UpdateUser(User user)
        {
            _context.User.Update(user);
            _context.SaveChanges();
        }
        public List<User> GetAllUser()
        {
            return _context.User.ToList();
        }
        #endregion

        #region [Авторизованный пользователь]

        //Авторизованный пользователь
        public void InsertAuth(Authorized authorized)
        {
            _context.Authorized.Add(authorized);
            _context.SaveChanges();
        }
        public void UpdateAuth(Authorized authorized)
        {
            _context.Authorized.Update(authorized);
            _context.SaveChanges();
        }
        public Authorized GetAuthorizedByAuth(bool status)
        {
            return _context.Authorized.FirstOrDefault(c => c.IsAuthenticated == status);
        }
        public Authorized GetAuthorizedByEmail(string email)
        {
            return _context.Authorized.FirstOrDefault(c => c.Email == email);
        }
        #endregion

        #region [Пользовательский контент]
        public List<int> GetUnlinkedContentIds()
        {
            // Получаем все идентификаторы контента
            var allContentIds = _context.Content
                .Select(c => c.Id)
                .ToList();

            // Получаем все связанные идентификаторы
            var linkedContentIds = _context.UserContents
                .AsEnumerable() // Переключаемся на клиентскую обработку для Split
                .SelectMany(uc => uc.ContentIds.Split(',')
                    .Select(id => int.TryParse(id, out var contentId) ? contentId : (int?)null))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct() // Убираем дубликаты
                .ToList();

            // Возвращаем только несвязанные ID
            return allContentIds.Except(linkedContentIds).ToList();
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
            _context.UserContents.Add(userContent);
            }
            else
            {
                for (int i = 0; i < contentId.Count; i++)
                {
                    oldCards.AddContentId(contentId[i]);
                }
                _context.Update(oldCards);
                _context.SaveChanges();
            }
            _context.SaveChanges();
                    
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
                _context.UserContents.Update(oldCards);
                _context.SaveChanges();
            }

        }
        public UserContents GetUserContentByUserId(int userId)
        {
            return _context.UserContents
                .FirstOrDefault(uc => uc.UserId == userId);
        }
        #endregion

        #region [Пользовательские настройки]
        public void InsertUserSetting(UserSettings user)
        {
            _context.UserSettings.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUserSetting(UserSettings user)
        {
            _context.UserSettings.Update(user);
            _context.SaveChanges();
        }

        public UserSettings GetUserSettingById(int id)
        {
            return _context.UserSettings.FirstOrDefault(c => c.UserId == id);
        }
        #endregion
    }
}
