using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class PlanedRacesPageViewModel : INotifyPropertyChanged
    {
        public List<RaceModel> Races
        {
            get
            {
                var GetRacePlanTask = App.PlanDatabase.GetRacePlan();
                GetRacePlanTask.Wait();
                var resList = GetRacePlanTask.Result;
                resList.Reverse();
                return resList;
            }
        }
        public PlanedRacesPageViewModel()
        { }
        #region Commands
        public ICommand ShowResultPageCommand { get => new Command<RaceModel>(Race => 
        { 
            /* TODO: edit this */
            Application.Current.MainPage.Navigation.PushModalAsync(new RaceResultsView(Race)); 
        }); }
        public ICommand ClearAllPlansCommand
        {
            get => new Command(() =>
            {
                var ClearAllPlansTask = App.PlanDatabase.ClearAllPlans();
                ClearAllPlansTask.Wait();
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
