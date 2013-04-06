using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Hawaii.Ocr.Client.Model;

namespace ReciCam.Windows.Phone.Models
{
    public class Recipe
    {
        public RecipeBase RecipeBaseTitle { get; set; }
        public ObservableCollection<RecipeBase> RecipeBaseContents { get; set; }
        public ObservableCollection<RecipeBase> RecipeBaseIngredients { get; set; }
        public ObservableCollection<RecipePhoto> RecipePhotos { get; set; }

        public static Recipe CreateFrom(RecipeBase recipeBaseTitle,
                                 ObservableCollection<RecipeBase> recipeBaseIngredients,
                                 ObservableCollection<RecipeBase> recipeBaseContents)
        {
            Recipe recipe = new Recipe();
            recipe.RecipeBaseTitle = recipeBaseTitle;
            recipe.RecipeBaseIngredients = recipeBaseIngredients;
            recipe.RecipeBaseContents = recipeBaseContents;
            return recipe;
        }
    }
}
