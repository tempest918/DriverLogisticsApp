using System.Globalization;

namespace DriverLogisticsApp.Converters
{

    /// <summary>
    /// checks if the value is null and returns true if it is, false otherwise
    /// </summary>
    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return true (visible) if the SelectedItem is null
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
