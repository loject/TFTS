using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using TFTS.Model;
using Xamarin.Essentials;

namespace TFTS.ViewModel
{
    public struct Lap
    {
        public float Length { get; set; }
        public TimeSpan Time { get; set; }
        public string TimeStr { get => Utils.getStringFromTimeSpan(Time); }
        public double Speed { get => Length / Time.TotalSeconds; }
        public int Position { get; set; }
    }

    public class Runner : INotifyPropertyChanged, IComparable
    {
        private string name_ = "Runner";
        private Race race_;
        public string Name { get { return name_; } set { name_ = value; OnPropertyChanged(nameof(Name)); } }
        public ObservableCollection<Lap> Laps { get; private set; } = new ObservableCollection<Lap>();

        #region constuctors
        public Runner()
        {
        }
        public Runner(string name)
        {
            Name = name;
        }
        public Runner(string name, Race race)
        {
            Name = name;
            race_ = race;
        }
        #endregion

        public float LapsOvercome { get => DistanceOvercome / race_.LapLength; }
        public float DistanceLeft { get => race_.Distance - DistanceOvercome; }
        public float LapsLeft { get => DistanceLeft / race_.LapLength; }
        public float LapsGoal { get => race_.Distance / race_.LapLength; }
        public string BestLapTime
        {
            get
            {
                try
                {
                    if (Laps.Count == 0) return "Н/С";
                    TimeSpan best = TimeSpan.MaxValue;
                    foreach (Lap lap in Laps)
                        if (lap.Time < best)
                            best = lap.Time;
                    return Utils.getStringFromTimeSpan((best == TimeSpan.MaxValue) ? TimeSpan.Zero : best);
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
                    if (Laps.Count == 0) return "Н/С";
                    return Utils.getStringFromTimeSpan(Laps[Laps.Count - 1].Time);
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
        public TimeSpan TotalTime { get { TimeSpan res = TimeSpan.Zero; foreach (Lap lap in Laps) res += lap.Time; return res; } }
        public float DistanceOvercome { get => Laps.Sum(lap => lap.Length); }
        public bool IsFinished { get => LapsOvercome >= LapsGoal; }

        public void LapDone(Lap lap)
        {
            try
            {
                Laps.Add(lap);
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
                Laps.RemoveAt(index);
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
                Laps.Clear();
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
            var x = this;
            var y = obj as Runner;
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

            int lastLapId = x.Laps.Count - 1;
            if (x.Laps[lastLapId].Time == y.Laps[lastLapId].Time) return 0;
            return (x.Laps[lastLapId].Time > y.Laps[lastLapId].Time) ? 1 : -1;
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