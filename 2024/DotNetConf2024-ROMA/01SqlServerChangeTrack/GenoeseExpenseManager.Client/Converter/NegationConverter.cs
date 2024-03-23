using System.Globalization;

namespace GenoeseExpenseManager.Client;

public class NegationConverter : IValueConverter
{

    public NegationConverter()
    {

    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return false;
        return !(bool)value;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
