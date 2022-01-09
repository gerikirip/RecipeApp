using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace RecipeAppModel
{
    public class AppLogic
    {
        #region Private Variables

        private Persistence persistence;

        #endregion

        #region Properties
        public IEnumerable<Recipe> Recipes { get; private set; }
        public IEnumerable<Recipe> FilteredRecipes { get; private set; }
        public Recipe ClickedRecipe { get; private set; }
        public List<string> SearchIngredients { get; private set; }
        public List<Ingredient> SaveIngredients { get; private set; }
        public List<string> SaveSteps { get; private set; }

        public List<Ingredient> FinishedIngredients { get; private set; }
        public List<string> FinishedSteps { get; private set; }

        #endregion

        #region Events

        public event EventHandler FileIsLoaded;
        public event EventHandler RecipeLoaded;
        public event EventHandler SearchResultsLoaded;
        
        public event EventHandler IngredientListChanged;
        public event EventHandler StepListChanged;

        public event EventHandler RatingChanged;

        public event EventHandler SaveIngredientListChanged;
        public event EventHandler SaveStepsListChanged;
        #endregion

        #region General Functions

        public AppLogic(Persistence per)
        {
            persistence = per;
        }

        public void LoadRecipesData()
        {
            Recipes = persistence.FileLoad();
            FileIsLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void InitFinishedProperties()
        {
            FinishedIngredients = new List<Ingredient>();
            FinishedSteps = new List<string>();      
        }

        public void InitSearchProperties()
        {
            SearchIngredients = new List<string>();
        }

        public void InitSaveProperties()
        {
            SaveIngredients = new List<Ingredient>();
            SaveSteps = new List<string>();
        }

        #endregion

        #region RecipePage Functions

        public void MoveIngredient(Ingredient ing, bool finished)
        {
            if(finished)
            {
                FinishedIngredients.Add(ing);
                IngredientListChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                FinishedIngredients.Remove(ing);
                IngredientListChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public void MoveStep(string step, bool finished)
        {
            if (finished)
            {
                FinishedSteps.Add(step);
                StepListChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                FinishedSteps.Remove(step);
                StepListChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SearchRecipe(string name, bool fav)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToLower().Trim();
                if(Recipes != null)
                {
                    List<Recipe> filteredRecipes = new List<Recipe>();
                    foreach (Recipe recipe in Recipes)
                    {
                        if (recipe.Name.Contains(name))
                        {
                            if((fav && recipe.Favourite) || !fav)
                            {
                                filteredRecipes.Add(recipe);
                            }
                        }
                    }
                    FilteredRecipes = filteredRecipes;
                    SearchResultsLoaded?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (fav)
                {
                    List<Recipe> filteredRecipes = new List<Recipe>();
                    foreach (Recipe recipe in Recipes)
                    {
                        if (recipe.Favourite)
                        {
                            filteredRecipes.Add(recipe);
                        }
                    }
                    FilteredRecipes = filteredRecipes;
                }
                else
                {
                    FilteredRecipes = Recipes;
                }
                SearchResultsLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ClickedRecipeSave(Recipe recipe)
        {
            if (recipe != null)
            {
                ClickedRecipe = recipe;
                RecipeLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SaveFavourite(bool fav)
        {
            ClickedRecipe.Favourite = fav;
            persistence.SaveFile(Recipes.ToList());
        }

        public void SaveRating(double rating)
        {
            ClickedRecipe.Rating = Math.Round((ClickedRecipe.Rating + rating) / 2,1);
            persistence.SaveFile(Recipes.ToList());
            RatingChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IngredientPage Functions
        public void AddToSearchIngredients(string ingredient)
        {
            if (!string.IsNullOrWhiteSpace(ingredient))
            {
                ingredient = ingredient.ToLower().Trim();

                if (!SearchIngredients.Contains(ingredient))
                {
                    SearchIngredients.Add(ingredient);
                }
            }
            IngredientListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveFromSearchIngredients(string ingredient)
        {
            SearchIngredients.Remove(ingredient);
            IngredientListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void FilterByIngredients(bool totalEq, bool fav)
        {
            bool found = false;

            if (SearchIngredients.Count > 0)
            {
                if (totalEq)
                {
                    FilteredRecipes = FilterTotalEq(fav);
                }
                else
                {
                    List<Recipe> filteredRecipes = new List<Recipe>();

                    foreach (Recipe recipe in Recipes)
                    {
                        foreach (string searchIngredient in SearchIngredients)
                        {
                            found = false;
                            foreach (Ingredient ingredient in recipe.Ingredients)
                            {
                                if (searchIngredient == ingredient.Name)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                break;
                            }
                        }
                        if (found)
                        {
                            if ((fav && recipe.Favourite) || !fav)
                            {
                                filteredRecipes.Add(recipe);
                            }
                        }
                    }
                    FilteredRecipes = filteredRecipes;
                }
            }
            else
            {
                if (fav)
                {
                    FilteredRecipes = AllFavouriteRecipes();
                }
                else
                {
                    FilteredRecipes = Recipes;
                }
            }

            SearchResultsLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void FilterByIngredientsCanMake(bool totalEq, bool fav)
        {
            bool found = true;

            if (Recipes != null && SearchIngredients != null)
            {
                if(totalEq)
                {
                    FilteredRecipes = FilterTotalEq(fav);
                }
                else
                {
                    List<Recipe> filteredRecipes = new List<Recipe>();
                    foreach (Recipe recipe in Recipes)
                    {
                        found = true;
                        foreach (Ingredient ingredient in recipe.Ingredients)
                        {
                            if (!SearchIngredients.Contains(ingredient.Name) && found)
                            {
                                found = false;
                            }
                        }
                        if (found)
                        {
                            if((fav && recipe.Favourite) || !fav)
                            {
                                filteredRecipes.Add(recipe);
                            }
                        }
                    }
                    FilteredRecipes = filteredRecipes;
                }
            }
            else
            {
                if (fav)
                {
                    FilteredRecipes = AllFavouriteRecipes();
                }
                else
                {
                    FilteredRecipes = Recipes;
                }
            }

            SearchResultsLoaded?.Invoke(this, EventArgs.Empty);
        }

        private List<Recipe> FilterTotalEq(bool fav)
        {
            bool found = false;

            List<Recipe> filteredRecipes = new List<Recipe>();

            foreach (Recipe recipe in Recipes)
            {
                if (SearchIngredients.Count != recipe.Ingredients.Count)
                {
                    found = false;
                }
                else
                {
                    found = true;
                    SearchIngredients.Sort();
                    recipe.Ingredients.Sort();

                    for (int i = 0; i < SearchIngredients.Count; i++)
                    {
                       if (SearchIngredients[i] != recipe.Ingredients[i].Name)
                       {
                           found = false;
                           break;
                       }
                    }
                }
                if (found)
                {
                    if ((fav && recipe.Favourite) || !fav)
                    {
                        filteredRecipes.Add(recipe);
                    }
                }
            }
            return filteredRecipes;
        }

        private List<Recipe> AllFavouriteRecipes()
        {
            List<Recipe> filteredRecipes = new List<Recipe>();
            foreach (Recipe recipe in Recipes)
            {
                if (recipe.Favourite)
                {
                    filteredRecipes.Add(recipe);
                }
            }
            return filteredRecipes;
        }

        #endregion

        #region NewRecipePage Functions
        public void AddToSaveIngredients(string name, double amount, string unit)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(unit))
            {
                Ingredient ing = new Ingredient();

                ing.Name = name.ToLower().Trim();
                ing.Amount = amount;
                ing.Unit = unit.ToLower().Trim();

                SaveIngredients.Add(ing);
                SaveIngredientListChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RemoveFromSaveIngredients(Ingredient ingredient)
        {
            SaveIngredients.Remove(ingredient);
            SaveIngredientListChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddToSaveSteps(string step)
        {
            if (!string.IsNullOrWhiteSpace(step))
            {
                step = step.ToLower().Trim();
                step = char.ToUpper(step[0]) + step.Substring(1);
                SaveSteps.Add(step);
                SaveStepsListChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RemoveFromSaveSteps(string step)
        {
            SaveSteps.Remove(step);
            SaveStepsListChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool AddRecipe(string name, int prepTime)
        {
            if(!string.IsNullOrWhiteSpace(name) && SaveIngredients.Count>0 && SaveSteps.Count>0)
            {
                Recipe rec = new Recipe();
                rec.Name = name.ToLower().Trim();
                rec.Minutes = prepTime;
                rec.Rating = 5.0;
                rec.Favourite = false;
                rec.Ingredients = SaveIngredients;
                rec.Steps = SaveSteps;
                List<Recipe> recipes = Recipes.ToList();
                recipes.Add(rec);

                Recipes = recipes;

                persistence.SaveFile(Recipes.ToList());

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
