using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class RunnerViewModel : INotifyPropertyChanged
    {
        public RunnerModel Runner { get; private set; }
        public RaceViewModel Race { get; private set; }/* for update when sort */
        public string Name { get { return Runner.Name; } set { Runner.Name = value; OnPropertyChanged(nameof(Name)); } }

        public RunnerViewModel(RunnerModel runner, RaceViewModel race)
        {
            Runner = runner;
            Race = race;
        }

        public float LapsOvercome { get => DistanceOvercome / Runner.Race.LapLength; }
        public float DistanceLeft { get => Runner.TotalDistance - DistanceOvercome; }
        public float LapsLeft { get => DistanceLeft / Runner.Race.LapLength; }
        public float LapsGoal { get => Runner.LapsGoal; }
        public string BestLapTime { get => Utils.getStringFromTimeSpan(Runner.Laps.OrderBy(lap => lap.Time).Take(1).Select(Lap => Lap.Time).FirstOrDefault(), "Н/С"); }
        public string LastLapTime { get => Utils.getStringFromTimeSpan(Runner.Laps.LastOrDefault().Time, "Н/С"); }
        public TimeSpan TotalTime { get => Runner.TotalTime; }
        public float DistanceOvercome { get => Runner.DistanceOvercome; }
        public bool IsFinished { get => Runner.IsFinished; }

        #region commands
        public ICommand LapDoneCommand
        {
            get => new Command<TimeSpan>((TimeSpan now) =>
            {
                Runner.LapDone(now);
                OnPropertyChanged(nameof(LapsLeft));
                OnPropertyChanged(nameof(LapsOvercome));
                OnPropertyChanged(nameof(BestLapTime));
                OnPropertyChanged(nameof(LastLapTime));
                Race.OnPropertyChanged("Runners");
            });
        }
        public ICommand ShowRunnerResultCommand
        {
            get => new Command<RunnerViewModel>((RunnerViewModel runner) => 
            { 
                Race.Navigation.PushModalAsync(new RunnerResultView(runner, Race.StartTime.ToString(), Runner.TotalDistance.ToString())); 
            });
        }
        public ICommand DeleteLapCommand
        {
            get => new Command<RunnerViewModel>((RunnerViewModel runner) => 
            { 
                if (runner.Runner.Laps.Count != 0) 
                    runner.RemoveLap(runner.Runner.Laps.Count - 1); 
            });
        }
        #endregion
        #region functions
        public void RemoveLap(int index)
        {
            try
            {
                Runner.Laps.RemoveAt(index);
                OnPropertyChanged(nameof(LapsLeft));
                OnPropertyChanged(nameof(LapsOvercome));
                OnPropertyChanged(nameof(BestLapTime));
                OnPropertyChanged(nameof(LastLapTime));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - " + e.Message);
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
        public void Clear()/* TODO: delete this */
        {
            try
            {
                Runner.Laps.Clear();
                OnPropertyChanged(nameof(LapsLeft));
                OnPropertyChanged(nameof(LapsOvercome));
                OnPropertyChanged(nameof(BestLapTime));
                OnPropertyChanged(nameof(LastLapTime));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - " + e.Message);
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
        #endregion
        #region InotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}