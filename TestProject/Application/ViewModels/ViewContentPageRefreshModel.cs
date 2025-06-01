using CineChronicle.Application.SupportClass;
using CineChronicle.Tables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineChronicle.Application.ViewModel
{
    public class ViewContentPageRefreshModel : ObservableObject
    {
        public Content Content { get; }

        public ViewContentPageRefreshModel(Content content)
        {
            Content = content;
        }
        public ViewContentPageRefreshModel(Tables.ContentRecommendation contentRecommendation, GetParsingInfo parser)
        {
            Content content = new();
            content.Title = contentRecommendation.Title;
            content.Type = contentRecommendation.Type;
            content.Image = contentRecommendation.ImageUrl;

            content.SourceLink = string.IsNullOrEmpty(parser?.ExtractUrlWatch) ? GetSourceLink(contentRecommendation.Type, contentRecommendation.Title) : parser.ExtractUrlWatch;
            content.Description = parser?.Description;
            content.CountLabel = parser?.CountLabel;
            content.NextEpisodeReleaseDate = parser?.NextEpisodeReleaseDate;
            content.DateRelease = parser?.DateRelease;
            content.YouTubeLink = parser?.YouTubeLink;
            content.YouTubeBackground = parser?.YouTubeBackground;
            content.OriginalTitle = parser?.OriginalTitle;

            Content = content;
        }
        private string GetSourceLink(string type, string title)
        {
            // Обновляем ссылку на источник
            switch (type)
            {
                case ContentTypes.ANIME:
                    return "https://animego.org/search/all?q=" + title;
                case ContentTypes.DORAMA:
                    return "https://dorama.land/search?q=" + title;
                case ContentTypes.SERIAL:
                case ContentTypes.CARTOON:
                case ContentTypes.FILM:
                    return "https://kinogo.biz/search/" + title;
                default:
                    return "https://kinogo.biz/search/" + title;
            }
        }
    }
}
