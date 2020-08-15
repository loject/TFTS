using TFTS.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceSetUpView : ContentPage
    {
        public RaceSetUpView(RaceSetUpViewModel race)
        {
            BindingContext = race;
            InitializeComponent();
        }
    }
}