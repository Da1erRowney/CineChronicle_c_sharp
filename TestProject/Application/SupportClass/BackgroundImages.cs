namespace CineChronicle.Application.SupportClass
{

    public static class BackgroundImages
    {
        public static string[] GetImageOfTheme(AppTheme appTheme, AppTheme systemTheme)
        {
            return appTheme switch
            {
                AppTheme.Dark => DarkThemeImages,
                AppTheme.Light => LightThemeImages,
                _ => systemTheme switch
                {
                    AppTheme.Dark => DarkThemeImages,
                    AppTheme.Light => LightThemeImages,
                    _ => DarkThemeImages // fallback если и системная тема не определена
                }
            };
        }

        private static readonly string[] DarkThemeImages =
        {
        "gradientsecond",
        "gradientthird",
        "paper",
        "gradientnewfisrt",
        "gradientnewsecond",
        "gradientnewthird",
        "gradientnewfourth",
        "gradientnewfive"
    };

        private static readonly string[] LightThemeImages =
        {
        "first",
        "second",
        "third",
        "fourth",
        "five",
        "six",
        "seven",
        "eight",
        "nine"
    };
    }

}

//"gradient",             // Реализм
//"gradienteight",        // Реализм
//"gradientfive",         // Реализм
//"gradientfouth",        // Реализм
//"gradientseven",        // Реализм
//"gradientsix",          // Реализм
            
