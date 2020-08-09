using System.ComponentModel;
using TFTS.Model;
using Xamarin.Essentials;

namespace TFTS.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged
    {
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
        public bool SortBest
        {
            get => Preferences.Get(nameof(SortBest), true);
            set
            {
                Preferences.Set(nameof(SortBest), value);
                OnPropertyChanged(nameof(SortBest));
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

        #region InotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
