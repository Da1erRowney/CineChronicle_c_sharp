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
        private Content content;


        public ViewContentPage(Content content)
        {
            this.content = content;
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

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("�����������", $"�� �������, ��� ������ ������� {content.Title}?", "��", "���");

            if (result)
            {
                string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

                DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

                databaseService.DeleteContent(content);

                databaseService.CloseConnection();

                await Navigation.PushAsync(new MainPage());
            }
        }



        private bool isEditing = false; // ����, �����������, � ������ �������������� ��� ���

        private void EditButton_Clicked(object sender, EventArgs e)
        {
            if (isEditing)
            {
                // ���� ��� � ������ ��������������, �� ����� ��������� ���������
                SaveChanges();
            }
            else
            {
                // ���� �� � ������ ��������������, �� ������������� � ���� �����
                StartEditing();
            }
        }

        private void StartEditing()
        {
            isEditing = true;
            EditButton.Text = "���������";
            CancelButton.IsVisible = true; // ���������� ������ "������"

            // �������������� ���� �����
            LinkSecondLabel.IsVisible = true;
            LinkEntry.IsVisible = true;
            LinkEntry.IsReadOnly = false;
            LinkLabel.IsVisible = false;
            DateAddedLabel.IsVisible = false;
            DateAddedEntry.IsVisible = false;
            SeriesChangeDateLabel.IsVisible = false;
            SeriesChangeDateEntry.IsVisible = false;

            TypePicker.IsVisible = true;
            TypePicker.SelectedItem = content.Type;
            WatchStatusPicker.IsVisible = true;
            WatchStatusPicker.SelectedItem = content.WatchStatus;


            TypeEntry.IsVisible = false;
            TypeLabel.IsVisible = false;
            WatchStatusEntry.IsVisible = false;
            WatchStatusLabel.IsVisible = false;

            TitleEntry.IsReadOnly = false;
            DubbingEntry.IsReadOnly = false;
            LastWatchedSeriesEntry.IsReadOnly = false;
            LastWatchedSeasonEntry.IsReadOnly = false;
            NextEpisodeReleaseDateEntry.IsReadOnly = false;
            WatchStatusEntry.IsReadOnly = false;
         

        }

        private void SaveChanges()
        {
            isEditing = false;
            EditButton.Text = "��������";
            CancelButton.IsVisible = false; // ������ ������ "������"

            // ����������� ���� �����
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = true;
            LinkLabel.IsVisible = true;
            DateAddedLabel.IsVisible = true;
            DateAddedEntry.IsVisible = true;
            SeriesChangeDateLabel.IsVisible = true;
            SeriesChangeDateEntry.IsVisible = true;

            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;


            TypeEntry.IsVisible = true;
            TypeLabel.IsVisible = true;
            WatchStatusEntry.IsVisible = true; 
            WatchStatusLabel.IsVisible = true;

            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            NextEpisodeReleaseDateEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
          


            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // ������� ��������� ������� ���� ������
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);
            content = databaseService.GetContentById(content.Id);
            content.Title = TitleEntry.Text;
            content.Type = TypePicker.SelectedItem.ToString();
            content.Dubbing=  DubbingEntry.Text;
            if (LastWatchedSeriesEntry.Text != content.LastWatchedSeries.ToString() || LastWatchedSeasonEntry.Text != content.LastWatchedSeason.ToString())
            {
                DateTime currentDate = DateTime.UtcNow;
                DateTime newDate = currentDate.AddHours(+3);
                content.SeriesChangeDate = newDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
            content.SeriesChangeDate = SeriesChangeDateEntry.Text;

            }
            content.Link = LinkEntry.Text;
            content.LastWatchedSeries =  int.Parse(LastWatchedSeriesEntry.Text);
            content.LastWatchedSeason = int.Parse(LastWatchedSeasonEntry.Text);

            content.NextEpisodeReleaseDate = NextEpisodeReleaseDateEntry.Text;
            content.WatchStatus = WatchStatusPicker.SelectedItem.ToString() ;
            content.DateAdded = DateAddedEntry.Text;
            databaseService.UpdateContent(content);
            databaseService.CloseConnection();


            // �������� ������ � ��
            // ��� ��� ��� ���������� ������ � ��
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            // �������� ��������� � ������������� �� ������ ��������������
            isEditing = false;
            EditButton.Text = "��������";
            CancelButton.IsVisible = false; // ������ ������ "������"

            // �������� ���� � ���� ������
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // ������� ��������� ������� ���� ������
            DatabaseServiceContent databaseService = new DatabaseServiceContent(databasePath);

            // �������� ������ �� ���� ������ �� ID
            content = databaseService.GetContentById(content.Id);

            // ��������� ������� ������ � ������� content
            if (content != null)
            {
                // ��������� ���� ����� ������� �� ������� content
                TitleEntry.Text = content.Title;
                TypeEntry.Text = content.Type;
                DubbingEntry.Text = content.Dubbing;
                LastWatchedSeriesEntry.Text = content.LastWatchedSeries.ToString();
                LastWatchedSeasonEntry.Text = content.LastWatchedSeason.ToString();
                NextEpisodeReleaseDateEntry.Text = content.NextEpisodeReleaseDate;
                WatchStatusEntry.Text = content.WatchStatus;
                DateAddedEntry.Text = content.DateAdded;
            }

            // ��������� ���� �����
            LinkSecondLabel.IsVisible = false;
            LinkEntry.IsVisible = false;
            LinkEntry.IsReadOnly = true;
            LinkLabel.IsVisible = true;
            DateAddedLabel.IsVisible = true;
            DateAddedEntry.IsVisible = true;
            SeriesChangeDateLabel.IsVisible = true;
            SeriesChangeDateEntry.IsVisible = true;
            TypePicker.IsVisible = false;
            WatchStatusPicker.IsVisible = false;

            TypeEntry.IsVisible = true;
            TypeLabel.IsVisible = true;

            WatchStatusEntry.IsVisible = true;
            WatchStatusLabel.IsVisible = true;

            TitleEntry.IsReadOnly = true;
            DubbingEntry.IsReadOnly = true;
            LastWatchedSeriesEntry.IsReadOnly = true;
            LastWatchedSeasonEntry.IsReadOnly = true;
            NextEpisodeReleaseDateEntry.IsReadOnly = true;
            WatchStatusEntry.IsReadOnly = true;
           

        }



    }
}