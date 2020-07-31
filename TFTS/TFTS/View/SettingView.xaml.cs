using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingView : ContentPage
    {
        public SettingView()
        {
            BindingContext = this;
            InitializeComponent();
        }

        public bool LapDoneBySwipe
        {
            get => Preferences.Get(nameof(LapDoneBySwipe), false);
            set
            {
                Preferences.Set(nameof(LapDoneBySwipe), value);
                OnPropertyChanged(nameof(LapDoneBySwipe));
            }
        }
        public bool FirstLapAlwaysFull
        {
            get => Preferences.Get(nameof(FirstLapAlwaysFull), true);
            set
            {
                Preferences.Set(nameof(FirstLapAlwaysFull), value);
                OnPropertyChanged(nameof(FirstLapAlwaysFull));
            }
        }
    }
}