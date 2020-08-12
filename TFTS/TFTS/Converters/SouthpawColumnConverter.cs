using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TFTS.Model;
using Xamarin.Forms;

namespace TFTS.Converters
{
    class SouthpawColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const int ColumnCount = 5;
            try
            {
                int inValue = 0;
                if (parameter.GetType() == typeof(int)) inValue = (int)parameter;
                if (parameter.GetType() == typeof(float)) inValue = (int)((float)parameter);
                if (parameter.GetType() == typeof(string)) inValue = int.Parse((string)parameter);
                int res;
                if (SettingsModel.LeftHandMode)
                {
                    res = inValue;
                }
                else
                {
                    res = inValue - 1;
                    if (res < 0) res = ColumnCount - 1;
                }
                return res;
            }
            catch
            {
                /* TODO: log error */
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
