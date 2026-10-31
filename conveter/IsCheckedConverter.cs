using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace FileTransfer.conveter
{
    public class IsCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;

            if (isChecked) 
            {
                return new SolidColorBrush(Colors.Pink);
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#69D3FE"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
