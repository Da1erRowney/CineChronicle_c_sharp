using System.Globalization;

namespace CineChronicle.Application.AllContentPages
{
    public class CountToSpanConverter : IValueConverter
    {
        private static readonly Random _random = new Random();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                if (count <= 2)
                    return 1;  // 1 или 2 элемента → в одну строку
                else if (count < 8)
                    return 2;      // от 3 до 6 → по 2 в строке
                else if(count == 8)
                {
                    return _random.Next(0, 2) == 0 ? 2 : 4; // Рандомно 2 или 4
                }
                else
                    return 3;      // >6 → по 3 в строке
            }
            return 2; // По умолчанию, если value не int
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Обратное преобразование не требуется
        }
    }
}