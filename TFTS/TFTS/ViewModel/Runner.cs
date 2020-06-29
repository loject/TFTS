using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TFTS.ViewModel
{
    public struct Lap
    {
        public int Length { get; set; }
        public TimeSpan Time { get; set; }
        public double Speed { get => (double)Length / Time.TotalSeconds; }
    }

    public class Runner : INotifyPropertyChanged
    {
        private string name_ = "Runner";
        public string Name {
            get
            { return name_; }
            set
            {
                name_ = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public List<Lap> Laps { get; private set; } = new List<Lap>();

        #region constuctors
        public Runner()
        {
        }
        public Runner(string name)
        {
            Name = name;
        }
        #endregion

        public int LapsOvercome { get => Laps.Count; }
        public string BestLapTime
        {
            get
            {
                if (Laps.Count == 0) return Utils.getStringFromTimeSpan(TimeSpan.Zero);
                TimeSpan best = TimeSpan.MaxValue;
                foreach(Lap lap in Laps)
                {
                    if (lap.Time < best)
                    {
                        best = lap.Time;
                    }
                }
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


        public void LapDone(Lap lap)
        {
            Laps.Add(lap);
            OnPropertyChanged(nameof(LapsOvercome));
            OnPropertyChanged(nameof(BestLapTime));
            OnPropertyChanged(nameof(LastLapTime));
        }
        public void Clear()
        {
            Laps.Clear();
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