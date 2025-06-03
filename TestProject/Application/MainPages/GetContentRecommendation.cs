using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using HtmlAgilityPack;

namespace CineChronicle.Application.MainPage;

public class GetContentRecommendation
{

    public async Task<List<ContentRecommendation>> GetRecommendationsAsync()
    {
        DatabaseServiceContent _databaseService = new DatabaseServiceContent();

        // Если рекомендации не пусты и не прошло 7 дней с добавления
        if (!_databaseService.IsRecomContentValid()) return _databaseService.GetAllRecomContent();

        DeviceInfo deviceInfo = new();
        if (!deviceInfo.CheckInternetConnection()) return _databaseService.GetAllRecomContent();

        _databaseService.ClearRecomContent();

        string url = "https://www.toramp.com/";

        List<ContentRecommendation> recommendations = new List<ContentRecommendation>();

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);

                HtmlNodeCollection recommendationNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='content h_scroll dis_flex pt_5 pb_5']/div[@class='pos_rel pr_5']");

                if (recommendationNodes != null)
                {
                    foreach (HtmlNode recommendationNode in recommendationNodes)
                    {
                        HtmlNode imageNode = recommendationNode.SelectSingleNode(".//a[@class='imgWrapper']/img");
                        HtmlNode titleNode = recommendationNode.SelectSingleNode(".//a[@class='imgWrapper']/img/@alt");

                        string imageSrc = imageNode?.GetAttributeValue("src", "");
                        string title = titleNode?.GetAttributeValue("alt", "");

                        if (!string.IsNullOrEmpty(imageSrc) && !string.IsNullOrEmpty(title))
                        {
                            string type = GetContentType(title); // Extract the type from the title
                            int slashIndex = title.IndexOf('/');
                            string titleBeforeSlash = title;
                            if (slashIndex >= 0)
                            {
                                titleBeforeSlash = title.Substring(0, slashIndex);
                                // Используйте titleBeforeSlash как требуется
                            }
                            else
                            {
                                // Символ `/` не найден в строке title
                            }
                            title = titleBeforeSlash;

                            var newContent = new ContentRecommendation
                            {
                                ImageUrl = imageSrc,
                                Title = title,
                                Type = type,
                                DateChange = DateTime.Now
                            };
                             _databaseService.InsertRecomContent(newContent);
                        }
                    }
                }
            }
        }

        return _databaseService.GetAllRecomContent();
    }

    public string GetContentType(string title)
    {

        if (title.Contains("(сериал)"))
        {
            return ContentTypes.SERIAL;
        }
        else if (title.Contains("(аниме)"))
        {
            return ContentTypes.ANIME;
        }
        else if (title.Contains("(мультсериал)"))
        {
            return ContentTypes.CARTOON;
        }
        else if (title.Contains("(мультфильм)"))
        {
            return ContentTypes.CARTOON; ;
        }
        else
        {
            return "неизвестно";
        }
    }
}



