using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TFTS.ViewModel
{
    public struct Lap
    {
        public int Length { get; set; }
        public TimeSpan Time { get; set; }
        public double Speed { get => (double)Length / Time.TotalSeconds; }
        public int Position { get; set; }
    }

    public class Runner : INotifyPropertyChanged
    {
        private string name_ = "Runner";
        private int lapsGoal_ = 0;
        public string Name { get { return name_; } set { name_ = value; OnPropertyChanged(nameof(Name)); } }
        public List<Lap> Laps { get; private set; } = new List<Lap>();

        #region constuctors
        public Runner()
        {
        }
        public Runner(string name)
        {
            Name = name;
        }
        public Runner(string name, int lapsGoal)
        {
            Name = name;
            lapsGoal_ = lapsGoal;
        }
        #endregion

        public int LapsOvercome { get => Laps.Count; }
        public string LapsLeft { get => (lapsGoal_ - Laps.Count).ToString(); }
        public int LapsGoal { get => lapsGoal_; set { lapsGoal_ = value; OnPropertyChanged(nameof(LapsGoal)); } }
        public string BestLapTime
        {
            get
            {
                if (Laps.Count == 0) return "Н/С";
                TimeSpan best = TimeSpan.MaxValue;
                foreach(Lap lap in Laps)
                    if (lap.Time < best)
                        best = lap.Time;
                return Utils.getStringFromTimeSpan((best == TimeSpan.MaxValue) ? TimeSpan.Zero : best);
            }
        }
        public string LastLapTime
        {
            get
            {
                if (Laps.Count == 0) return "Н/С";
                return Utils.getStringFromTimeSpan(Laps[Laps.Count - 1].Time);
            }
        }
        public TimeSpan TotalTime { get { TimeSpan res = TimeSpan.Zero; foreach (Lap lap in Laps) res += lap.Time; return res; } }


        public void LapDone(Lap lap)
        {
            Laps.Add(lap);
            OnPropertyChanged(nameof(LapsLeft));
            OnPropertyChanged(nameof(LapsOvercome));
            OnPropertyChanged(nameof(BestLapTime));
            OnPropertyChanged(nameof(LastLapTime));
        }
        public void Clear()
        {
            Laps.Clear();
            OnPropertyChanged(nameof(LapsLeft));
            OnPropertyChanged(nameof(LapsOvercome));
            OnPropertyChanged(nameof(BestLapTime));
            OnPropertyChanged(nameof(LastLapTime));
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