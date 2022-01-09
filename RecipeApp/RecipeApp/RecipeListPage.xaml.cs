using RecipeAppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RecipeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecipeListPage : ContentPage
    {
        private AppLogic appLogic;

        public RecipeListPage(AppLogic appLogic)
        {
            InitializeComponent();

            this.appLogic = appLogic;
            appLogic.FileIsLoaded += model_FileIsLoaded;
            appLogic.LoadRecipesData();
        }

        #region Control Event Handlers


        private void _searchButton_Clicked(object sender, EventArgs e)
            => appLogic.SearchRecipe(_searchEntry.Text,_favCheck.IsChecked);
        private void _searchEntry_Completed(object sender, EventArgs e)
            => appLogic.SearchRecipe(_searchEntry.Text,_favCheck.IsChecked);


        private void RecipeResultButton_Clicked(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            Recipe clickedRecipe = button.BindingContext as Recipe;
            appLogic.ClickedRecipeSave(clickedRecipe);
        }

        #endregion

        #region Model Event Handlers
        private void model_FileIsLoaded(object sender, EventArgs e)
        {
            if (appLogic.Recipes != null)
                foreach (Recipe result in appLogic.Recipes)
                {
                    Button recipeResultButton = new Button()
                    {
                        Text = result.Name,
                        BindingContext = result,
                    };
                    recipeResultButton.Clicked += RecipeResultButton_Clicked;

                    _recipeList.Children.Add(recipeResultButton);
                }
        }

        private void model_SearchResultsLoaded(object sender, EventArgs e)
        {
            _recipeList.Children.Clear();
            if (appLogic.FilteredRecipes != null)
            {
                foreach (Recipe result in appLogic.FilteredRecipes)
                {
                    Button recipeResultButton = new Button()
                    {
                        Text = result.Name,
                        BindingContext = result,
                    };
                    recipeResultButton.Clicked += RecipeResultButton_Clicked;

                    _recipeList.Children.Add(recipeResultButton);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            appLogic.SearchResultsLoaded += model_SearchResultsLoaded;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            appLogic.SearchResultsLoaded -= model_SearchResultsLoaded;
        }

        #endregion
    }
}