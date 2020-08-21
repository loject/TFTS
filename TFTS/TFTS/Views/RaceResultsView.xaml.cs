using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TFTS.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceResultsView : ContentPage
    {
        public RaceResultsView(RaceModel race)
        {
            InitializeComponent();
            BindingContext = race;
            if (race == null) return;

            /* TOOD: move to race view model */
            int CeilLapsCount = (int)Math.Ceiling(race.Distance / race.LapLength);
            grid.Children.Add(GetLabelWithText("Имя"), 0, 0);
            //for (int i = 1; i <= race.LapsCount; ++i) grid.Children.Add(GetLabelWithText("круг №" + i.ToString()), i, 0);
            if (SettingsModel.FirstLapAlwaysFull)
            {
                for (float i = 1; i < CeilLapsCount; ++i)
                    grid.Children.Add(GetLabelWithText($"{i}-й круг"), (int)i, 0);
                grid.Children.Add(GetLabelWithText($"{race.Distance / race.LapLength}-й круг"), CeilLapsCount, 0);
            }
            else
            {
                var FirstLapRatio = (race.Distance % race.LapLength == 0) ? 1 : (race.Distance % race.LapLength) / race.LapLength;
                for (float i = FirstLapRatio; i <= CeilLapsCount; ++i)
                    grid.Children.Add(GetLabelWithText($"{i}-й круг"), (int)Math.Ceiling(i), 0);
            }
            grid.Children.Add(GetLabelWithText("Общее время"), CeilLapsCount + 1, 0);


            for (int i = 1; i <= race.Runners.Count; ++i)
            {
                string positionOnFinish = "Н/Ф";
                if (race.Runners[i - 1].IsFinished)
                {
                    positionOnFinish = race.Runners[i - 1].Laps[^1].Position.ToString();
                }
                grid.Children.Add(GetLabelWithText(race.Runners[i - 1].Name + "(" + positionOnFinish + ")"), 0, i);
                for (int j = 1; j <= CeilLapsCount; ++j)
                {
                    if (race.Runners[i - 1].Laps.Count >= j)
                        grid.Children.Add(GetLabelWithText(race.Runners[i - 1].Laps[j - 1].TimeStr + "(" + race.Runners[i - 1].Laps[j - 1].Position + "-й)"), j, i);
                    else
                        grid.Children.Add(GetLabelWithText("-"), j, i);
                }
                grid.Children.Add(GetLabelWithText(txt: Utils.getStringFromTimeSpan(race.Runners[i - 1].TotalTime)),
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

    /* TODO: move converter from here */
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