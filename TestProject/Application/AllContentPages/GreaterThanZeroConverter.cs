using System.Globalization;

namespace CineChronicle.Application.AllContentPages
{
    public class GreaterThanZeroConverter : IValueConverter
    {
        public static GreaterThanZeroConverter Instance { get; } = new GreaterThanZeroConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}