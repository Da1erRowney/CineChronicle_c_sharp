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

            content.Description = parser?.Description;
            content.CountLabel = parser?.CountLabel;
            content.NextEpisodeReleaseDate = parser?.NextEpisodeReleaseDate;
            content.DateRelease = parser?.DateRelease;
            content.YouTubeLink = parser?.YouTubeLink;
            content.YouTubeBackground = parser?.YouTubeBackground;
            content.OriginalTitle = parser?.OriginalTitle;

            Content = content;
        }
    }
}
