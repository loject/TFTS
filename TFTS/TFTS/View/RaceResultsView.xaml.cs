using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFTS.Model;
using TFTS.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceResultsView : ContentPage
    {
        public RaceResultsView(RaceViewModel race)
        {
            InitializeComponent();
            BindingContext = race;

            /* TODO: Delete this sort */
            List<RunnerViewModel> tmp = new List<RunnerViewModel>(race.Runners.ToList<RunnerViewModel>());
            tmp.Sort((RunnerViewModel a, RunnerViewModel b) => 
            {
                if (a.LapsOvercome == 0 && b.LapsOvercome == 0)
                    return 0;
                if (a.LapsOvercome != b.LapsOvercome)
                    return (a.LapsOvercome > a.LapsOvercome) ? -1 : 1;
                if (a.Runner.Laps[^1].Time != b.Runner.Laps[^1].Time)
                    return (a.Runner.Laps[^1].Time > b.Runner.Laps[^1].Time) ? 1 : -1;
                return 0;
            });


            int CeilLapsCount = (int)Math.Ceiling(race.LapsCount);
            grid.Children.Add(GetLabelWithText("Имя"), 0, 0);
            //for (int i = 1; i <= race.LapsCount; ++i) grid.Children.Add(GetLabelWithText("круг №" + i.ToString()), i, 0);
            if (SettingsModel.FirstLapAlwaysFull)
            {
                for (float i = 1; i < CeilLapsCount; ++i)
                    grid.Children.Add(GetLabelWithText($"{i}-й круг"), (int)i, 0);
                grid.Children.Add(GetLabelWithText($"{race.LapsCount}-й круг"), CeilLapsCount, 0);
            }
            else
            {
                var FirstLapRatio = (race.Distance % race.LapLength == 0) ? 1 : (race.Distance % race.LapLength) / race.LapLength;
                for (float i = FirstLapRatio; i <= CeilLapsCount; ++i)
                    grid.Children.Add(GetLabelWithText($"{i}-й круг"), (int)Math.Ceiling(i), 0);
            }
            grid.Children.Add(GetLabelWithText("Общее время"), CeilLapsCount + 1, 0);


            for (int i = 1; i <= tmp.Count; ++i)
            {
                string positionOnFinish = "Н/Ф";
                if (tmp[i - 1].IsFinished)
                {
                    positionOnFinish = tmp[i - 1].Runner.Laps[CeilLapsCount - 1].Position.ToString();
                }
                grid.Children.Add(GetLabelWithText(tmp[i - 1].Name + "(" + positionOnFinish + ")"), 0, i);
                for (int j = 1; j <= CeilLapsCount; ++j)
                {
                    if (tmp[i - 1].Runner.Laps.Count >= j)
                        grid.Children.Add(GetLabelWithText(tmp[i - 1].Runner.Laps[j - 1].TimeStr + "(" + tmp[i - 1].Runner.Laps[j - 1].Position + "-й)"), j, i);
                    else
                        grid.Children.Add(GetLabelWithText("-"), j, i);
                }
                grid.Children.Add(GetLabelWithText(txt: Utils.getStringFromTimeSpan(tmp[i - 1].TotalTime)),
                                  CeilLapsCount + 1, i);
            }
        }

        Label GetLabelWithText(string txt)
        {
            return new Label
            {
                Text = txt
            };
        }
    }

    public class DateTimeToStrCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}