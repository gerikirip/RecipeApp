using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RecipeAppPersistence
{
    public class JSONPersistence
    {
        public async Task<List<Recipe>> FileLoad()
        {
            try
            {
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "savedRecipes.dat");
                string json = "";

                List<Recipe> recipes = new List<Recipe>();
                await Task.Run(() =>
                {
                    if (File.Exists(fileName))
                    {
                        json = File.ReadAllText(fileName);
                        recipes = JsonConvert.DeserializeObject<RecipeList>(json).Recipes;
                    }
                });

                return recipes;
            }
            catch { return null; }
        }

        public async void SaveFile(List<Recipe> recipes)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "savedRecipes.dat");

            try
            {
                RecipeList recList = new RecipeList();
                recList.Recipes = recipes;

                string json = JsonConvert.SerializeObject(recList);

                await Task.Run(() => File.WriteAllText(fileName, json));
            }
            catch { }

        }

        public bool SavedFileExists()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "savedRecipes.dat");
            return (File.Exists(fileName));
        }
    }
}
