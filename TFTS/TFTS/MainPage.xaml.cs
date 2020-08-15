using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TFTS.View;
using TFTS.ViewModel;
using Xamarin.Forms;

namespace TFTS
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void BtnTempo_Clicked(object sender, EventArgs e)
        {
            new RaceSetUpViewModel(Navigation);
        }

        private async void BtnWork_ClickedAsync(object sender, EventArgs e)
        {
            await DisplayAlert("Warning", message: "Currently not implemented", cancel: "cancel");
        }

        private void BtnSettings_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingView());
        }
    }
}
