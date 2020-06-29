using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TFTS.ViewModel
{
    public struct Lap
    {
        public int length { get; set; }
        public TimeSpan time { get; set; }
        public double speed { get => (double)length / time.TotalSeconds; }
    }

    public class Runner : INotifyPropertyChanged
    {
        public string name {
            get
            { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(name));
            }
        }
        public List<Lap> laps = new List<Lap>();

        public Runner()
        {
        }

        public int LapsOvercome { get => laps.Count; }
        public string BestLapTime
        {
            get
            {
                if (laps.Count == 0) return Utils.getStringFromTimeSpan(TimeSpan.Zero);
                TimeSpan best = TimeSpan.MaxValue;
                foreach(Lap lap in laps)
                {
                    if (lap.time < best)
                    {
                        best = lap.time;
                    }
                }
                return Utils.getStringFromTimeSpan((best == TimeSpan.MaxValue) ? TimeSpan.Zero : best);
            }
        }
        public string LastLapTime
        {
            get
            {
                if (laps.Count == 0) return "Н/С";
                return Utils.getStringFromTimeSpan(laps[laps.Count - 1].time);
            }
        }


        public void LapDone(Lap lap)
        {
            laps.Add(lap);
            OnPropertyChanged(nameof(LapsOvercome));
            OnPropertyChanged(nameof(BestLapTime));
            OnPropertyChanged(nameof(LastLapTime));
        }
        public void Clear()
        {
            laps.Clear();
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