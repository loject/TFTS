using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace TFTS.TMP
{
    class tmp : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tmp = value.GetType();
                Console.WriteLine(tmp);
                int input = 0;
                if (value is int) input = (int)value;
                if (value is string) input = int.Parse((string)value);
                if (value is float) input = (int)((float)value);
                input += 1;
                return input;
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message);
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
