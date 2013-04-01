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
            RecipePhotos = new ObservableCollection<RecipePhoto>();
            RecipeBaseTitle = new RecipeBaseTitle();
            RecipeBaseContents = new ObservableCollection<RecipeBaseContent>();
            RecipeBaseIngredients = new ObservableCollection<RecipeBaseIngredient>();
        }

        public ObservableCollection<RecipePhoto> RecipePhotos { get; private set; }
        public RecipeBaseTitle RecipeBaseTitle { get; private set; }
        public ObservableCollection<RecipeBaseContent> RecipeBaseContents { get; private set; }
        public ObservableCollection<RecipeBaseIngredient> RecipeBaseIngredients { get; private set; }
        public RecipeContentType RecipeContentType { get; set; }
        public RecipePhoto PhotoToCrop { get; private set; }
        public RecipePhoto CroppedPhoto { get; set; }

        public void SetPhotoToCrop(RecipePhoto recipePhoto)
        {
            if (RecipeContentType == RecipeContentType.Undefined)
            {
                throw new AccessViolationException("Content type undefined. Set content type before calling this method");
            }

            PhotoToCrop = recipePhoto;
        }

        public Boolean CanFlushCroppedPhoto()
        {
            return (CroppedPhoto != null && (RecipeContentType != RecipeContentType.Undefined));
        }

        public void FlushCroppedPhoto()
        {
            switch (RecipeContentType)
            {
                case RecipeContentType.Ingredient:
                    RecipeBaseIngredients.Add(RecipeBaseIngredient.CreateFrom(PhotoToCrop));
                    break;
                case RecipeContentType.Content:
                    RecipeBaseContents.Add(RecipeBaseContent.CreateFrom(PhotoToCrop));
                    break;
                case RecipeContentType.Title:
                    RecipeBaseTitle.RecipePhoto = CroppedPhoto;
                    break;
            }

            CroppedPhoto = null;
            RecipeContentType = RecipeContentType.Undefined;
        }

        public void AddRecipePhoto(RecipePhoto recipePhoto)
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
