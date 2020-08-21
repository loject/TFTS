using PropertyChanged;
using System.Windows.Input;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainPageViewModel
    {
        public INavigation Navigation { get; private set; }
        public MainPageViewModel(INavigation navigation)
        {
            Navigation = navigation;
        }
        public ICommand GoToRaceCommand
        {
            get => new Command(() =>
            {
                new RaceSetUpViewModel(Navigation);
            });
        }
        public ICommand GoToTobataCommand
        {
            get => new Command(async () =>
            {
                await Navigation.NavigationStack[^1].DisplayAlert("Warning", message: "Currently not implemented", cancel: "cancel");
            });
        }
        public ICommand GoToHistoryCommand
        {
            get => new Command(() =>
            {
                new HistoryViewModel(Navigation);
            });
        }
        public ICommand GoToSettingsCommand
        {
            get => new Command(() =>
            {
                new SettingsViewModel(Navigation);
            });
        }
    }
}
