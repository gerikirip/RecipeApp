using RecipeAppModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RecipeApp
{
    public partial class MainPage : FlyoutPage
    {
        AppLogic appLogic;
        public MainPage(AppLogic model)
        {
            appLogic = model;

            InitializeComponent();

            Detail = new NavigationPage(new RecipeListPage(appLogic));
            appLogic.RecipeLoaded += Model_RecipeLoaded;

            _flyoutPage._listView.ItemSelected += OnItemSelected;

        }

        private async void Model_RecipeLoaded(object sender, EventArgs e)
        {
            await Detail.Navigation.PushAsync(new RecipePage(appLogic));
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as FlyoutPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetPage,appLogic));
                _flyoutPage._listView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
