using Android.Content.Res;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TFTS.misc;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TFTS.ViewModels
{
    public class RaceViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; private set; }
        //public NavigationPage Page { get; private set; }
        public RaceModel Race { get; set; }
        public SortableObservableCollection<RunnerViewModel> Runners { get => new SortableObservableCollection<RunnerViewModel>(Race.Runners?.Select(r => new RunnerViewModel(r, this)).ToList() ?? new List<RunnerViewModel>()); }
        private Stopwatch timer_ = new Stopwatch();

        /* TODO: move to ImplementPropertyChanged and DependsOn */
        public float Distance { get => Race.Distance; set { Race.Distance = value; OnPropertyChanged(nameof(Distance)); OnPropertyChanged(nameof(LapsCount)); OnPropertyChanged(nameof(UnevenLaps)); } }
        public float LapsCount { get => Race.Distance / Race.LapLength; }
        public TimeSpan TotalTime { get => timer_.Elapsed; }
        public string TotalTimeStr { get => Utils.getStringFromTimeSpan(timer_.Elapsed); }
        public bool IsRunning { get => timer_.IsRunning; }
        public float LapLength { get => Race.LapLength; set { Race.LapLength= value; OnPropertyChanged(nameof(LapLength)); OnPropertyChanged(nameof(LapsCount)); OnPropertyChanged(nameof(UnevenLaps)); } }
        public bool UnevenLaps { get => Race.Distance % Race.LapLength != 0; }
        public string StartTime { get => Race.StartTime.ToString(); }

        #region constructors
        public RaceViewModel(INavigation navigation = null, RaceModel race = null)
        {
            Race = race ?? new RaceModel();
            Navigation = navigation;
            Navigation?.PushAsync(new RaceView(this));
            var Page = Navigation.NavigationStack[^1];
            /* add exit listener for save race to db*/
            (Application.Current.MainPage as NavigationPage).Popped += (object sender, NavigationEventArgs args) =>
            {
                if (args.Page == Page)
                {
                    SaveRaceToDB();
                }
            };
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
                        if (timer_.ElapsedMilliseconds == 0) Race.StartTime = DateTime.Now;
                        timer_.Start();
                        Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                        {
                            OnPropertyChanged(nameof(TotalTime));
                            OnPropertyChanged(nameof(TotalTimeStr));
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
                    bool choiceIsStop = await Navigation.NavigationStack[^1].DisplayAlert("Сброс", "Вы уверены?", "Да", "Нет");
                    if (choiceIsStop == true)
                    {
                        Reset();
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
                    Navigation.NavigationStack[^1].DisplayAlert("Error", e.Message, "OK");
                }
                catch
                {
                    /* TODO: error massage */
                }
            });
        }
        public ICommand ShowResultPageCommand { get => new Command(() => { Navigation.PushModalAsync(new RaceResultsView(this.Race)); }); }

        #endregion
        #region misc
        public void Reset()
        {
            timer_.Reset();
            Race.Runners?.ForEach(r => r.Laps.Clear());
            OnPropertyChanged(nameof(TotalTimeStr));
            OnPropertyChanged(nameof(TotalTime));
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(Race.Runners));
        }
        private string GetRaceResultCSV() /* TODO: delete this? */
        {
            string separator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            string res = "";
            res += "Время начала" + separator + Race.StartTime + "\n";
            res += "Дистанция" + separator + Distance.ToString() + "\n";
            res += "Длинна круга" + separator + LapLength.ToString() + "\n";
            res += "Спортсмен\\Время круга(позиция)" + separator;

            for (int i = 1; i <= LapsCount; ++i) res += i.ToString() + separator; 
            res += "\n";

            for (int i = 0; i < Race.Runners.Count; ++i)
            {
                res += Race.Runners[i].Name + separator;
                for (int j = 0; j < Race.Runners[i].Laps.Count; ++j)
                {
                    res += Utils.getStringFromTimeSpan(Race.Runners[i].Laps[j].Time) + "(" + Race.Runners[i].Laps[j].Position.ToString() + ")" + separator;
                }
                res += "\n";
            }

            return res;
        }
        public IWorkbook GetRaceResultXLSX()
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");

            sheet.CreateRow(0).CreateCell(0).SetCellValue("Начало");          sheet.GetRow(0).CreateCell(1).SetCellValue(Race.StartTime.ToString());
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

            for (int i = 0; i < Race.Runners.Count; ++i)
            {
                var CurrentRow = sheet.CreateRow(RowId);
                CurrentRow.CreateCell(0).SetCellValue(Race.Runners[i].Name);
                for (int j = 1; j <= Race.Runners[i].Laps.Count; ++j)
                {
                    CurrentRow.CreateCell(2 * j - 1).SetCellValue(Race.Runners[i].Laps[j - 1].Time.ToString());
                    CurrentRow.CreateCell(2 * j).SetCellValue(Race.Runners[i].Laps[j - 1].Position);
                }
                CurrentRow.CreateCell(2 * CeilLapsCount + 1).SetCellValue(Race.Runners[i].TotalTime.ToString());
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
            for (int i = 0; i < Race.Runners.Count; ++i)
            {
                var row = sheet.CreateRow(RowId);
                var time = TimeSpan.Zero;
                row.CreateCell(0).SetCellValue(Race.Runners[i].Name);
                for (int j = 1; j <= Race.Runners[i].Laps.Count; ++j)
                {
                    time += Race.Runners[i].Laps[j - 1].Time;
                    row.CreateCell(j).SetCellValue(time.ToString());
                }
                row.CreateCell(CeilLapsCount + 1).SetCellValue(Race.Runners[i].TotalTime.ToString());
                RowId++;
            }

            return workbook;
        }
        private void SaveRaceToDB()
        {
            App.Database.SaveRaceToRaceHistory(Race);
        }
        #endregion
        #region INotifyPropertyChanged interface implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}

/* TODO: 
 * optimize lapDoneCommand
 * dont turn off screen 
 * fix overrunned laps 
 * export overruned laps
 * Move to end finshed runners option in settings set isEnabled
 * Add posibiliti to save race after each lapDone
 */
