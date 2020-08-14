using System.Collections.ObjectModel;
using TFTS.ViewModel;

namespace TFTS.Model
{
    public class RunnerModel
    {
        public string Name { get; set; } = "Runner";
        public ObservableCollection<Lap> Laps { get; private set; } = new ObservableCollection<Lap>();
        public Race Race { get; private set; }

        public RunnerModel(string Name, Race race)
        {
            this.Name = Name;
            Race = race;
        }
    }
}
