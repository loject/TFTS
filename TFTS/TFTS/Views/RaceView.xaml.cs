using TFTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceView : ContentPage
    {
        public RaceView(RaceViewModel race)
        {
            InitializeComponent();
            BindingContext = race;
        }
    }
}