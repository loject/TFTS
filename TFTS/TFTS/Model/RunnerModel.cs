using System;
using System.Collections.ObjectModel;
using System.Linq;
using TFTS.ViewModel;

namespace TFTS.Model
{
    public class RunnerModel
    {
        public string Name { get; set; } = "Runner";
        public ObservableCollection<float> CheckPoints { get; private set; } = new ObservableCollection<float>();
        public ObservableCollection<Lap> Laps { get; private set; } = new ObservableCollection<Lap>();
        public RaceViewModel Race { get; private set; }

        public RunnerModel(string Name, float Distance, RaceViewModel race)
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
        public float TotalDistance { get => CheckPoints.Sum(); }
        #endregion
    }
}
