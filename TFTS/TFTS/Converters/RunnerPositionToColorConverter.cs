using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TFTS.Model;
using TFTS.ViewModel;
using Xamarin.Forms;

namespace TFTS.Converters
{
    class RunnerPositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!SettingsModel.HighlightFinishers) return Color.Default;
            if (value == null) return Color.Default;
            try
            {
                float RunnerPosition = (float)value;
                if (RunnerPosition <= 0) return Color.Red;
                if (RunnerPosition <= 1) return Color.Yellow;
                return Color.Default;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - " + e.Message);
            }
            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
