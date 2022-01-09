using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppModel
{
    public class Persistence
    {
        public List<Recipe> FileLoad()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "savedRecipes.dat");
            string json = "";

            if (File.Exists(fileName))
            {
                json = File.ReadAllText(fileName);
            }

            if (json == "")
            {
                Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppLogic)).Assembly;
                Stream stream = assembly.GetManifestResourceStream("RecipeAppModel.Data.recipes.json");
                using (StreamReader sr = new StreamReader(stream))
                {
                    json = sr.ReadToEnd();
                }
            }
            List<Recipe> recipes = new List<Recipe>();

            recipes = JsonConvert.DeserializeObject<RecipeList>(json).Recipes;
            return recipes;
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
    }
}
