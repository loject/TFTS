using PropertyChanged;
using System.Windows.Input;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainPageViewModel
    {
        public MainPageViewModel()
        { }
        public ICommand GoToRaceCommand
        {
            get => new Command(() =>
            {
                var RaceSetUpVM = new RaceSetUpViewModel();
                var RaceSetUpPage = new RaceSetUpView();
                RaceSetUpPage.BindingContext = RaceSetUpVM;
                Application.Current.MainPage.Navigation.PushAsync(RaceSetUpPage);
            });
        }
        public ICommand GoToTobataCommand
        {
            get => new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.NavigationStack[^1].DisplayAlert("Warning", message: "Currently not implemented", cancel: "cancel");
            });
        }
        public ICommand GoToHistoryCommand
        {
            get => new Command(() =>
            {
                var HistoryVM = new HistoryViewModel();
                var HistoryPage = new HistoryPageView();
                HistoryPage.BindingContext = HistoryVM;
                Application.Current.MainPage.Navigation.PushAsync(HistoryPage);
            });
        }
        public ICommand GoToSettingsCommand
        {
            get => new Command(() =>
            {
                new SettingsViewModel(Application.Current.MainPage.Navigation);
            });
        }
    }
}
