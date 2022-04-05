using System;
using System.Globalization;
using System.Windows.Data;

namespace ItsyBitsOptitrackCapture
{
    internal class CircleCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String param = parameter as String;
            if (value is int)
            {
                int angle = (int)value;
                double angleDeg = angle * (Math.PI / 180.0);

                int range = 250;

                if (param == "cos")
                {
                    var cos = Math.Cos(angleDeg - Math.PI / 2);
                    return range * cos + 180;
                }
                else
                {
                    var sin = Math.Sin(angleDeg - Math.PI / 2);
                    double offset = 0;
                    if (angleDeg <= 10 || angleDeg >= 350)
                    {
                        offset = 0;
                    }
                    return range * sin + range + offset - 100;
                }

            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
