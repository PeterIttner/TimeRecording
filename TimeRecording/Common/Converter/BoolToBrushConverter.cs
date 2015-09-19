using System;
using System.Collections.Generic;
using System.Drawing;using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TimeRecording.Common.Converter
{

    [ValueConversion(typeof(bool), typeof(Brushes))]
    public class BoolToBrushConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rgb = Brushes.Yellow;
            if (value != null)
            {
                bool isOk = false;
                if (bool.TryParse(value.ToString(), out isOk))
                {
                    if (isOk)
                    {
                        rgb = Brushes.Green;
                    }
                    else
                    {
                        rgb = Brushes.DarkRed;
                    }
                }
            }
            return rgb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not Supported
            return null;
        }
    }

}
