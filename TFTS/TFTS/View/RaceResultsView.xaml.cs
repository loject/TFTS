using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFTS.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceResultsView : ContentPage
    {
        public RaceResultsView(Race race)
        {
            InitializeComponent();
            BindingContext = race;

            List<Runner> tmp = new List<Runner>(race.Runners.ToList<Runner>());
            tmp.Sort((Runner a, Runner b) => 
            {
                if (a.LapsOvercome == 0 && b.LapsOvercome == 0)
                    return 0;
                if (a.LapsOvercome != b.LapsOvercome)
                    return (a.LapsOvercome > a.LapsOvercome) ? -1 : 1;
                if (a.Laps[a.Laps.Count - 1].Time != b.Laps[b.Laps.Count - 1].Time)
                    return (a.Laps[a.Laps.Count - 1].Time > b.Laps[b.Laps.Count - 1].Time) ? 1 : -1;
                return 0;
            });
            grid.Children.Add(GetLabelWithText("Имя"), 0, 0);
            for (int i = 1; i <= race.LapsCount; ++i) grid.Children.Add(GetLabelWithText("круг №" + i.ToString()), i, 0);
            grid.Children.Add(GetLabelWithText("Общее время"), (int)race.LapsCount + 1, 0);
            for (int i = 1; i <= tmp.Count; ++i)
            {
                string positionOnFinish = "Н/Ф";
                if (tmp[i - 1].Laps.Count >= race.LapsCount) positionOnFinish = tmp[i - 1].Laps[(int)race.LapsCount - 1].Position.ToString();
                grid.Children.Add(GetLabelWithText(tmp[i - 1].Name + "(" + positionOnFinish + ")"), 0, i);
                for (int j = 1; j <= race.LapsCount; ++j)
                {
                    if (tmp[i - 1].Laps.Count >= j)
                        grid.Children.Add(GetLabelWithText(tmp[i - 1].Laps[j - 1].TimeStr + "(" + tmp[i - 1].Laps[j - 1].Position + "-й)"), j, i);
                    else
                        grid.Children.Add(GetLabelWithText("-"), j, i);
                }
                grid.Children.Add(GetLabelWithText(Utils.getStringFromTimeSpan(tmp[i - 1].TotalTime)), (int)race.LapsCount + 1, i);
            }
        }

        Label GetLabelWithText(string txt)
        {
            Label label = new Label();
            label.Text = txt;
            return label;
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