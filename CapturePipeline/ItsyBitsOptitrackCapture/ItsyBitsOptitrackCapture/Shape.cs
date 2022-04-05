using System;
using System.Windows.Media.Imaging;

namespace ItsyBitsOptitrackCapture
{
    public class Shape
    {
        public String Name { get; set; }
        public bool Selected { get; set; }
        public String Size { get; set; }
        public BitmapImage Image { get; set; }
    }
}
