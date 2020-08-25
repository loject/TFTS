using PropertyChanged;
using System;
using System.Collections.Generic;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel
    {
        const int MaxVibrationLength = 10000;
        const int MinVibrationLength = 0;

        public SettingsViewModel()
        { }

        #region properties
        public bool LapDoneBySwipe
        {
            get => SettingsModel.LapDoneBySwipe;
            set => SettingsModel.LapDoneBySwipe = value;
        }
        public bool FirstLapAlwaysFull
        {
            get => SettingsModel.FirstLapAlwaysFull;
            set => SettingsModel.FirstLapAlwaysFull = value;
        }
        public string SortBest
        {
            get => SettingsModel.SortBest.ToString();
            set
            {
                try
                {
                    RunnersSortingType SortType = (RunnersSortingType)Enum.Parse(typeof(RunnersSortingType), value);
                    SettingsModel.SortBest = SortType;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error - " + e.Message);
                }
            }
        }
        [DependsOn(nameof(SortBest))]
        public bool MoveFinishedToEnd
        {
            get => SettingsModel.MoveFinishedToEnd;
            set => SettingsModel.MoveFinishedToEnd = value;
        }
        public bool LeftHandMode
        {
            get => SettingsModel.LeftHandMode;
            set => SettingsModel.LeftHandMode = value;
        }
        public bool VibrationOnLapDone
        {
            get => SettingsModel.VibrationOnLapDone;
            set => SettingsModel.VibrationOnLapDone = value;
        }
        public int VibrationOnLapDoneLength
        {
            get => SettingsModel.VibrationOnLapDoneLength;
            set => SettingsModel.VibrationOnLapDoneLength = Math.Clamp(value, MinVibrationLength, MaxVibrationLength);
        }
        public bool HighlightFinishers
        {
            get => SettingsModel.HighlightFinishers;
            set => SettingsModel.HighlightFinishers = value;
        }
        public bool IndividualDistance
        {
            get => SettingsModel.IndividualDistance;
            set => SettingsModel.IndividualDistance = value;
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
    }
}
