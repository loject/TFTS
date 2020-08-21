using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TFTS.Models;
using TFTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RunnerResultView : ContentPage
    {
        public RunnerViewModel Runner { get; }
        public string StartTime { get; }
        public string Distance { get; }
        public string About { get => $"{Runner.Name}.\nДистанция {Distance} метров.\nСтарт {StartTime}"; }
        public RunnerResultView(RunnerViewModel runner, string startTime, string distance)
        {
            InitializeComponent();
            Runner = runner;
            StartTime = startTime;
            Distance = distance;
            BindingContext = this;
        }
    }

    public class IndexFromListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return -1;
            try
            {
                var index = ((ListView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value) + 1;
                return index;
            }
            catch
            {
                return -1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TotalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return -1;
            try
            {
                List<Lap> laps = ((ListView)parameter).ItemsSource.Cast<Lap>().ToList();
                TimeSpan totalTime = TimeSpan.Zero;
                foreach(Lap lap in laps)
                {
                    if (lap.Equals(value))
                        break;
                    else
                        totalTime += lap.Time;
                }
                return Utils.getStringFromTimeSpan(totalTime);
            }
            catch
            {
                return -1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}