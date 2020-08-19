using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TFTS.misc;
using TFTS.Models;
using TFTS.ViewModels;
using Xamarin.Forms;

namespace TFTS.Converters
{
    class ConcatRunnersNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            try
            {
                var list = value as SortableObservableCollection<RunnerModel>;
                if (list.Count == 0) return "Без спортсменов?!";
                return String.Join(", ", list.Select(r => r.Name));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - " + e.Message);
            }
            return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
