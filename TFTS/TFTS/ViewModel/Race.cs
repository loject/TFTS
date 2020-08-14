using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using TFTS.misc;
using TFTS.Model;
using TFTS.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TFTS.ViewModel
{
    public class Race : INotifyPropertyChanged
    {
        private INavigation Navigation;
        public SortableObservableCollection<RunnerViewModel> Runners { get; private set; }
        private float distance_ = 1500;
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

            Runners = new SortableObservableCollection<RunnerViewModel>
            {
                new RunnerViewModel("Runner", this),
                new RunnerViewModel("Runner1", this),
                new RunnerViewModel("Runner2", this),
                new RunnerViewModel("Runner3", this),
            };

            Navigation = navigation;
            Navigation.PushAsync(new View.RaceSetUpView(this));
        }
        #endregion
        #region RaceSetUp commands
        public ICommand AddNewRunnerCommand
        {
            get => new Command(() => Runners.Add(new RunnerViewModel("Runner" + Runners.Count.ToString(), this)));
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
                catch (Exception e)
                {
                    /* TODO: log the error */
                    Console.WriteLine("Error while executing - GoToRacePageCommand - " + e.Message);
                }
                catch
                {
                    /* TODO: log the error */
                    Console.WriteLine("Error while executing - GoToRacePageCommand");
                };
            });
        }
        #endregion
        #region RaceViewCommands
        public ICommand StartStopCommand
        {
            get => new Command(() =>
            {
                try
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error - " + e.Message);
                }
                catch
                {

                }
            });
        }
        public ICommand ResetCommand
        {
            get => new Command(async () =>
            {
                try
                {
                    bool choiceIsStop = await Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Сброс", "Вы уверены?", "Да", "Нет");
                    if (choiceIsStop == true)
                    {
                        timer_.Reset();
                        foreach (RunnerViewModel runner in Runners)
                            runner.Clear();
                        OnPropertyChanged(nameof(TotalTime));
                        OnPropertyChanged(nameof(IsRunning));
                        OnPropertyChanged(nameof(Runners));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error - " + e.Message);
                }
            });
        }
        public ICommand ExportCommand
        {
            get => new Command(() =>
            {
                try
                {
                    Share.RequestAsync(new ShareTextRequest(text: GetRaceResultCSV(), title: "Save results"));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error - " + e.Message);
                }
                catch
                {
                    Console.WriteLine("Error");
                }
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
            get => new Command<RunnerViewModel>((RunnerViewModel runner) =>
            {
                try
                {
                    int position = 1;
                    foreach (RunnerViewModel runner1 in Runners) if (runner1.Laps.Count > runner.Laps.Count) position++;
                    float lapLength = lapLength_;
                    if (UnevenLaps)
                    {
                        if (SettingsModel.FirstLapAlwaysFull && runner.LapsLeft < 1 && runner.LapsLeft > 0
                            || !SettingsModel.FirstLapAlwaysFull && runner.LapsOvercome == 0)
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

                    if (SettingsModel.SortBest == RunnersSortingType.SortImmediately)
                    {
                        Runners.Sort();
                    }
                    else if (SettingsModel.SortBest == RunnersSortingType.SortAfterLastLapDone)
                    {
                        if (position == Runners.Count)
                        {
                            Runners.Sort();
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
            });
        }
        public ICommand ShowRunnerResultCommand 
        { 
            get => new Command<RunnerViewModel>((RunnerViewModel runner) => { Navigation.PushModalAsync(new RunnerResultView(runner, startTime.ToString(), Distance.ToString())); });
        }
        public ICommand DeleteLapCommand
        {
            get => new Command<RunnerViewModel>((RunnerViewModel runner) => { if (runner.Laps.Count != 0) runner.RemoveLap(runner.Laps.Count - 1); });
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

            int RowId = 4;
            /* laps times */
            /* TODO: replace this with overome distance */
            int CeilLapsCount = (int)Math.Ceiling(LapsCount);
            sheet.CreateRow(RowId).CreateCell(0).SetCellValue("Спортсмен");
            /* TODO: fix this check */
            if (SettingsModel.FirstLapAlwaysFull)
            {
                for (float i = 1; i < CeilLapsCount; ++i)
                {
                    sheet.GetRow(RowId).CreateCell(2 * (int)Math.Ceiling(i) - 1).SetCellValue($"Время {i}-го круга");
                    sheet.GetRow(RowId).CreateCell(2 * (int)Math.Ceiling(i)).SetCellValue($"Позиция на {i}-ом круге");
                }
                sheet.GetRow(RowId).CreateCell(2 * CeilLapsCount - 1).SetCellValue($"Время {LapsCount}-го круга");
                sheet.GetRow(RowId).CreateCell(2 * CeilLapsCount).SetCellValue($"Позиция на {LapsCount}-ом круге");
            }
            else
            {
                var FirstLapRatio = (Distance % LapLength == 0) ? 1 : (Distance % LapLength) / LapLength;
                for (float i = FirstLapRatio; i <= CeilLapsCount; ++i)
                {
                    sheet.GetRow(RowId).CreateCell(2 * (int)Math.Ceiling(i) - 1).SetCellValue($"Время {i}-го круга");
                    sheet.GetRow(RowId).CreateCell(2 * (int)Math.Ceiling(i)).SetCellValue($"Позиция на {i}-ом круге");
                }
            }
            sheet.GetRow(RowId).CreateCell(2 * CeilLapsCount + 1).SetCellValue("Общее время");
            RowId++;

            for (int i = 0; i < Runners.Count; ++i)
            {
                var CurrentRow = sheet.CreateRow(RowId);
                CurrentRow.CreateCell(0).SetCellValue(Runners[i].Name);
                for (int j = 1; j <= Runners[i].Laps.Count; ++j)
                {
                    CurrentRow.CreateCell(2 * j - 1).SetCellValue(Runners[i].Laps[j - 1].Time.ToString());
                    CurrentRow.CreateCell(2 * j).SetCellValue(Runners[i].Laps[j - 1].Position);
                }
                CurrentRow.CreateCell(2 * CeilLapsCount + 1).SetCellValue(Runners[i].TotalTime.ToString());
                RowId++;
            }

            /* overcome times */
            /* TODO: replace this with overome distance */
            RowId += 3;
            sheet.CreateRow(RowId).CreateCell(0).SetCellValue("Спортсмен");
            /* TODO: fix this check */
            if (SettingsModel.FirstLapAlwaysFull)
            {
                for (float i = 1; i <= CeilLapsCount; ++i)
                    sheet.GetRow(RowId).CreateCell((int)Math.Ceiling(i)).SetCellValue($"{i}-й круг");
                sheet.GetRow(RowId).CreateCell(CeilLapsCount).SetCellValue($"{LapsCount}-й круг");
            }
            else
            {
                var FirstLapRatio = (Distance % LapLength == 0) ? 1 : (Distance % LapLength) / LapLength;
                for (float i = FirstLapRatio; i <= CeilLapsCount; ++i)
                    sheet.GetRow(RowId).CreateCell((int)Math.Ceiling(i)).SetCellValue($"{i}-й круг");
            }
            sheet.GetRow(RowId).CreateCell(CeilLapsCount + 1).SetCellValue("Общее время");
            RowId++;
            for (int i = 0; i < Runners.Count; ++i)
            {
                var row = sheet.CreateRow(RowId);
                var time = TimeSpan.Zero;
                row.CreateCell(0).SetCellValue(Runners[i].Name);
                for (int j = 1; j <= Runners[i].Laps.Count; ++j)
                {
                    time += Runners[i].Laps[j - 1].Time;
                    row.CreateCell(j).SetCellValue(time.ToString());
                }
                row.CreateCell(CeilLapsCount + 1).SetCellValue(Runners[i].TotalTime.ToString());
                RowId++;
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
    }
}

/* TODO: 
 * same name runners
 * optimize lapDoneCommand
 * dont turn off screen 
 * fix overrunned laps 
 * validate race done command
 */
