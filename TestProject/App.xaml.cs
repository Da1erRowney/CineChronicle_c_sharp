using CineChronicle.Tables;

namespace TestProject;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }
    private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        // Получаем текущее состояние сети
        var access = e.NetworkAccess;
        var profiles = e.ConnectionProfiles;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (access == NetworkAccess.Internet)
            {
                // Интернет появился
                MessagingCenter.Send(this, "InternetConnected");
            }
            else
            {
                // Интернет пропал
                MessagingCenter.Send(this, "InternetDisconnected");
            }
        });
    }

    protected override void CleanUp()
    {
        // Не забываем отписаться при закрытии
        Connectivity.ConnectivityChanged -= OnConnectivityChanged;
        base.CleanUp();
    }
}
