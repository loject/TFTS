using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        public List<RaceModel> Races 
        {
            get
            {
                var GetRaceHistoryTask = App.HistoryDatabase.GetRaceHistory();
                GetRaceHistoryTask.Wait();
                var resList = GetRaceHistoryTask.Result;
                resList.Reverse();
                return resList;
            }
        } 
        public HistoryViewModel()
        { }
        #region Commands
        public ICommand ShowResultPageCommand { get => new Command<RaceModel>(Race => { Application.Current.MainPage.Navigation.PushModalAsync(new RaceResultsView(Race)); }); }
        public ICommand ClearHistoryCommand 
        { 
            get => new Command(() => 
            {
                var ClearHistoryTask = App.HistoryDatabase.ClearHistory();
                ClearHistoryTask.Wait();
                OnPropertyChanged(nameof(Races));
            }); 
        }
        #endregion
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
