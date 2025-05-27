using SQLite;

namespace CineChronicle.Tables
{
    public class UserContents
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UserId { get; set; }

        // Строка для хранения идентификаторов контента, разделенных запятыми
        public string ContentIds { get; set; } = string.Empty;

        // Метод для получения списка ContentId из строки
        public int[] GetContentIdArray()
        {
            return !string.IsNullOrEmpty(ContentIds)
                ? ContentIds.Split(',').Select(int.Parse).ToArray()
                : new int[0];
        }

        // Метод для добавления нового ContentId в строку
        public void AddContentId(int contentId)
        {
            var contentIdList = GetContentIdArray().ToList();
            if (!contentIdList.Contains(contentId))
            {
                contentIdList.Add(contentId);
                ContentIds = string.Join(",", contentIdList);
            }
        }
        public void RemoveContentId(int contentId)
        {
            var contentIdList = GetContentIdArray().ToList();

            // Удаляем идентификатор, если он существует
            if (contentIdList.Contains(contentId))
            {
                contentIdList.Remove(contentId);
                ContentIds = string.Join(",", contentIdList); // Обновляем строку
            }
        }
    }
}