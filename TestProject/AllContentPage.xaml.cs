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

            // Получите путь к базе данных
            string databasePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "content.db");

            // Создайте сервис базы данных и таблицу
            var databaseService = new DatabaseServiceContent(databasePath);
            databaseService.CreateTables();

            // Установите заголовок страницы в зависимости от выбора пользователя
            SetPageTitle();

            // Получите контент в зависимости от выбора пользователя или все элементы
            List<Content> content = GetContent(databaseService);

            // Привяжите данные к списку
            ContentListView.ItemsSource = content;
        }

        private string AppDataDirectory => FileSystem.AppDataDirectory;


        private void SetPageTitle()
        {
            switch (Choise)
            {
                case "All":
                    MainLabel.Text = "Ваш весь контент";
                    return;
                case "Movies":
                    MainLabel.Text = "Ваши фильмы";
                    return;
                case "Series":
                    MainLabel.Text = "Ваши сериалы";
                    return;
                case "Anime":
                    MainLabel.Text = "Ваше аниме";
                    return;
                case "Other":
                    MainLabel.Text = "Прочий контент";
                    return;
                case "Doram":
                    MainLabel.Text = "Ваши дорамы";
                    return;
                case "Mult":
                    MainLabel.Text = "Ваши мультсериалы";
                    return;
                case "Documental":
                    MainLabel.Text = "Ваши документальные передачи";
                    return;

            }
        }

        private List<Content> GetContent(DatabaseServiceContent databaseService)
        {
            if (Choise == "Viewed" || Choise == "Progress" || Choise == "NotStarted")
            {
                return Choise switch
                {
                    "Progress" => databaseService.GetAllContent().Where(c => c.WatchStatus == "Смотрю").ToList(),
                    "NotStarted" => databaseService.GetAllContent().Where(c => c.WatchStatus == "Не начинал").ToList(),
                    "Viewed" => databaseService.GetAllContent().Where(c => c.WatchStatus == "Просмотрено").ToList(),
                    _ => databaseService.GetAllContent(),
                };

            }
            else
            {
                return Choise switch { 
                "All" => databaseService.GetAllContent(),
                    "Movies" => databaseService.GetAllContent().Where(c => c.Type == "Фильм").ToList(),
                    "Series" => databaseService.GetAllContent().Where(c => c.Type == "Сериал").ToList(),
                    "Anime" => databaseService.GetAllContent().Where(c => c.Type == "Аниме").ToList(),
                    "Other" => databaseService.GetAllContent().Where(c => c.Type == "Прочее").ToList(),
                    "Doram" => databaseService.GetAllContent().Where(c => c.Type == "Дорама").ToList(),
                    "Mult" => databaseService.GetAllContent().Where(c => c.Type == "Мультсериал").ToList(),
                    "Documental" => databaseService.GetAllContent().Where(c => c.Type == "Документалка").ToList(),

                    _ => databaseService.GetAllContent(),
                };
        }
    }
            
        

        private async void ContentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            // Получите выбранный элемент контента
            Content selectedContent = (Content)e.Item;

            // Создайте новую страницу для отображения подробной информации
            ViewContentPage viewContentPage = new ViewContentPage(selectedContent);

            // Перейдите на новую страницу
            await Navigation.PushAsync(viewContentPage);

            // Сбросьте выбор элемента в ListView
            ((ListView)sender).SelectedItem = null;
        }

    }
}
