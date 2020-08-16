using System;
using System.Collections.Generic;
using System.ComponentModel;
using TFTS.Models;
using Xamarin.Essentials;

namespace TFTS.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        const int MaxVibrationLength = 10000;
        const int MinVibrationLength = 0;
        public bool LapDoneBySwipe
        {
            get => Preferences.Get(nameof(LapDoneBySwipe), false);
            set
            {
                Preferences.Set(nameof(LapDoneBySwipe), value);
                OnPropertyChanged(nameof(LapDoneBySwipe));
            }
        }
        public bool FirstLapAlwaysFull
        {
            get => Preferences.Get(nameof(FirstLapAlwaysFull), true);
            set
            {
                Preferences.Set(nameof(FirstLapAlwaysFull), value);
                OnPropertyChanged(nameof(FirstLapAlwaysFull));
            }
        }
        public List<string> SortBestPosibleValues
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var property in typeof(RunnersSortingType).GetEnumNames())
                    res.Add(property);
                return res;
            }
        }
        public string SortBest
        {
            get
            {
                string res = "error";
                try
                {
                    res = Preferences.Get(nameof(SortBest), RunnersSortingType.DontSort.ToString());
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error - " + e.Message);
                }
                return res;
            }
            set
            {
                Preferences.Set(nameof(SortBest), value.ToString());
                OnPropertyChanged(nameof(SortBest));
                OnPropertyChanged(nameof(MoveFinishedToEndIsEnabled));
            }
        }
        public bool MoveFinishedToEnd
        {
            get => Preferences.Get(nameof(MoveFinishedToEnd), true);
            set
            {
                Preferences.Set(nameof(MoveFinishedToEnd), value);
                OnPropertyChanged(nameof(MoveFinishedToEnd));
            }
        }
        public bool LeftHandMode
        {
            get => Preferences.Get(nameof(LeftHandMode), false);
            set
            {
                Preferences.Set(nameof(LeftHandMode), value);
                OnPropertyChanged(nameof(LeftHandMode));
            }
        }
        public bool VibrationOnLapDone
        {
            get => Preferences.Get(nameof(VibrationOnLapDone), true);
            set
            {
                Preferences.Set(nameof(VibrationOnLapDone), value);
                OnPropertyChanged(nameof(VibrationOnLapDone));
            }
        }
        public int VibrationOnLapDoneLength
        {
            get => Preferences.Get(nameof(VibrationOnLapDoneLength), 150);
            set
            {
                try
                {
                    int val = value;
                    if (val > MaxVibrationLength) val = MaxVibrationLength;
                    if (val < MinVibrationLength) val = MinVibrationLength;
                    Preferences.Set(nameof(VibrationOnLapDoneLength), val);
                    OnPropertyChanged(nameof(VibrationOnLapDoneLength));
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error - " + e.Message);
                }
                catch
                {
                    System.Console.WriteLine("Error");
                }
            }
        }
        public bool HighlightFinishers
        {
            get => Preferences.Get(nameof(HighlightFinishers), true);
            set
            {
                Preferences.Set(nameof(HighlightFinishers), value);
                OnPropertyChanged(nameof(HighlightFinishers));
            }
        }
        public bool IndividualDistance
        {
            get => Preferences.Get(nameof(IndividualDistance), false);
            set
            {
                Preferences.Set(nameof(IndividualDistance), value);
                OnPropertyChanged(nameof(IndividualDistance));
            }
        }
        #region misc
        public bool MoveFinishedToEndIsEnabled { get => SortBest != RunnersSortingType.DontSort.ToString(); }
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
