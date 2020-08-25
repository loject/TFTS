using PropertyChanged;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class RacePageViewModel : RaceViewModel
    {
        #region Members
        private Stopwatch timer_ = new Stopwatch();
        #endregion
        public RacePageViewModel(RaceModel raceModel) : base(raceModel)
        { }
        #region Property
        public TimeSpan TotalTime { get => timer_.Elapsed; }
        public string TotalTimeStr { get => Utils.getStringFromTimeSpan(timer_.Elapsed); }
        public bool IsRunning { get => timer_.IsRunning; }
        #endregion
        #region Commands
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
            });
        }
        public ICommand ShowResultPageCommand { get => new Command(() => 
        { 
            Application.Current.MainPage.Navigation.PushModalAsync(new RaceResultsView(Race)); 
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
        public ICommand ResetCommand
        {
            get => new Command(async () =>
            {
                try
                {
                    bool choiceIsStop = await Application.Current.MainPage.Navigation.NavigationStack[^1].DisplayAlert("Сброс", "Вы уверены?", "Да", "Нет");
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
        public ICommand ExportXLSXFileCommand
        {
            get => new Command(() =>
            {
                try
                {
                    var fn = "TFTS_" + Name + ".xlsx";
                    var file = Path.Combine(FileSystem.CacheDirectory, fn);
                    var xlsx = GetRaceResultXLSX();
                    xlsx.Write(File.Create(file));
                    Share.RequestAsync(new ShareFileRequest(file: new ShareFile(file), title: fn));
                }
                catch (Exception e)
                {
                    Application.Current.MainPage.Navigation.NavigationStack[^1].DisplayAlert("Error", e.Message, "OK");
                }
            });
        }
        #endregion
        #region misc
        public void Reset()
        {
            timer_.Reset();
            Race.Runners?.ForEach(r => r.Laps.Clear());
            OnPropertyChanged(nameof(TotalTime));
            OnPropertyChanged(nameof(TotalTimeStr));
            OnPropertyChanged(nameof(IsRunning));
        }
        #endregion
    }
}
