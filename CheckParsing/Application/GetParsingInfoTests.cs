using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CineChronicle.Application.Tests
{
    [TestClass()]
    public class GetParsingInfoTests
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

        private GetParsingInfo parser = new();

        [TestMethod()]
        public void GetInfoTest()
        {
            // Arrange
            PushParser(ANIME, "Магическая битва", AG);

            // Act
            var description = parser?.Description;
            var image = parser?.Image;
            var youTube = parser?.YouTube;
            var nextEpisodeReleaseDate = parser?.NextEpisodeReleaseDate;
            var countLabel = parser?.CountLabel;

            // Assert
            Assert.IsNotNull(description, "Description should not be null");
            Assert.IsFalse(string.IsNullOrEmpty(description), "Description should not be empty");

            Assert.IsNotNull(image, "Image should not be null");
            Assert.IsFalse(string.IsNullOrEmpty(image), "Image should not be empty");

            Assert.IsNotNull(youTube, "YouTube link should not be null");
            Assert.IsTrue(youTube.StartsWith("https://"), "YouTube link should be a valid URL");

            Assert.IsNotNull(nextEpisodeReleaseDate, "NextEpisodeReleaseDate should not be null");
            Assert.IsFalse(string.IsNullOrEmpty(nextEpisodeReleaseDate), "NextEpisodeReleaseDate should not be empty");

            Assert.IsNotNull(countLabel, "CountLabel should not be null");
            Assert.IsFalse(string.IsNullOrEmpty(countLabel), "CountLabel should not be empty");
        }

        private void PushParser(string type, string title, string typePars)
        {
            parser?.GetInfo(title, type, typePars, false);
            parser?.GetInfo(title, type, typePars, true);
            parser?.GetInfo(title, type, YT, false);
            parser?.GetInfo(title, type, DE, false);
        }
    }
}