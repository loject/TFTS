using System;
using Xamarin.Essentials;

namespace TFTS.Models
{
    enum RunnersSortingType
    {
        DontSort = 0,
        SortImmediately = 1,
        SortAfterLastLapDone = 2
    }
    class SettingsModel
    {
        public static bool LapDoneBySwipe
        {
            get => Preferences.Get(nameof(LapDoneBySwipe), false);
            set => Preferences.Set(nameof(LapDoneBySwipe), value);
        }
        public static bool FirstLapAlwaysFull
        {
            get => Preferences.Get(nameof(FirstLapAlwaysFull), true);
            set => Preferences.Set(nameof(FirstLapAlwaysFull), value);
        }
        public static RunnersSortingType SortBest
        {
            get
            {
                try
                {
                    string SortTypeStr = Preferences.Get(nameof(SortBest), RunnersSortingType.DontSort.ToString());
                    RunnersSortingType SortType = (RunnersSortingType)Enum.Parse(typeof(RunnersSortingType), SortTypeStr);
                    return SortType;
                }
                catch (Exception e)
                {
                    Preferences.Remove(nameof(SortBest));
                    Console.WriteLine("Error - " + e.Message);
                    return RunnersSortingType.DontSort;
                }
            } 
            set => Preferences.Set(nameof(SortBest), value.ToString());
        }
        public static bool MoveFinishedToEnd
        {
            get => Preferences.Get(nameof(MoveFinishedToEnd), true);
            set => Preferences.Set(nameof(MoveFinishedToEnd), value);
        }
        public static bool LeftHandMode
        {
            get => Preferences.Get(nameof(LeftHandMode), true);
            set => Preferences.Set(nameof(LeftHandMode), value);
        }
        public static bool VibrationOnLapDone
        {
            get => Preferences.Get(nameof(VibrationOnLapDone), true);
            set => Preferences.Set(nameof(VibrationOnLapDone), value);
        }
        public static int VibrationOnLapDoneLength
        {
            get => Preferences.Get(nameof(VibrationOnLapDoneLength), 150);
            set => Preferences.Set(nameof(VibrationOnLapDoneLength), value);
        }
        public static bool HighlightFinishers
        {
            get => Preferences.Get(nameof(HighlightFinishers), true);
            set => Preferences.Set(nameof(HighlightFinishers), value);
        }
        public static bool IndividualDistance
        {
            get => Preferences.Get(nameof(IndividualDistance), false);
            set => Preferences.Set(nameof(IndividualDistance), value);
        }
    }
}
