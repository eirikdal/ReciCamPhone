using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ReciCam.Windows.Phone.Models;

namespace ReciCam.Windows.Phone.Services
{
    public class RecipeService
    {
        private RecipeService()
        {   
        }

        public ObservableCollection<Recipe> Recipes { get; set; }

        public static Recipe CreateRecipe(RecipeBase recipeBaseTitle, ObservableCollection<RecipeBase> recipeBaseIngredients, ObservableCollection<RecipeBase> recipeBaseContents)
        {
            return Recipe.CreateFrom(recipeBaseTitle, recipeBaseIngredients, recipeBaseContents);
        }

        // Private 'instance' variable
        static private RecipeService instance;

        // Public property to get at the single instance
        static public RecipeService Instance
        {
            get
            {
                // If not created yet, create it
                if (instance == null)
                {
                    instance = new RecipeService();
                }
                return instance;
            }
        }
    }
}
