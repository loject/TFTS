using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TFTS.misc;
using TFTS.Model;
using Xamarin.Forms;

namespace TFTS.ViewModel
{
    public class SimpleRunner : INotifyPropertyChanged
    {
        private string _name;
        private string _distance;
        public string Name 
        { 
            get => _name; 
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
    public class RaceSetUpViewModel : INotifyPropertyChanged
    {
        private string _distance = "1500";
        private string _lapLength = "200";

        private INavigation Navigation;
        public ObservableCollection<SimpleRunner> Runners { get; private set; }
        public RaceViewModel Race { get; private set; }
        public string Distance 
        { 
            get => _distance;  
            set
            {
                foreach (var i in Runners) if (i.Distance == _distance) i.Distance = value;
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }
        public string LapLength
        {
            get => _lapLength;
            set
            {
                _lapLength = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

        public RaceSetUpViewModel(INavigation navigation, RaceViewModel race = null)
        {
            if (race != null)
            {
                Race = race;
                Runners = new ObservableCollection<SimpleRunner>();
                foreach (var runner in race.Runners)
                    Runners.Add(new SimpleRunner { Name = runner.Name, Distance = runner.Runner.TotalDistance.ToString() });
            }
            else
            {
                Race = new RaceViewModel(navigation);
                Runners = new ObservableCollection<SimpleRunner>
                {
                    new SimpleRunner{ Name = "Runner", Distance = Distance.ToString() },
                    new SimpleRunner{ Name = "Runner1", Distance = Distance.ToString() },
                    new SimpleRunner{ Name = "Runner2", Distance = Distance.ToString() },
                    new SimpleRunner{ Name = "Runner3", Distance = Distance.ToString() },
                };
            }

            Navigation = navigation;
            Navigation.PushAsync(new View.RaceSetUpView(this));
        }
        #region Commands
        public ICommand AddNewRunnerCommand
        {
            get => new Command(() => Runners.Add(new SimpleRunner { Name = "Runner" + Runners.Count.ToString(), Distance = Distance.ToString() }));
        }
        public ICommand RaceEditingDoneCommand
        {
            get => new Command(async () =>
            {
                try
                {
                    for (int i = 0; i < Runners.Count; ++i)
                    {
                        if (Runners[i].Name == "") Runners.RemoveAt(i--);
                        else float.Parse(Runners[i].Distance);
                    }
                    Race.Reset();
                    Race.Runners = new SortableObservableCollection<RunnerViewModel>(
                        Runners.Select(runner => new RunnerViewModel(new RunnerModel(runner.Name, float.Parse(runner.Distance), Race))
                        ).ToList());
                    Race.OnPropertyChanged(nameof(Runners));
                    await Navigation.PopAsync(true);
                }
                catch (Exception e)
                {
                    /* TODO: log the error */
                    await Navigation.NavigationStack[^1].DisplayAlert("Ошибка", e.Message, "Окей");
                    Console.WriteLine("Error while executing - GoToRacePageCommand - " + e.Message);
                }
                catch
                {
                    /* TODO: log the error */
                    Console.WriteLine("Error while executing - GoToRacePageCommand");
                };
            });
        }
        #endregion
        #region misc
        public bool IndividualDistance { get => SettingsModel.IndividualDistance; }
        #endregion
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
