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
    }
}
