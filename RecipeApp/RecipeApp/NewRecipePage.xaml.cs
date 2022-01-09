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
    public partial class NewRecipePage : ContentPage
    {
        private AppLogic applogic;

        public NewRecipePage(AppLogic model)
        {
            InitializeComponent();

            applogic = model;
            applogic.InitSaveProperties();
        }

        #region Control Event Handlers

        private void _addIngredientButton_Clicked(object sender, EventArgs e)
        {
            double amount = 0;
            if (double.TryParse(_ingAmountEntry.Text, out amount))
            {
                applogic.AddToSaveIngredients(_ingNameEntry.Text, amount, _ingUnitEntry.Text);
            }
        }

        private void _stepEntry_Completed(object sender, EventArgs e)
            => applogic.AddToSaveSteps(_stepEntry.Text);

        private void _addStepButton_Clicked(object sender, EventArgs e)
            => applogic.AddToSaveSteps(_stepEntry.Text);

        private void _addRecipeButton_Clicked(object sender, EventArgs e)
        {
            _message.Children.Clear();
            int prepTime = 0;
            bool result = false;
            if (int.TryParse(_prepTimeEntry.Text, out prepTime))
            {
                result = applogic.AddRecipe(_nameEntry.Text, prepTime);
            }

            Frame frame = new Frame()
            {
                BorderColor = Color.Brown,
                BackgroundColor = Color.Red,
                CornerRadius = 40
            };

            string resultMessage = "";

            if (result)
            {
                frame.BackgroundColor = Color.Green;
                frame.BorderColor = Color.DarkGreen;
                resultMessage = "Recept sikeresen elmentve.";
            }
            else
            {
                resultMessage = "Adjon meg minden adatot, legalább 1 alapanyagot és lépést!";
            }

            Label label = new Label()
            {
                Text = resultMessage,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };

            frame.Content = label;

            _message.Children.Add(frame);

        }

        private void IngredientButton_Clicked(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            Ingredient clickedIngredient = button.BindingContext as Ingredient;
            applogic.RemoveFromSaveIngredients(clickedIngredient);
        }

        private void StepButton_Clicked(object sender, EventArgs e)
        {
            Button button = (sender as Button);
            string clickedStep = button.BindingContext as string;
            applogic.RemoveFromSaveSteps(clickedStep);
        }

        #endregion

        #region Model Event Handlers

        private void model_IngredientListChanged(object sender, EventArgs e)
        {
            _ingredientList.Children.Clear();
            _ingNameEntry.Text = "";
            _ingAmountEntry.Text = "";
            _ingUnitEntry.Text = "";
            if (applogic.SaveIngredients != null)
            {
                foreach (Ingredient ing in applogic.SaveIngredients)
                {
                    Button ingredientButton = new Button()
                    {
                        Text = ing.Amount.ToString() + " " + ing.Unit + " " + ing.Name,
                        BindingContext = ing,
                        Margin = 5,
                        HeightRequest = 40,
                    };
                    ingredientButton.Clicked += IngredientButton_Clicked;

                    _ingredientList.Children.Add(ingredientButton);
                }
            }
        }

        private void model_StepListChanged(object sender, EventArgs e)
        {
            _stepList.Children.Clear();
            _stepEntry.Text = "";
            if (applogic.SaveSteps != null)
            {
                foreach (string step in applogic.SaveSteps)
                {
                    Button stepButton = new Button()
                    {
                        Text = step,
                        BindingContext = step,
                        Margin = 5,
                        HeightRequest = 40,
                    };
                    stepButton.Clicked += StepButton_Clicked;

                    _stepList.Children.Add(stepButton);
                }
            }
        }


        #endregion

        #region Protected Methods

        protected override void OnAppearing()
        {
            base.OnAppearing();
            applogic.SaveIngredientListChanged += model_IngredientListChanged;
            applogic.SaveStepsListChanged += model_StepListChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            applogic.SaveIngredientListChanged -= model_IngredientListChanged;
            applogic.SaveStepsListChanged -= model_StepListChanged;
        }

        #endregion

 
    }
}