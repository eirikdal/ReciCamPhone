using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReciCam.Windows.Phone.Models;

namespace ReciCam.Windows.Phone.Services
{
    public class RecipeService
    {
        private RecipeService() { RecipePhotos = new ObservableCollection<RecipePhoto>(); }

        public ObservableCollection<RecipePhoto> RecipePhotos { get; private set; }

        public void AddRecipe(RecipePhoto recipePhoto)
        {
            RecipePhotos.Add(recipePhoto);
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
