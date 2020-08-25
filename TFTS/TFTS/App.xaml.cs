using System;
using System.IO;
using TFTS.Models;
using TFTS.ViewModels;
using Xamarin.Forms;

namespace TFTS
{
    public partial class App : Application
    {
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TFTS_Races.db3"));
                }
                return database;
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
