using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ParserTest.GetParsingInfo;

namespace ParserTest
{
    public class Program
    {
        #region [Constants]
        private const string WIK = "Википедия";
        private const string KO = "Kinogo";
        private const string JS = "Jutsu";
        private const string AG = "AnimeGo";
        private const string PG = "PremierGo";
        private const string YT = "YouTube";
        private const string LF = "LordsFilm";
        private const string DE = "DateExit";

        private const string ANIME = "Аниме";
        private const string FILM = "Фильм";
        private const string SERIAL = "Сериал";
        private const string DORAMA = "Дорама";
        private const string OTHER = "Прочее";
        private const string CARTOON = "Мультсериал";
        #endregion

        public GetParsingInfo parser = new GetParsingInfo();

        public static void Main(string[] args)
        {
            var program = new Program();
            program.GetInfoTest();
        }

        public async void GetInfoTest()
        {
            parser.GetUniqueVideoIds("Магическая битва", "Аниме");
            Console.ReadKey();
            // Arrange
            //PushParser(ANIME, "Магическая битва", AG);

            // Act


            //Console.OutputEncoding = System.Text.Encoding.UTF8;

            //while (true)
            //{
            //    Console.Clear();
            //    Console.WriteLine("╔══════════════════════════════════╗");
            //    Console.WriteLine("║        ПАРСЕР ДАННЫХ v1.0         ║");
            //    Console.WriteLine("╠══════════════════════════════════╣");
            //    Console.WriteLine("║ Выберите источник:               ║");
            //    Console.WriteLine("║ 1. Википедия                     ║");
            //    Console.WriteLine("║ 2. Kinogo                        ║");
            //    Console.WriteLine("║ 3. Jutsu                         ║");
            //    Console.WriteLine("║ 4. AnimeGo                       ║");
            //    Console.WriteLine("║ 5. PremierGo                     ║");
            //    Console.WriteLine("║ 6. YouTube                       ║");
            //    Console.WriteLine("║ 7. LordsFilm                     ║");
            //    Console.WriteLine("║ 8. DateExit                      ║");
            //    Console.WriteLine("║                                  ║");
            //    Console.WriteLine("║ z. Выход                         ║");
            //    Console.WriteLine("╚══════════════════════════════════╝");
            //    Console.Write("Ваш выбор: ");

            //    string sourceChoice = Console.ReadLine();

            //    if (sourceChoice.ToLower() == "z")
            //    {
            //        Console.WriteLine("Завершение работы...");
            //        return;
            //    }

            //    string source = sourceChoice switch
            //    {
            //        "1" => "Википедия",
            //        "2" => "Kinogo",
            //        "3" => "Jutsu",
            //        "4" => "AnimeGo",
            //        "5" => "PremierGo",
            //        "6" => "YouTube",
            //        "7" => "LordsFilm",
            //        "8" => "DateExit",
            //        _ => null
            //    };

            //    if (source == null)
            //    {
            //        Console.WriteLine("Неверный выбор! Нажмите любую клавишу...");
            //        Console.ReadKey();
            //        continue;
            //    }

            //    Console.Clear();
            //    Console.WriteLine("╔══════════════════════════════════╗");
            //    Console.WriteLine("║        ВЫБЕРИТЕ ТИП КОНТЕНТА      ║");
            //    Console.WriteLine("╠══════════════════════════════════╣");
            //    Console.WriteLine("║ 1. Аниме                         ║");
            //    Console.WriteLine("║ 2. Фильм                         ║");
            //    Console.WriteLine("║ 3. Сериал                        ║");
            //    Console.WriteLine("║ 4. Дорама                        ║");
            //    Console.WriteLine("║ 5. Мультсериал                   ║");
            //    Console.WriteLine("║ 6. Прочее                        ║");
            //    Console.WriteLine("║                                  ║");
            //    Console.WriteLine("║ z. Назад                         ║");
            //    Console.WriteLine("╚══════════════════════════════════╝");
            //    Console.Write("Ваш выбор: ");

            //    string typeChoice = Console.ReadLine();

            //    if (typeChoice.ToLower() == "z")
            //        continue;

            //    string type = typeChoice switch
            //    {
            //        "1" => "Аниме",
            //        "2" => "Фильм",
            //        "3" => "Сериал",
            //        "4" => "Дорама",
            //        "5" => "Мультсериал",
            //        "6" => "Прочее",
            //        _ => null
            //    };

            //    if (type == null)
            //    {
            //        Console.WriteLine("Неверный выбор! Нажмите любую клавишу...");
            //        Console.ReadKey();
            //        continue;
            //    }

            //    Console.Clear();
            //    Console.WriteLine("╔══════════════════════════════════╗");
            //    Console.WriteLine("║        ВВЕДИТЕ НАЗВАНИЕ          ║");
            //    Console.WriteLine("╚══════════════════════════════════╝");
            //    Console.Write("> ");
            //    string title = Console.ReadLine();

            //    // Вызов парсера
            //    PushParser(type,title, source);

            //    //var description = parser?.Description;
            //    //var image = parser?.Image;
            //    //var youTube = parser?.YouTube;
            //    //var nextEpisodeReleaseDate = parser?.NextEpisodeReleaseDate;
            //    //var countLabel = parser?.CountLabel;

            //    //Console.WriteLine("Название: " + title);
            //    //Console.WriteLine("Описание: " + description);
            //    //Console.WriteLine("Картинка: " + image);
            //    //Console.WriteLine("Ссылка трейлера: " + youTube);
            //    //Console.WriteLine("Дата выхода эпизода: " + nextEpisodeReleaseDate);
            //    //Console.WriteLine("Количество: " + countLabel);


            //    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            //    Console.ReadKey();
            //}

        }

        private async Task PushParser(string type, string title, string typePars)
        {
            parser.DescriptionRead += HandleLoadDescription;
            parser.ImageRead += HandleLoadImage;
            parser.YouTubeRead += HandleLoadYouTube;
            parser.NextEpisodeReleaseDateRead += HandleLoadNextEpisode;
            parser.CountLabelRead += HandleLoadCount;
            parser.DateReleaseRead += HandleLoadDateRelease;

            try
            {
                var task1 = parser.GetInfo(title, type, typePars, false);
                var task2 = parser.GetInfo(title, type, typePars, true);
                //var task3 = parser.GetInfo(title, type, YT, false);
                //var task4 = parser.GetInfo(title, type, DE, false);

                await Task.WhenAll(task1, task2);

                await Task.Delay(100);
            }
            finally
            {
                parser.DescriptionRead -= HandleLoadDescription;
                parser.ImageRead -= HandleLoadImage;
                parser.YouTubeRead -= HandleLoadYouTube;
                parser.NextEpisodeReleaseDateRead -= HandleLoadNextEpisode;
                parser.CountLabelRead -= HandleLoadCount;
                parser.DateReleaseRead -= HandleLoadDateRelease;
            }
        }

        private static void HandleLoadDescription(GetParsingInfo.DescriptionResult result)
        {
            if (!string.IsNullOrEmpty(result.Description))
                Console.WriteLine($"Описание: {result.Description}");
        }
        private static void HandleLoadImage(GetParsingInfo.ImageResult result)
        {
            if (!string.IsNullOrEmpty(result.Image))
                Console.WriteLine($"Картинка: {result.Image}");
        }
        private static void HandleLoadYouTube(GetParsingInfo.YouTubeResult result)
        {
            if (!string.IsNullOrEmpty(result.YouTube))
                Console.WriteLine($"Ссылка на трейлер: {result.YouTube}");
        }
        private static void HandleLoadNextEpisode(GetParsingInfo.NextEpisodeReleaseDateResult result)
        {
            if (!string.IsNullOrEmpty(result.NextEpisodeReleaseDate))
                Console.WriteLine($"Следующий эпизод: {result.NextEpisodeReleaseDate}");
        }
        private static void HandleLoadCount(GetParsingInfo.CountLabelResult result)
        {
            if (!string.IsNullOrEmpty(result.CountLabel))
                Console.WriteLine($"Количество: {result.CountLabel}");
        }
        private static void HandleLoadDateRelease(GetParsingInfo.DateReleaseResult result)
        {
            if (!string.IsNullOrEmpty(result.DateRelease))
                Console.WriteLine($"Дата выхода: {result.DateRelease}");
        }

    }
}
