using System;
using TFTS.misc;

namespace TFTS.Models
{
    public class RaceModel
    {
        //public string Name { get; set; } /* TODO */
        public DateTime StartTime { get; set; } = DateTime.Now;
        public float Distance { get; set; }
        public float LapLength { get; set; }
        public SortableObservableCollection<RunnerModel> Runners { get; set; }

        #region constructors
        public RaceModel(float Distance = 1500, float LapLength = 200, SortableObservableCollection<RunnerModel> runners = null)
        {
            this.Distance = Distance;
            this.LapLength = LapLength;
            Runners = runners ?? new SortableObservableCollection<RunnerModel>();
        }
        #endregion
    }
}
