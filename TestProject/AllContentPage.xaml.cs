using DataContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace TestProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllContentPage : ContentPage
    {
        private string Choise;
        public AllContentPage(string userChoise)
        {
            InitializeComponent();
            Choise = userChoise;

            // �������� ���� � ���� ������
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // �������� ������ ���� ������ � �������
            var databaseService = new DatabaseServiceContent(databasePath);
            databaseService.CreateTables();

            // ���������� ��������� �������� � ����������� �� ������ ������������
            SetPageTitle();

            // �������� ������� � ����������� �� ������ ������������ ��� ��� ��������
            List<Content> content = GetContent(databaseService);

            // ��������� ������ � ������
            ContentListView.ItemsSource = content;
        }

        private string AppDataDirectory => FileSystem.AppDataDirectory;


        private void SetPageTitle()
        {
            switch (Choise)
            {
                case "All":
                    MainLabel.Text = "��� ���� �������";
                    return;
                case "Movies":
                    MainLabel.Text = "���� ������";
                    return;
                case "Series":
                    MainLabel.Text = "���� �������";
                    return;
                case "Anime":
                    MainLabel.Text = "���� �����";
                    return;
                case "Other":
                    MainLabel.Text = "������ �������";
                    return;
                case "Doram":
                    MainLabel.Text = "���� ������";
                    return;
                case "Mult":
                    MainLabel.Text = "���� ������������";
                    return;
                case "Documental":
                    MainLabel.Text = "���� �������������� ��������";
                    return;

            }
        }

        private List<Content> GetContent(DatabaseServiceContent databaseService)
        {
            if (Choise == "Viewed" || Choise == "Progress" || Choise == "NotStarted")
            {
                return Choise switch
                {
                    "Progress" => databaseService.GetAllContent().Where(c => c.WatchStatus == "������").ToList(),
                    "NotStarted" => databaseService.GetAllContent().Where(c => c.WatchStatus == "�� �������").ToList(),
                    "Viewed" => databaseService.GetAllContent().Where(c => c.WatchStatus == "�����������").ToList(),
                    _ => databaseService.GetAllContent(),
                };

            }
            else
            {
                return Choise switch { 
                "All" => databaseService.GetAllContent(),
                    "Movies" => databaseService.GetAllContent().Where(c => c.Type == "�����").ToList(),
                    "Series" => databaseService.GetAllContent().Where(c => c.Type == "������").ToList(),
                    "Anime" => databaseService.GetAllContent().Where(c => c.Type == "�����").ToList(),
                    "Other" => databaseService.GetAllContent().Where(c => c.Type == "������").ToList(),
                    "Doram" => databaseService.GetAllContent().Where(c => c.Type == "������").ToList(),
                    "Mult" => databaseService.GetAllContent().Where(c => c.Type == "�����������").ToList(),
                    "Documental" => databaseService.GetAllContent().Where(c => c.Type == "������������").ToList(),

                    _ => databaseService.GetAllContent(),
                };
        }
    }
            
        

        private async void ContentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            // �������� ��������� ������� ��������
            Content selectedContent = (Content)e.Item;

            // �������� ����� �������� ��� ����������� ��������� ����������
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);

            // ��������� �� ����� ��������
            await Navigation.PushAsync(viewContentPage);

            // �������� ����� �������� � ListView
            ((ListView)sender).SelectedItem = null;
        }

    }
}
