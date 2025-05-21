using CineChronicle.Application.MainPage;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;
using System.Diagnostics;

namespace TestProject;

public partial class MainPage : ContentPage
{
    public Content SelectedItem { get; set; }
    public static string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content.db");

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Подписываемся на события
        MessagingCenter.Subscribe<App>(this, "InternetConnected", (sender) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Debug.WriteLine("Соединение с интернетом есть.");

                ConnectionInternet();
            });
        });

        MessagingCenter.Subscribe<App>(this, "InternetDisconnected", (sender) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Debug.WriteLine("Нет соединения с интернетом. Показываем алерт.");
                NotConnectionInternet();
            });
        });

        // Проверяем состояние при открытии
        CheckInitialConnection();
    }

    private void ConnectionInternet()
    {
        IsDeviceOfflineBorder.IsVisible = false;
        MobilePhoneRec.IsVisible = true;
        InitializeViewModel();
    }

    private void NotConnectionInternet()
    {
        IsDeviceOfflineBorder.IsVisible = true;
        InitializeViewModel();
    }

    private async void InitializeViewModel()
    {
        var viewModel = new ViewContentMainPageModel();
        this.BindingContext = viewModel;
        await viewModel.InitializeAsync();

        PrepapePage();
    }

    private void PrepapePage()
    {
        if (ViewContentMainPageModel.isContentNull[0])
        {
            CAdded.IsVisible = false;
        }
        else
        {
            CAdded.IsVisible = true;
        }

        if (ViewContentMainPageModel.isContentNull[1])
        {
            CWatched.IsVisible = false;
        }
        else
        {
            CWatched.IsVisible = true;
        }

        if (ViewContentMainPageModel.isContentNull[2])
        {
            CRelease.IsVisible = false;
        }
        else
        {
            CRelease.IsVisible = true;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<App>(this, "InternetConnected");
        MessagingCenter.Unsubscribe<App>(this, "InternetDisconnected");
    }

    private void CheckInitialConnection()
    {
        var current = Connectivity.NetworkAccess;
        if (current != NetworkAccess.Internet)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                NotConnectionInternet();
            });
        }
        else
        {
            ConnectionInternet();
        }
    }

    private void ItemButtonClicked(object sender, EventArgs e)
    {
        HandleClick(sender);
    }

    private void HandleClick(object sender)
    {
        var button = (Button)sender;
        var item = (Content)button.CommandParameter;
        OnItemClick(item.Id);
    }

    private async void OnItemClick(int id)
    {
        SelectedItem = ViewContentMainPageModel.GetContentById(id);
        if (SelectedItem.Type == "Ваш контент")
        {
            ViewContentMainPageModel.DeleteBaseContent();
            await Navigation.PushAsync(new AddMoreContentPage());
        }
        else
        {
            ViewContentPage viewContentPage = new ViewContentPage(SelectedItem);
            await Navigation.PushAsync(viewContentPage);
        }
    }

    private void ItemButtonClickedRecommendation(object sender, EventArgs e)
    {
        var selectedItem = (ContentRecommendation)((Button)sender).CommandParameter;
        List <Content> content = ViewContentMainPageModel.FindSameContent(selectedItem.Title);
        if (content?.Count == 0)
        {
            var viewContentPage = new ViewContentPage(selectedItem);
            OnRecomClick(viewContentPage);
        }
        else
        {
            OnItemClick(content[0].Id);
        }
    }

    private async void OnRecomClick(ViewContentPage viewContentPage)
    {
        await Navigation.PushAsync(viewContentPage);
    }

   
}


