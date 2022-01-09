using RecipeAppModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecipeApp
{
    public partial class App : Application
    {

        private AppLogic appLogic;
        private Persistence persistence;
        public App()
        {
            InitializeComponent();
            persistence = new Persistence();
            appLogic = new AppLogic(persistence);
            MainPage = new MainPage(appLogic);
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
