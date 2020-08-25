using System;
using System.IO;
using TFTS.Databases;
using TFTS.ViewModels;
using Xamarin.Forms;

namespace TFTS
{
    public partial class App : Application
    {
        static HistoryDatabase historyDatabase;

        public static HistoryDatabase HistoryDatabase
        {
            get
            {
                if (historyDatabase == null)
                {
                    historyDatabase = new HistoryDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TFTS_RaceHistory.db3"));
                }
                return historyDatabase;
            }
        }
        public App()
        {
            InitializeComponent();

            var MainPageVM = new MainPageViewModel();
            var MainPagePage = new MainPage();
            MainPagePage.BindingContext = MainPageVM;

            MainPage = new NavigationPage(MainPagePage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
