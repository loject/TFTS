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
    }
}
