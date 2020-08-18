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
    }
}
