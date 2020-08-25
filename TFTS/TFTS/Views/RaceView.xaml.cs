using Android.Content;
using TFTS.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TFTS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RaceView : ContentPage
    {
        public RaceViewModel RaceViewModel { get; set; }
        public RacePageViewModel RacePageViewModel { get; set; }
        public RaceView()
        {
            InitializeComponent();
            BindingContext = this;
            /* add exit listener for save race to db*/
            (Application.Current.MainPage as NavigationPage).Popped += SaveRaceToDB;
        }
        public void SaveRaceToDB(object sender, NavigationEventArgs args)
        {
            if (args.Page == this)
            {
                (BindingContext as RaceViewModel).SaveRaceToDB.Execute(null);
                (Application.Current.MainPage as NavigationPage).Popped -= SaveRaceToDB;
            }
        }
    }
}