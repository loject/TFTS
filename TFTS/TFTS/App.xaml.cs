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
        static PlanDatabase planDatabase;

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
        public static PlanDatabase PlanDatabase
        {
            get
            {
                if (planDatabase == null)
                {
                    planDatabase = new PlanDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TFTS_RacePlan.db3"));
                }
                return planDatabase;
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
