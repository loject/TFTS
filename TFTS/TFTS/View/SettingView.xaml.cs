using TFTS.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingView : ContentPage
    {
        public SettingView()
        {
            BindingContext = new SettingsViewModel();
            InitializeComponent();
        }
    }
}