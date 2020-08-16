using TFTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
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