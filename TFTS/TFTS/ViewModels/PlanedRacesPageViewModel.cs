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
        public ICommand AddRaceToPlanCommand { get => new Command<RaceModel>(Race =>
        {
            var RaceSetUpVM = new RaceSetUpViewModel();
            RaceSetUpVM.ActionAfterEditing = RaceSetUpVM =>
            {
                var RaceVM = RaceSetUpVM.GetRaceViewModel();
                App.PlanDatabase.SaveRaceToRacePlan(RaceVM.Race);
            };
            var RaceSetUpPage = new RaceSetUpView();
            RaceSetUpPage.BindingContext = RaceSetUpVM;
            Application.Current.MainPage.Navigation.PushAsync(RaceSetUpPage);
        }); }
        public ICommand GoToRacePage
        {
            get => new Command(() =>
            {
                /* incoming */
            });
        }
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
