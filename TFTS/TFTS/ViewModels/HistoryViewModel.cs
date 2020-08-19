﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using TFTS.Models;
using TFTS.Views;
using Xamarin.Forms;

namespace TFTS.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation;
        public List<RaceModel> Races { get => App.Database.GetRaceHistory(); } 
        public HistoryViewModel(INavigation navigation = null)
        {
            Navigation = navigation;
            Navigation?.PushAsync(new HistoryPageView(this));
        }
        #region Commands
        public ICommand ShowResultPageCommand { get => new Command<RaceModel>(Race => { Navigation.PushModalAsync(new RaceResultsView(Race)); }); }
        public ICommand ClearHistoryCommand { get => new Command(() => { App.Database.ClearHistory(); }); }
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
