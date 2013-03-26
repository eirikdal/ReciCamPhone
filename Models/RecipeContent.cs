using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipeContent
    {
        public RecipePhoto RecipePhoto { get; private set; }
        public RecipeContentType RecipeContentType { get; private set; }

        public static RecipeContent createFrom(RecipePhoto photoToCrop, RecipeContentType recipeContentType)
        {
            var recipeContent = new RecipeContent();
            recipeContent.RecipeContentType = recipeContentType;
            recipeContent.RecipePhoto = photoToCrop;
            return recipeContent;
        }
    }
}
