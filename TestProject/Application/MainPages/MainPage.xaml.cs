using CineChronicle.Application.MainPage;
using CineChronicle.Application.ViewModels;
using CineChronicle.Tables;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestProject;

public partial class MainPage : ContentPage
{
    public Content SelectedItem { get; set; }
    public CineChronicle.Application.DeviceInfo _device = new();
    public static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "content.db");

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

                IsDeviceOfflineBorder.IsVisible = false;
                _device.NotifyUse = false;
                MobilePhoneRec.IsVisible = _device.TypeDevice != "WinUI";
                BindingContext = new ViewContentMainPageModel();
            });
        });

        MessagingCenter.Subscribe<App>(this, "InternetDisconnected", (sender) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Debug.WriteLine("Нет соединения с интернетом. Показываем алерт.");
                MobilePhoneRec.IsVisible = false;
                _device.NotifyUse = true;
                IsDeviceOfflineBorder.IsVisible = true;
                BindingContext = new ViewContentMainPageModel();
            });
        });

        // Проверяем состояние при открытии
        CheckInitialConnection();
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
                IsDeviceOfflineBorder.IsVisible = true;
                MobilePhoneRec.IsVisible = false;
                BindingContext = new ViewContentMainPageModel();
            });
        }
        else
        {
            IsDeviceOfflineBorder.IsVisible = false;
            _device.NotifyUse = false;
            MobilePhoneRec.IsVisible = _device.TypeDevice != "WinUI";
            BindingContext = new ViewContentMainPageModel();
        }
    }

    private void ItemButtonClickedChange(object sender, EventArgs e)
    {
        HandleClick(sender);
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

    private async void ItemButtonClickedRecommendation(object sender, EventArgs e)
    {
        var selectedItem = (ContentRecommendation)((Button)sender).CommandParameter;
        var viewContentPage = new ViewContentPage(selectedItem);

        await Navigation.PushAsync(viewContentPage);
    }

    private async void OnItemClick(int id)
    {
        SelectedItem = ViewContentMainPageModel.GetContentById(id);
        if (SelectedItem.Type == "Пустота")
        {
            await Navigation.PushAsync(new AddMoreContentPage());
        }
        else
        {
            ViewContentPage viewContentPage = new ViewContentPage(SelectedItem);
            await Navigation.PushAsync(viewContentPage);
        }
    } 
}


