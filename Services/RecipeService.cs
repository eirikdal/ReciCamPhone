using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using SnapBook.Windows.Phone.Models;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone.Services
{
    public class RecipeService
    {
        private RecipeService()
        {   
            Recipes = new ObservableCollection<RecipeViewModel>();
        }

        public ObservableCollection<RecipeViewModel> Recipes { get; set; }

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
