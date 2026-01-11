using System;
using System.Globalization;
using System.Windows.Data;

namespace GuildGame.UI.Converters;

public class AbsValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
            return Math.Abs(intValue);
        if (value is double doubleValue)
            return Math.Abs(doubleValue);
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
