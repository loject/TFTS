using System;
using System.Collections.Generic;
using System.ComponentModel;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; private set; }
        const int MaxVibrationLength = 10000;
        const int MinVibrationLength = 0;

        public SettingsViewModel(INavigation navigation)
        {
            Navigation = navigation;
            Navigation.PushAsync(new SettingView(this));
        }

        #region properties
        public bool LapDoneBySwipe
        {
            get => SettingsModel.LapDoneBySwipe;
            set
            {
                SettingsModel.LapDoneBySwipe = value;
                OnPropertyChanged(nameof(LapDoneBySwipe));
            }
        }
        public bool FirstLapAlwaysFull
        {
            get => SettingsModel.FirstLapAlwaysFull;
            set
            {
                SettingsModel.FirstLapAlwaysFull = value;
                OnPropertyChanged(nameof(FirstLapAlwaysFull));
            }
        }
        public string SortBest
        {
            get => SettingsModel.SortBest.ToString();
            set
            {
                try
                {
                    string SortTypeStr = Preferences.Get(nameof(SortBest), RunnersSortingType.DontSort.ToString());
                    RunnersSortingType SortType = (RunnersSortingType)Enum.Parse(typeof(RunnersSortingType), SortTypeStr);
                    SettingsModel.SortBest = SortType;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error - " + e.Message);
                }
                OnPropertyChanged(nameof(SortBest));
                OnPropertyChanged(nameof(MoveFinishedToEndIsEnabled));
            }
        }
        public bool MoveFinishedToEnd
        {
            get => SettingsModel.MoveFinishedToEnd;
            set
            {
                SettingsModel.MoveFinishedToEnd = value;
                OnPropertyChanged(nameof(MoveFinishedToEnd));
            }
        }
        public bool LeftHandMode
        {
            get => SettingsModel.LeftHandMode;
            set
            {
                SettingsModel.LeftHandMode = value;
                OnPropertyChanged(nameof(LeftHandMode));
            }
        }
        public bool VibrationOnLapDone
        {
            get => SettingsModel.VibrationOnLapDone;
            set
            {
                SettingsModel.VibrationOnLapDone = value;
                OnPropertyChanged(nameof(VibrationOnLapDone));
            }
        }
        public int VibrationOnLapDoneLength
        {
            get => SettingsModel.VibrationOnLapDoneLength;
            set
            {
                try
                {
                    int val = Math.Clamp(value, MinVibrationLength, MaxVibrationLength);
                    SettingsModel.VibrationOnLapDoneLength = val;
                    OnPropertyChanged(nameof(VibrationOnLapDoneLength));
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error - " + e.Message);
                }
            }
        }
        public bool HighlightFinishers
        {
            get => SettingsModel.HighlightFinishers;
            set
            {
                SettingsModel.HighlightFinishers = value;
                OnPropertyChanged(nameof(HighlightFinishers));
            }
        }
        public bool IndividualDistance
        {
            get => SettingsModel.IndividualDistance;
            set
            {
                SettingsModel.IndividualDistance = value;
                OnPropertyChanged(nameof(IndividualDistance));
            }
        }
        #endregion
        #region misc
        public bool MoveFinishedToEndIsEnabled { get => SortBest != RunnersSortingType.DontSort.ToString(); }
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
