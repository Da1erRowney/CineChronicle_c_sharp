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
            BindingContext = content; // ����������� ������ Content � BindingContext ��������
            OpenLinkCommand = new Command<string>(OpenLink);
            SetupLabelTappedEvents();
        }

        private void SetupLabelTappedEvents()
        {
            // ��� ������������� � �����, ��� ��� � ��� ������ ���� �����
            // ����� ������ �������� ���������� ��� ���� �����
            if (LinkLabel != null)
            {
                LinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = OpenLinkCommand,
                    CommandParameter = (BindingContext as Content)?.Link // �������� ������ � �������� ��������� �������
                });
            }
        }

        private async void OpenLink(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                // ��������� ������ � ��������
                await Browser.OpenAsync(new Uri(link), BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}