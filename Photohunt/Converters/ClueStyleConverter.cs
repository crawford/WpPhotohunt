using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace Photohunt.Converters
{
    public class ClueStyleConverter : IValueConverter
    {
        public Style ClueStyle { get; set; }
        public Style BonusStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return BonusStyle;
            else
                return ClueStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
