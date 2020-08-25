﻿using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SimpleRunner
    {
        public string Name { get; set; }
        public string Distance { get; set; }
    }
    [AddINotifyPropertyChangedInterface]
    public class RaceSetUpViewModel
    {
        private string _raceName { get; set; } = DateTime.Now.ToString();
        private string _distance { get; set; } = "1500";
        private string _lapLength { get; set; } = "200";

        public ObservableCollection<SimpleRunner> Runners { get; private set; }
        //public RaceViewModel Race { get; private set; }
        public string Name { get => _raceName; set => _raceName = value; }
        public string Distance
        {
            get => _distance;
            set
            {
                foreach (SimpleRunner i in Runners) if (i.Distance == Distance) i.Distance = value;
                _distance = value;
            }
        }
        public string LapLength { get => _lapLength; set => _lapLength = value; }

        public RaceSetUpViewModel(/*RaceViewModel race = null*/)
        {
            /* TODO: adaptive to race edition */
            Runners = new ObservableCollection<SimpleRunner>
            {
                new SimpleRunner{ Name = "0", Distance = Distance.ToString() },
                new SimpleRunner{ Name = "1", Distance = Distance.ToString() },
            };
        }
        #region Commands
        public ICommand AddNewRunnerCommand
        {
            get => new Command(() => Runners.Add(new SimpleRunner { Name = Runners.Count.ToString(), Distance = Distance.ToString() }));
        }
        public ICommand RaceEditingDoneCommand
        {
            get => new Command(async () =>
            {
                try
                {
                    string ErrorStr = Validate();
                    if (string.IsNullOrEmpty(ErrorStr))
                    {
                        /* TODO: optional open race view */
                        var RacePageVM = GetRacePageViewModel();
                        var RacePage = new RaceView();
                        RacePage.BindingContext = RacePageVM;
                        await Application.Current.MainPage.Navigation.PushAsync(RacePage);
                        Application.Current.MainPage.Navigation.RemovePage(Application.Current.MainPage.Navigation.NavigationStack[^2]);
                    }
                    else
                    {
                        await Application.Current.MainPage.Navigation.NavigationStack[^1].DisplayAlert("Ошибка", ErrorStr, "Окей");
                    }
                }
                catch (Exception e)
                {
                    /* TODO: log the error */
                    await Application.Current.MainPage.Navigation.NavigationStack[^1].DisplayAlert("Ошибка", e.Message, "Окей");
                    Console.WriteLine("Error while executing - GoToRacePageCommand - " + e.Message);
                }
            });
        }
        #endregion
        #region Functions
        /**
         * Update viewModel if it pass, create new if it null
         * */
        private RaceViewModel GetRaceViewModel(RaceViewModel raceViewModel = null)
        {
            RaceViewModel raceVM = raceViewModel ?? new RaceViewModel(new RaceModel
            {
                Name = Name,
                Distance = float.Parse(Distance),
                LapLength = float.Parse(LapLength),
            });
            if (string.IsNullOrEmpty(Name)) raceVM.Name = DateTime.Now.ToString();
            raceVM.Race.Runners = Runners.Select(r => new RunnerModel(r.Name, float.Parse(r.Distance), raceVM.Race)).ToList();
            return raceVM;
        }
        /**
         * Update viewModel if it pass, create new if it null
         * */
        private RaceViewModel GetRacePageViewModel(RacePageViewModel racePageViewModel = null)
        {
            RacePageViewModel raceVM = racePageViewModel ?? new RacePageViewModel(new RaceModel
            {
                Name = Name,
                Distance = float.Parse(Distance),
                LapLength = float.Parse(LapLength),
            });
            if (string.IsNullOrEmpty(Name)) raceVM.Name = DateTime.Now.ToString();
            raceVM.Race.Runners = Runners.Select(r => new RunnerModel(r.Name, float.Parse(r.Distance), raceVM.Race)).ToList();
            return raceVM;
        }
        #endregion
        #region misc
        /**
         * validate data in entyes
         * * validate that all distances can be parsed to float
         * * no repeated names
         * * no empty names
         * * race distance greater than runners distance
         * @return string that contain error string if error is ocured, empty if no errors
         * */
        public string Validate()
        {
            string ErrorStr = "";
            /* validate distance */
            if (!float.TryParse(Distance, out _))
            {
                ErrorStr += $"Некорректная дистанция забега\n";
                return ErrorStr;
            }
            /* validate lap length */
            if (!float.TryParse(LapLength, out _))
            {
                ErrorStr += $"Некорректная длина круга\n";
                return ErrorStr;
            }
            /* validate runners list */
            for (int i = 0; i < Runners.Count; ++i)
            {
                /* empty name */
                if (Runners[i].Name == "")
                {
                    Runners.RemoveAt(i--);
                    continue;
                }
                /* correct distance */
                if (!float.TryParse(Runners[i].Distance, out _))
                {
                    ErrorStr += $"У {Runners[i].Name} некорректная дистанция\n";
                    continue;
                }
                /* runner distance should be less than race distance */
                if (float.Parse(Runners[i].Distance) > float.Parse(Distance))
                {
                    ErrorStr += $"У {Runners[i].Name} слишком большая дистанция\n";
                    continue;
                }
                /* no reapeated names */
                for (int j = i + 1; j < Runners.Count; ++j)
                {
                    if (Runners[i].Name == Runners[j].Name)
                    {
                        ErrorStr += $"{Runners[i].Name} указан несколько раз\n";
                        break;
                    }
                }
            }
            return ErrorStr;
        }
        public bool IndividualDistance { get => SettingsModel.IndividualDistance; }
        #endregion
    }
}
