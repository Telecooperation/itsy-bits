using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ItsyBitsOptitrackCapture
{
    internal class AngleToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int count = (int)value;

                if (count >= MainWindow.SAMPLE_THRESHOLD)
                {
                    return Brushes.White;
                }
                else if (count >= (MainWindow.SAMPLE_THRESHOLD / 2.0))
                {
                    return Brushes.Gold;
                }
            }
            return Brushes.DarkRed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
