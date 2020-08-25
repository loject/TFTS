using PropertyChanged;
using System.Windows.Input;
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
                new RaceSetUpViewModel(Application.Current.MainPage.Navigation);
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
                new HistoryViewModel(Application.Current.MainPage.Navigation);
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
