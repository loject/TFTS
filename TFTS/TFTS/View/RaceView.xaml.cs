using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFTS.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceView : ContentPage
    {
        public RaceView(Race race)
        {
            InitializeComponent();
            BindingContext = race;

            //var dataTemplate = new DataTemplate (() => 
            //{
            //    Xamarin.Forms.ControlTemplate tmp = (Xamarin.Forms.ControlTemplate)Resources["LapDoneBySwipeOn"];
            //    Console.WriteLine(tmp.GetType());
            //    Xamarin.Forms.View view = tmp.;
            //    ViewCell viewCell = new ViewCell { View = tmp.LoadFromXaml() };
            //    return viewCell; 
            //});
            //RunnersList.ItemTemplate = dataTemplate;
        }
    }
}