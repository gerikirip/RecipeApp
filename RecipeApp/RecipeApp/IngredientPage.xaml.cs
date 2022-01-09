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
    public partial class IngredientPage : ContentPage
    {
        private AppLogic appLogic;
        public IngredientPage(AppLogic model)
        {
            InitializeComponent();
            appLogic = model;
            appLogic.InitSearchProperties();
        }

        #region Control Event Handlers

        private void _addButton_Clicked(object sender, EventArgs e)
            => appLogic.AddToSearchIngredients(_addEntry.Text);

        private void _addEntry_Completed(object sender, EventArgs e)
            => appLogic.AddToSearchIngredients(_addEntry.Text);

        private void _searchButton_Clicked(object sender, EventArgs e)
            => appLogic.FilterByIngredients(_totalEqCheck.IsChecked,_favCheck.IsChecked);

        private void _canMakeButton_Clicked(object sender, EventArgs e)
            => appLogic.FilterByIngredientsCanMake(_totalEqCheck.IsChecked, _favCheck.IsChecked);

        private void IngredientButton_Clicked(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            string clickedIngredient = button.BindingContext as string;
            appLogic.RemoveFromSearchIngredients(clickedIngredient);
        }

        private void RecipeResultButton_Clicked(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            Recipe clickedRecipe = button.BindingContext as Recipe;
            appLogic.ClickedRecipeSave(clickedRecipe);
        }

        #endregion

        #region Model Event Handlers

        private void model_IngredientListChanged(object sender, EventArgs e)
        {
            _ingredintList.Children.Clear();
            _addEntry.Text = "";
            if (appLogic.SearchIngredients != null)
            {
                foreach (string result in appLogic.SearchIngredients)
                {
                    Button ingredientButton = new Button()
                    {
                        Text = result,
                        BindingContext = result,
                        Margin=5,
                        HeightRequest=40,
                    };
                    ingredientButton.Clicked += IngredientButton_Clicked;

                    _ingredintList.Children.Add(ingredientButton);
                }
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
            appLogic.IngredientListChanged += model_IngredientListChanged;
            appLogic.SearchResultsLoaded += model_SearchResultsLoaded;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            appLogic.IngredientListChanged -= model_IngredientListChanged;
            appLogic.SearchResultsLoaded -= model_SearchResultsLoaded;
        }


        #endregion

    }
}