using System;
using System.Collections.Generic;

namespace RecipeAppPersistence
{
    public class Recipe
    {
        public int RecipeId;
        public string Name;
        public int Minutes;
        public bool Favourite;
        public double Rating;
        public List<Ingredient> Ingredients;
        public List<string> Steps;
    }
}
