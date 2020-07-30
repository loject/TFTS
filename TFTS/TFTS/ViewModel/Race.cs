using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using TFTS.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TFTS.ViewModel
{
    public class Race : INotifyPropertyChanged
    {
        private INavigation Navigation;
        public ObservableCollection<Runner> Runners { get; private set; }
        private float distance_ = 3000;
        private float lapLength_ = 200;
        private DateTime startTime = new DateTime();
        private Stopwatch timer_ = new Stopwatch();

        public float Distance { get => distance_; set { distance_ = value; foreach (Runner runner in Runners) runner.LapsGoal = (int)LapsCount;  OnPropertyChanged(nameof(Distance)); } }
        public float LapsCount { get => distance_ / lapLength_; }
        public string TotalTime { get => Utils.getStringFromTimeSpan(timer_.Elapsed); }
        public bool IsRunning { get => timer_.IsRunning; }
        public float LapLength { get => lapLength_; set { lapLength_ = value; foreach (Runner runner in Runners) runner.LapsGoal = (int)LapsCount; OnPropertyChanged(nameof(LapLength)); } }
        public string StartTime { get => startTime.ToString(); }

        #region constructors
        public Race(INavigation navigation)
        {

            Runners = new ObservableCollection<Runner>
            {
                new Runner("Runner", (int)LapsCount),
                new Runner("Runner", (int)LapsCount),
            };

            Navigation = navigation;
            Navigation.PushAsync(new View.RaceSetUpView(this));
        }
        #endregion
        #region RaceSetUp commands
        public ICommand AddNewRunnerCommand
        {
            get => new Command(() => Runners.Add(new Runner("Runner", (int)LapsCount)));
        }
        public ICommand GoToRacePageCommand
        {
            get => new Command(() =>
            {
                try
                {
                    foreach (var i in Runners)
                        if (i.Name == "")
                            Runners.Remove(i);
                    Navigation.PushAsync(new View.RaceView(this));
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                }
                catch
                {
                };
            });
        }
        #endregion
        #region RaceViewCommands
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
                    if (timer_.ElapsedMilliseconds == 0) startTime = DateTime.Now;
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
        public ICommand ExportCommand
        {
            get => new Command(() =>
            {
                Share.RequestAsync(new ShareTextRequest(text: GetRaceResultCSV(), title: "Save results"));
            });
        }
        public ICommand ExportFileCommand
        {
            get => new Command(() =>
            {
                try
                {
                    var fn = "TFTS_" + DateTime.Now.ToString() + ".csv";
                    var file = Path.Combine(FileSystem.CacheDirectory, fn);
                    File.WriteAllText(file, GetRaceResultCSV());
                    Share.RequestAsync(new ShareFileRequest(file: new ShareFile(file), title: fn));
                }
                catch
                {
                    /* TODO: error massage */
                }
            });
        }
        public ICommand ShowResultPageCommand { get => new Command(() => { Navigation.PushModalAsync(new RaceResultsView(this)); }); }
        public ICommand LapDoneCommand
        {
            get => new Command<Runner>((Runner runner) =>
            {
                int position = 1;
                foreach (Runner runner1 in Runners) if (runner1.Laps.Count > runner.Laps.Count) position++;
                runner.LapDone(new Lap
                {
                    Length = (int)LapLength,
                    Time = timer_.Elapsed - runner.TotalTime,
                    Position = position
                });
            });
        }
        public ICommand ShowRunnerResultCommand { get => new Command<Runner>((Runner runner) => 
        { Navigation.PushModalAsync(new RunnerResultView(runner, startTime.ToString(), Distance.ToString())); });
        }
        public ICommand DeleteLapCommand
        {
            get => new Command<Runner>((Runner runner) =>
            { if (runner.Laps.Count != 0) runner.RemoveLap(runner.Laps.Count - 1); });
        }

        #endregion
        #region misc
        private string GetRaceResultCSV()
        {
            string res = "";
            res += "Забег на " + Distance + "метров. Начало " + startTime + "\n";
            res += "Спортсмен\\Время круга(позиция);";
            for (int i = 1; i <= Runners[0].Laps.Count; ++i) res += i.ToString() + ";";
            res += "\n";

            for (int i = 0; i < Runners.Count; ++i)
            {
                res += Runners[i].Name + ";";
                for (int j = 0; j < Runners[i].Laps.Count; ++j)
                {
                    res += Utils.getStringFromTimeSpan(Runners[i].Laps[j].Time) + "(" + Runners[i].Laps[j].Position.ToString() + ");";
                }
                res += "\n";
            }

            return res;
        }
        #endregion
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
        #region settings
        public bool LapDoneBySwipe { get => Preferences.Get(nameof(LapDoneBySwipe), false); }
        #endregion
    }
}
