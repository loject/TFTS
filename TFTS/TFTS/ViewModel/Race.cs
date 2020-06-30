using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace TFTS.ViewModel
{
    public class Race : INotifyPropertyChanged
    {
        private INavigation Navigation;
        public ObservableCollection<Runner> Runners { get; private set; }
        private float distance_ = 3000;
        private float lapLength_ = 200;
        private Stopwatch timer_ = new Stopwatch();

        public float Distance { get => distance_; set { distance_ = value; OnPropertyChanged(nameof(Distance)); } }
        public float LapsCount { get => distance_ / lapLength_; }
        public string TotalTime { get => Utils.getStringFromTimeSpan(timer_.Elapsed); }
        public bool IsRunning { get => timer_.IsRunning; }
        public float LapLength { get => lapLength_; set { lapLength_ = value; foreach (Runner runner in Runners) runner.LapsGoal = (int)LapsCount; OnPropertyChanged(nameof(LapLength)); } }

        #region constructors
        public Race(INavigation navigation)
        {

            Runners = new ObservableCollection<Runner>
            {
                new Runner(),
                new Runner(),
            };

            Navigation = navigation;
            Navigation.PushAsync(new View.RaceSetUp(this));
        }
        #endregion
        #region command
        public ICommand AddNewRunnerCommand
        {
            get => new Command(() => Runners.Add(new Runner()));
        }
        public ICommand GoToRacePageCommand
        {
            get => new Command(() =>
            {
                try
                {
                    Navigation.PushAsync(new View.RaceView(this));
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                }
                catch
                {
                };
            });
        }
        public ICommand StartStopCommand
        {
            get => new Command(() =>
            {
                if (IsRunning)
                {
                    timer_.Stop();
                }
                else
                {
                    timer_.Start();
                    Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                    {
                        OnPropertyChanged(nameof(TotalTime));
                        return timer_.IsRunning;
                    });
                }
                OnPropertyChanged(nameof(IsRunning));
            });
        }
        public ICommand ResetCommand
        {
            get => new Command(async () =>
            {
                bool choiceIsStop = await Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Сброс", "Вы уверены?", "Да", "Нет");
                if (choiceIsStop == true)
                {
                    timer_.Reset();
                    foreach (Runner runner in Runners)
                        runner.Clear();
                    OnPropertyChanged(nameof(TotalTime));
                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(Runners));
                }
            });
        }
        public ICommand LapDoneCommand
        {
            get => new Command<Runner>((Runner runner) =>
            {
                runner.LapDone(new Lap
                {
                    Length = (int)LapLength,
                    Time = timer_.Elapsed - runner.TotalTime
                });
            });
        }
        
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
