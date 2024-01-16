using System.Globalization;

namespace HotdogOrNot.Converters;

public class BytesToImageConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        var bytes = (byte[])value;
        var stream = new MemoryStream(bytes);

        return ImageSource.FromStream(() => stream);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

