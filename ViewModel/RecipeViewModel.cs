using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SnapBook.Windows.Phone.Models;

namespace SnapBook.Windows.Phone.ViewModel
{
    public class RecipeViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.
        private RecipeDataContext recipeDb;

        // Class constructor, create the data context object.
        public RecipeViewModel(string toDoDBConnectionString)
        {
            recipeDb = new RecipeDataContext(toDoDBConnectionString);
        }

        private ObservableCollection<RecipeModel> _recipeModels;
        public ObservableCollection<RecipeModel> RecipeModels
        {
            get { return _recipeModels; }
            set
            {
                _recipeModels = value;
                NotifyPropertyChanged("RecipeModels");
            }
        }

        private ObservableCollection<RecipeContent> _recipeContents;
        public ObservableCollection<RecipeContent> RecipeContents
        {
            get { return _recipeContents; }
            set
            {
                _recipeContents = value;
                NotifyPropertyChanged("RecipeContents");
            }
        }

        private ObservableCollection<RecipeIngredient> _recipeIngredients;
        public ObservableCollection<RecipeIngredient> RecipeIngredients
        {
            get { return _recipeIngredients; }
            set
            {
                _recipeIngredients = value;
                NotifyPropertyChanged("RecipeIngredients");
            }
        }

        private ObservableCollection<RecipeTitle> _recipeTitles;
        public ObservableCollection<RecipeTitle> RecipeTitles
        {
            get { return _recipeTitles; }
            set
            {
                _recipeTitles = value;
                NotifyPropertyChanged("RecipeTitles");
            }
        }

        private ObservableCollection<RecipeDescription> _recipeDescriptions;
        public ObservableCollection<RecipeDescription> RecipeDescriptions
        {
            get { return _recipeDescriptions; }
            set
            {
                _recipeDescriptions = value;
                NotifyPropertyChanged("RecipeDescriptions");
            }
        }

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            recipeDb.SubmitChanges();
        }

        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all to-do items in the database.
            var recipes = from RecipeModel recipeModel in recipeDb.Recipes
                                select recipeModel;

            // Query the database and load all to-do items.
            RecipeModels = new ObservableCollection<RecipeModel>(recipes);

            // Specify the query for all categories in the database.
            var ingredients = from RecipeIngredient ingredient in recipeDb.Ingredients
                              select ingredient;

            RecipeIngredients = new ObservableCollection<RecipeIngredient>(ingredients);

            var titles = from RecipeTitle title in recipeDb.Titles
                              select title;

            RecipeTitles = new ObservableCollection<RecipeTitle>(titles);

            var contents = from RecipeContent content in recipeDb.Contents
                              select content;

            RecipeContents = new ObservableCollection<RecipeContent>(contents);

            var descriptions = from RecipeDescription description in recipeDb.Descriptions
                              select description;

            RecipeDescriptions = new ObservableCollection<RecipeDescription>(descriptions);
        }


        public void AddRecipeModel(RecipeModel recipeModel)
        {
            recipeDb.Recipes.InsertOnSubmit(recipeModel);

            recipeDb.SubmitChanges();

            RecipeModels.Add(recipeModel);
            
            foreach (var recipeIngredient in recipeModel.Ingredients)
            {
                RecipeIngredients.Add(recipeIngredient);
            }
            foreach (var recipeContent in recipeModel.Contents)
            {
                RecipeContents.Add(recipeContent);
            }
            RecipeTitles.Add(recipeModel.Title);
            RecipeDescriptions.Add(recipeModel.Description);
        }

        // Remove a to-do task item from the database and collections.
        public void DeleteRecipeModel(RecipeModel recipeToDelete)
        {

            // Remove the to-do item from the "all" observable collection.
            RecipeModels.Remove(recipeToDelete);

            // Remove the to-do item from the data context.
            recipeDb.Recipes.DeleteOnSubmit(recipeToDelete);

            // Save changes to the database.
            recipeDb.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
