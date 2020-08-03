using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using TFTS.misc;
using TFTS.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TFTS.ViewModel
{
    public class Race : INotifyPropertyChanged
    {
        private INavigation Navigation;
        public SortableObservableCollection<Runner> Runners { get; private set; }
        private float distance_ = 3000;
        private float lapLength_ = 200;
        private DateTime startTime = new DateTime();
        private Stopwatch timer_ = new Stopwatch();

        public float Distance { get => distance_; set { distance_ = value; OnPropertyChanged(nameof(Distance)); } }
        public float LapsCount { get => distance_ / lapLength_; }
        public string TotalTime { get => Utils.getStringFromTimeSpan(timer_.Elapsed); }
        public bool IsRunning { get => timer_.IsRunning; }
        public float LapLength { get => lapLength_; set { lapLength_ = value; OnPropertyChanged(nameof(LapLength)); } }
        public bool UnevenLaps { get => distance_ % lapLength_ != 0; }
        public string StartTime { get => startTime.ToString(); }

        #region constructors
        public Race(INavigation navigation)
        {

            Runners = new SortableObservableCollection<Runner>
            {
                new Runner("Runner", this),
                new Runner("Runner1", this),
                new Runner("Runner2", this),
                new Runner("Runner3", this),
            };

            Navigation = navigation;
            Navigation.PushAsync(new View.RaceSetUpView(this));
        }
        #endregion
        #region RaceSetUp commands
        public ICommand AddNewRunnerCommand
        {
            get => new Command(() => Runners.Add(new Runner("Runner", this)));
        }
        public ICommand GoToRacePageCommand
        {
            get => new Command(() =>
            {
                try
                {
                    foreach (var i in Runners)
                        if (i.Name == "")
                            Runners.Remove(i);
                    Navigation.PushAsync(new View.RaceView(this));
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                }
                catch
                {
                };
            });
        }
        #endregion
        #region RaceViewCommands
        public ICommand StartStopCommand
        {
            get => new Command(() =>
            {
                if (IsRunning)
                {
                    timer_.Stop();
                }
                else
                {
                    if (timer_.ElapsedMilliseconds == 0) startTime = DateTime.Now;
                    timer_.Start();
                    Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                    {
                        OnPropertyChanged(nameof(TotalTime));
                        return timer_.IsRunning;
                    });
                }
                OnPropertyChanged(nameof(IsRunning));
            });
        }
        public ICommand ResetCommand
        {
            get => new Command(async () =>
            {
                bool choiceIsStop = await Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Сброс", "Вы уверены?", "Да", "Нет");
                if (choiceIsStop == true)
                {
                    timer_.Reset();
                    foreach (Runner runner in Runners)
                        runner.Clear();
                    OnPropertyChanged(nameof(TotalTime));
                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(Runners));
                }
            });
        }
        public ICommand ExportCommand
        {
            get => new Command(() =>
            {
                Share.RequestAsync(new ShareTextRequest(text: GetRaceResultCSV(), title: "Save results"));
            });
        }
        public ICommand ExportXLSXFileCommand
        {
            get => new Command(() =>
            {
                try
                {
                    var fn = "TFTS_" + DateTime.Now.ToString() + ".xlsx";
                    var file = Path.Combine(FileSystem.CacheDirectory, fn);
                    var xlsx = GetRaceResultXLSX();
                    xlsx.Write(File.Create(file));
                    Share.RequestAsync(new ShareFileRequest(file: new ShareFile(file), title: fn));
                }
                catch (Exception e)
                {
                    Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Error", e.Message, "OK");
                }
                catch
                {
                    /* TODO: error massage */
                }
            });
        }
        public ICommand ShowResultPageCommand { get => new Command(() => { Navigation.PushModalAsync(new RaceResultsView(this)); }); }
        public ICommand LapDoneCommand
        {
            get => new Command<Runner>((Runner runner) =>
            {
                try
                {
                    int position = 1;
                    foreach (Runner runner1 in Runners) if (runner1.Laps.Count > runner.Laps.Count) position++;
                    float lapLength = lapLength_;
                    if (UnevenLaps)
                    {
                        if (FirstLapAlwaysFull && runner.LapsLeft < 1 && runner.LapsLeft > 0
                            || !FirstLapAlwaysFull && runner.LapsOvercome == 0)
                        {
                            lapLength = Distance % LapLength;
                        }
                    }
                    runner.LapDone(new Lap
                    {
                        Length = lapLength,
                        Time = timer_.Elapsed - runner.TotalTime,
                        Position = position
                    });

                    if (SortBest)
                    {
                        /* greater - faster */
                        Runners.Sort(new Comparison<Runner>((a, b) => {
                            if (a.LapsOvercome != b.LapsOvercome)
                            {
                                if (MoveFinishedToEnd && (a.IsFinished && !b.IsFinished || !a.IsFinished && b.IsFinished))
                                {
                                    if (a.IsFinished && !b.IsFinished)
                                        return 1;
                                    if (!a.IsFinished && b.IsFinished)
                                        return -1;
                                }
                                if (a.LapsOvercome > b.LapsOvercome)
                                    return -1;
                                if (a.LapsOvercome < b.LapsOvercome)
                                    return 1;
                            }
                            if (a.LapsOvercome == 0)
                                return 0;

                            int lastLapId = a.Laps.Count - 1;
                            if (a.Laps[lastLapId].Time == b.Laps[lastLapId].Time) return 0;
                            return (a.Laps[lastLapId].Time > b.Laps[lastLapId].Time) ? 1 : -1;
                        }));
                    }
                }
                catch
                {
                    /* TODO: error massage */
                }
            },
            (Runner runner) =>
            {
                try
                {
                    return runner.LapsLeft > 0;
                }
                catch
                {
                    /* TODO: error massage */
                }
                return false;
            });
        }
        public ICommand ShowRunnerResultCommand 
        { 
            get => new Command<Runner>((Runner runner) => { Navigation.PushModalAsync(new RunnerResultView(runner, startTime.ToString(), Distance.ToString())); });
        }
        public ICommand DeleteLapCommand
        {
            get => new Command<Runner>((Runner runner) => { if (runner.Laps.Count != 0) runner.RemoveLap(runner.Laps.Count - 1); });
        }

        #endregion
        #region misc
        private string GetRaceResultCSV()
        {
            string separator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            string res = "";
            res += "Время начала" + separator + startTime + "\n";
            res += "Дистанция" + separator + Distance.ToString() + "\n";
            res += "Длинна круга" + separator + LapLength.ToString() + "\n";
            res += "Спортсмен\\Время круга(позиция)" + separator;

            for (int i = 1; i <= LapsCount; ++i) res += i.ToString() + separator; 
            res += "\n";

            for (int i = 0; i < Runners.Count; ++i)
            {
                res += Runners[i].Name + separator;
                for (int j = 0; j < Runners[i].Laps.Count; ++j)
                {
                    res += Utils.getStringFromTimeSpan(Runners[i].Laps[j].Time) + "(" + Runners[i].Laps[j].Position.ToString() + ")" + separator;
                }
                res += "\n";
            }

            return res;
        }
        public IWorkbook GetRaceResultXLSX()
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");

            sheet.CreateRow(0).CreateCell(0).SetCellValue("Начало");          sheet.GetRow(0).CreateCell(1).SetCellValue(startTime.ToString());
            sheet.CreateRow(1).CreateCell(0).SetCellValue("Дистанция");       sheet.GetRow(1).CreateCell(1).SetCellValue(Distance);
            sheet.CreateRow(2).CreateCell(0).SetCellValue("Длинна круга");    sheet.GetRow(2).CreateCell(1).SetCellValue(LapLength);
            sheet.CreateRow(3).CreateCell(0).SetCellValue("Спортсмен\\Время круга(позиция)");

            /* TODO: replace this with overome distance */
            for (int i = 1; i <= LapsCount; ++i) sheet.GetRow(3).CreateCell(i).SetCellValue(i);
            sheet.GetRow(3).CreateCell((int)Math.Ceiling(LapsCount) + 1).SetCellValue("Общее время");

            for (int i = 0; i < Runners.Count; ++i)
            {
                var row = sheet.CreateRow(4 + i);
                row.CreateCell(0).SetCellValue(Runners[i].Name);
                for (int j = 0; j < Runners[i].Laps.Count; ++j)
                    row.CreateCell(j + 1).SetCellValue(Utils.getStringFromTimeSpan(Runners[i].Laps[j].Time) + "(" + Runners[i].Laps[j].Position.ToString() + ")");
                row.CreateCell((int)Math.Ceiling(LapsCount) + 1).SetCellValue(Utils.getStringFromTimeSpan(Runners[i].TotalTime));
            }

            return workbook;
        }
        #endregion
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
        #region settings
        public bool LapDoneBySwipe { get => Preferences.Get(nameof(LapDoneBySwipe), false); }
        public bool FirstLapAlwaysFull { get => Preferences.Get(nameof(FirstLapAlwaysFull), false); }
        public bool SortBest { get => Preferences.Get(nameof(SortBest), false); }
        public bool MoveFinishedToEnd { get => Preferences.Get(nameof(MoveFinishedToEnd), false); }
        #endregion
    }
}
