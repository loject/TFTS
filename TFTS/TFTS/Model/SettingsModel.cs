using NPOI.HSSF.Record;
using Xamarin.Essentials;

namespace TFTS.Model
{
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
        public static bool SortBest
        {
            get => Preferences.Get(nameof(SortBest), true);
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
    }
}
