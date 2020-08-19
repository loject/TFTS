using System.Collections.Generic;
using System.ComponentModel;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation;
        public List<RaceModel> Races { get => App.Database.GetRaceHistory(); } 
        public HistoryViewModel(INavigation navigation = null)
        {
            Navigation = navigation;
            Navigation?.PushAsync(new HistoryPageView(this));
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
