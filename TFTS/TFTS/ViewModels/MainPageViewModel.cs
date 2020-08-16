using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
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
        public ICommand GoToSettingsCommand
        {
            get => new Command(async () =>
            {
                new SettingsViewModel(Navigation);
            });
        }
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
