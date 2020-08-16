using System;
using System.Collections.ObjectModel;
using System.Linq;
using TFTS.ViewModels;
using Xamarin.Essentials;

namespace TFTS.Models
{
    public class RunnerModel : IComparable
    {
        public string Name { get; set; } = "Runner";
        public ObservableCollection<float> CheckPoints { get; private set; } = new ObservableCollection<float>();
        public ObservableCollection<Lap> Laps { get; private set; } = new ObservableCollection<Lap>();
        public RaceModel Race { get; private set; }

        public RunnerModel(string Name, float Distance, RaceModel race)
        {
            this.Name = Name;
            Race = race;

            int CeilLapsCount = (int)Math.Ceiling(Distance / race.LapLength);
            if (SettingsModel.FirstLapAlwaysFull)
            {
                var LastLapLength = Distance % race.LapLength == 0 ? race.LapLength : Distance % race.LapLength;
                for (float i = 1; i < CeilLapsCount; ++i)
                    CheckPoints.Add(race.LapLength);
                CheckPoints.Add(LastLapLength);
            }
            else
            {
                var FirstLapLength = (Distance % race.LapLength == 0) ? race.LapLength : Distance % race.LapLength;
                CheckPoints.Add(FirstLapLength);
                for (float i = 1; i < CeilLapsCount; ++i)
                    CheckPoints.Add(race.LapLength);
            }
        }

        #region Properties
        public TimeSpan TotalTime { get => TimeSpan.FromTicks(Laps.Sum(lap => lap.Time.Ticks)); }
        public float TotalDistance { get => CheckPoints.Sum(); }
        public float DistanceOvercome { get => Laps.Sum(lap => lap.Length); }
        public float LapsOvercome { get => DistanceOvercome / Race.LapLength; }
        public float LapsGoal { get => TotalDistance / Race.LapLength; }
        public bool IsFinished { get => LapsOvercome >= LapsGoal; }
        #endregion
        #region functions
        public void LapDone(TimeSpan now)
        {
            try
            {
                float lapLength = (IsFinished ? Race.LapLength : CheckPoints[(int)Math.Ceiling(LapsOvercome)]);
                Laps.Add(new Lap
                {
                    Length = lapLength,
                    Time = now - TotalTime,
                    Position = Position
                });

                if (SettingsModel.SortBest == RunnersSortingType.SortImmediately)
                {
                    Race.Runners.Sort();
                }
                else if (SettingsModel.SortBest == RunnersSortingType.SortAfterLastLapDone)
                {
                    if (Position == Race.Runners.Count)
                    {
                        Race.Runners.Sort();
                    }
                }
                if (SettingsModel.VibrationOnLapDone)
                {
                    Vibration.Vibrate(SettingsModel.VibrationOnLapDoneLength);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - " + e.Message);
            }
            catch
            {
                /* TODO: error massage */
            }
        }
        /**
         * position
         * NOTE: return position if runner runs through the finish line now
         * */
        private int Position 
        { 
            get
            {
                int position = 1;
                foreach (RunnerModel runner1 in Race.Runners) 
                    if (runner1.Laps.Count > Laps.Count) 
                        position++;
                return position;
            }
        }
        #endregion
        #region interface implementations
        /* greater - faster */
        public int CompareTo(object obj)
        {
            /* mb compare distance */
            var x = this;
            var y = obj as RunnerModel;
            if (x.LapsOvercome != y.LapsOvercome)
            {
                if (SettingsModel.MoveFinishedToEnd && (x.IsFinished && !y.IsFinished || !x.IsFinished && y.IsFinished))
                {
                    if (x.IsFinished && !y.IsFinished)
                        return 1;
                    if (!x.IsFinished && y.IsFinished)
                        return -1;
                }
                if (x.LapsOvercome > y.LapsOvercome)
                    return -1;
                if (x.LapsOvercome < y.LapsOvercome)
                    return 1;
            }
            if (x.LapsOvercome == 0)
                return 0;

            int lastLapId = x.Laps.Count - 1;
            if (x.Laps[lastLapId].Time == y.Laps[lastLapId].Time) return 0;
            return (x.Laps[lastLapId].Time > y.Laps[lastLapId].Time) ? 1 : -1;
        }
        #endregion
    }
}
