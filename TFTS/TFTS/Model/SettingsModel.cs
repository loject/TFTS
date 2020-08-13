using NPOI.HSSF.Record;
using System;
using Xamarin.Essentials;

namespace TFTS.Model
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
        }
        public static bool FirstLapAlwaysFull
        {
            get => Preferences.Get(nameof(FirstLapAlwaysFull), true);
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
        }
        public static bool MoveFinishedToEnd
        {
            get => Preferences.Get(nameof(MoveFinishedToEnd), true);
        }
        public static bool LeftHandMode
        {
            get => Preferences.Get(nameof(LeftHandMode), true);
        }
        public static bool VibrationOnLapDone
        {
            get => Preferences.Get(nameof(VibrationOnLapDone), true);
        }
        public static int VibrationOnLapDoneLength
        {
            get => Preferences.Get(nameof(VibrationOnLapDoneLength), 150);
        }
        public static bool HighlightFinishers
        {
            get => Preferences.Get(nameof(HighlightFinishers), true);
        }
    }
}
