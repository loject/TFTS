using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TFTS.Model;

namespace TFTS.ViewModel
{
    public class RunnerViewModel : INotifyPropertyChanged, IComparable
    {
        public RunnerModel Runner { get; private set; }
        public string Name { get { return Runner.Name; } set { Runner.Name = value; OnPropertyChanged(nameof(Name)); } }

        public RunnerViewModel(RunnerModel runner) => this.Runner = runner;

        public float LapsOvercome { get => DistanceOvercome / Runner.Race.LapLength; }
        public float DistanceLeft { get => Runner.TotalDistance - DistanceOvercome; }
        public float LapsLeft { get => DistanceLeft / Runner.Race.LapLength; }
        public float LapsGoal { get => Runner.TotalDistance / Runner.Race.LapLength; }
        public string BestLapTime
        {
            get
            {
                try
                {
                    /* TODO: beatify */
                    if (Runner.Laps.Count == 0) return "Н/С";
                    return Utils.getStringFromTimeSpan(Runner.Laps.OrderBy(lap => lap.Time).Take(1).Select(Lap => Lap.Time).FirstOrDefault());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error - " + e.Message);
                }
                catch
                {
                    Console.WriteLine("Error");
                }
                return "Error";
            }
        }
        public string LastLapTime
        {
            get
            {
                try
                {
                    if (Runner.Laps.Count == 0) return "Н/С";
                    return Utils.getStringFromTimeSpan(Runner.Laps[Runner.Laps.Count - 1].Time);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error - " + e.Message);
                }
                catch
                {
                    Console.WriteLine("Error");
                }
                return "Error";
            }
        }
        public TimeSpan TotalTime { get { TimeSpan res = TimeSpan.Zero; foreach (Lap lap in Runner.Laps) res += lap.Time; return res; } }
        public float DistanceOvercome { get => Runner.Laps.Sum(lap => lap.Length); }
        public bool IsFinished { get => LapsOvercome >= LapsGoal; }

        public void LapDone(Lap lap)
        {
            try
            {
                Runner.Laps.Add(lap);
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
        public void Clear()
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

        /* greater - faster */
        public int CompareTo(object obj)
        {
            /* mb compare distance */
            var x = this;
            var y = obj as RunnerViewModel;
            if (x.LapsOvercome != y.LapsOvercome)
            {
                if (SettingsModel.MoveFinishedToEnd && (x.IsFinished && !y.IsFinished || !x.IsFinished && y.IsFinished))
                {
                    if (x.IsFinished && !y.IsFinished)
                        return 1;
                    if (!x.IsFinished && y.IsFinished)
                        return -1;
                }
                if (x.LapsOvercome > y.LapsOvercome)
                    return -1;
                if (x.LapsOvercome < y.LapsOvercome)
                    return 1;
            }
            if (x.LapsOvercome == 0)
                return 0;

            int lastLapId = x.Runner.Laps.Count - 1;
            if (x.Runner.Laps[lastLapId].Time == y.Runner.Laps[lastLapId].Time) return 0;
            return (x.Runner.Laps[lastLapId].Time > y.Runner.Laps[lastLapId].Time) ? 1 : -1;
        }
        #region InotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}