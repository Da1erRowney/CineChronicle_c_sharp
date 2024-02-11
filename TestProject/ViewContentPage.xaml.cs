using DataContent;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Windows.Input;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewContentPage : ContentPage
    {
        public ICommand OpenLinkCommand { get; private set; }

        public ViewContentPage(Content content)
        {
            InitializeComponent();
            BindingContext = content; // Привязываем объект Content к BindingContext страницы
            OpenLinkCommand = new Command<string>(OpenLink);
            SetupLabelTappedEvents();
        }

        private void SetupLabelTappedEvents()
        {
            // Нет необходимости в цикле, так как у вас только одна метка
            // Можно просто добавить обработчик для этой метки
            if (LinkLabel != null)
            {
                LinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = OpenLinkCommand,
                    CommandParameter = (BindingContext as Content)?.Link // Передаем ссылку в качестве параметра команды
                });
            }
        }

        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                // Открываем ссылку в браузере
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}