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
    public partial class RecipePage : ContentPage
    {
        private AppLogic appLogic;

        public RecipePage(AppLogic model)
        {
            InitializeComponent();

            appLogic = model;

            appLogic.InitFinishedProperties();

            if(appLogic.ClickedRecipe != null)
            {
                _titleLabel.Text = appLogic.ClickedRecipe.Name.ToUpper();
                _prepTime.Text = appLogic.ClickedRecipe.Minutes.ToString();
                _rating.Text = appLogic.ClickedRecipe.Rating.ToString();
                _favCheck.IsChecked = appLogic.ClickedRecipe.Favourite;

                foreach(Ingredient ing in appLogic.ClickedRecipe.Ingredients)
                {
                    AddNewIngredient(appLogic.FinishedIngredients.Contains(ing), ing);
                }
                int i = 1;
                foreach (string step in appLogic.ClickedRecipe.Steps)
                {
                    AddNewStep(appLogic.FinishedSteps.Contains(step), step,i);
                    i++;
                }
            }
        }

        #region Private Methods

        private void AddNewStep(bool finished, string step, int count)
        {
           Label label = new Label()
            {
                Text = count.ToString() + ". " + step,
                HorizontalOptions = LayoutOptions.Center,
            };

            CheckBox checkBox = new CheckBox()
            {
                Color = Color.Black,
                BindingContext = step
            };

            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };

            if (finished)
            {
                checkBox.IsChecked = true;
                label.TextDecorations = TextDecorations.Strikethrough;
                checkBox.CheckedChanged += _step_CheckedChanged;
                stack.Children.Add(checkBox);
                stack.Children.Add(label);
                _finished_steps.Children.Add(stack);
            }
            else
            {
                checkBox.CheckedChanged += _step_CheckedChanged;
                stack.Children.Add(checkBox);
                stack.Children.Add(label);
                _steps.Children.Add(stack);
            }
        }

        private void AddNewIngredient(bool finished, Ingredient ing)
        {
            Label label = new Label()
            {
                Text = ing.Amount.ToString() + " " + ing.Unit + " " + ing.Name,
                HorizontalOptions = LayoutOptions.Center,
            };

            CheckBox checkBox = new CheckBox()
            {
                Color = Color.Black,
                BindingContext = ing
            };

            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };

            if (finished)
            {
                checkBox.IsChecked = true;
                label.TextDecorations = TextDecorations.Strikethrough;
                checkBox.CheckedChanged += _ing_CheckedChanged;
                stack.Children.Add(checkBox);
                stack.Children.Add(label);
                _finished_ings.Children.Add(stack);
            }
            else
            {
                checkBox.CheckedChanged += _ing_CheckedChanged;
                stack.Children.Add(checkBox);
                stack.Children.Add(label);
                _ingredients.Children.Add(stack);
            }
        }

        #endregion

        #region Control Event Handlers

        private void _step_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            string step = check.BindingContext as string;
            appLogic.MoveStep(step, check.IsChecked);
        }

        private void _favCheck_CheckedChanged(object sender, CheckedChangedEventArgs e)
            => appLogic.SaveFavourite(_favCheck.IsChecked);

        private void _ing_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            Ingredient ing = check.BindingContext as Ingredient;
            appLogic.MoveIngredient(ing, check.IsChecked);
        }

        private void rateButton_Clicked(object sender, EventArgs e)
            => appLogic.SaveRating(_ratingStepper.Value);

        #endregion

        #region Model Event Handlers

        private void model_RatingChanged(object sender, EventArgs e)
        {
            _rating.Text = appLogic.ClickedRecipe.Rating.ToString();

        }

        private void model_IngredientListChanged(object sender, EventArgs e)
        {
            _ingredients.Children.Clear();
            _finished_ings.Children.Clear();

            foreach (Ingredient ing in appLogic.ClickedRecipe.Ingredients)
            {
                AddNewIngredient(appLogic.FinishedIngredients.Contains(ing), ing);
            }
        }

        private void model_StepListChanged(object sender, EventArgs e)
        {
            _steps.Children.Clear();
            _finished_steps.Children.Clear();

            int i = 1;
            foreach (string step in appLogic.ClickedRecipe.Steps)
            {
                AddNewStep(appLogic.FinishedSteps.Contains(step), step, i);
                i++;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            appLogic.RatingChanged += model_RatingChanged;
            appLogic.IngredientListChanged += model_IngredientListChanged;
            appLogic.StepListChanged += model_StepListChanged;
        }



        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            appLogic.RatingChanged -= model_RatingChanged;
            appLogic.IngredientListChanged -= model_IngredientListChanged;
            appLogic.StepListChanged -= model_StepListChanged;
        }

        #endregion
    }
}