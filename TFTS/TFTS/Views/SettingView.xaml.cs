using TFTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingView : ContentPage
    {
        public SettingView(SettingsViewModel settingsViewModel)
        {
            BindingContext = settingsViewModel;
            InitializeComponent();
        }
    }
}