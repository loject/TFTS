using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFTS.ViewModel;
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
        }
    }
}