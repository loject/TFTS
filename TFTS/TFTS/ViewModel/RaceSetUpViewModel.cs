using Android.Content;
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
                foreach (var runner in race.Race.Runners)
                    Runners.Add(new SimpleRunner { Name = runner.Name, Distance = runner.TotalDistance.ToString() });
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
                    string ErrorStr = Validate();
                    if (string.IsNullOrEmpty(ErrorStr))
                    {
                        Race.Reset();
                        Race.Distance = float.Parse(Distance);
                        Race.LapLength = float.Parse(LapLength);
                        Race.Race.Runners = new SortableObservableCollection<RunnerModel>(
                            Runners.Select(runner => new RunnerModel(runner.Name, float.Parse(runner.Distance), Race.Race)
                            ).ToList());
                        Race.OnPropertyChanged(nameof(Runners));
                        await Navigation.PopAsync(true);
                    }
                    else
                    {
                        await Navigation.NavigationStack[^1].DisplayAlert("Ошибка", ErrorStr, "Окей");
                    }
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
        /**
         * validate data in entyes
         * * validate that all distances can be parsed to float
         * * no repeated names
         * * no empty names
         * * race distance greater than runners distance
         * @return string that contain error string if error is ocured, empty if no errors
         * */
        public string Validate()
        {
            string ErrorStr = "";
            /* validate distance */
            if (!float.TryParse(Distance, out _))
            {
                ErrorStr += $"Некорректная дистанция забега\n";
                return ErrorStr;
            }
            /* validate lap length */
            if (!float.TryParse(LapLength, out _))
            {
                ErrorStr += $"Некорректная длина круга\n";
                return ErrorStr;
            }
            /* validate runners list */
            for (int i = 0; i < Runners.Count; ++i)
            {
                /* empty name */
                if (Runners[i].Name == "")
                {
                    Runners.RemoveAt(i--);
                    continue;
                }
                /* correct distance */
                if (!float.TryParse(Runners[i].Distance, out _))
                {
                    ErrorStr += $"У {Runners[i].Name} некорректная дистанция\n";
                    continue;
                }
                /* runner distance should be less than race distance */
                if (float.Parse(Runners[i].Distance) > float.Parse(Distance))
                {
                    ErrorStr += $"У {Runners[i].Name} слишком большая дистанция\n";
                    continue;
                }
                /* no reapeated names */
                for (int j = i + 1; j < Runners.Count; ++j)
                {
                    if (Runners[i].Name == Runners[j].Name)
                    {
                        ErrorStr += $"{Runners[i].Name} указан несколько раз\n";
                        break;
                    }
                }
            }
            return ErrorStr;
        }
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
