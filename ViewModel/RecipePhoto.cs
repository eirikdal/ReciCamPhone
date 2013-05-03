using System;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;

namespace SnapBook.Windows.Phone.ViewModel
{
    public class RecipePhoto
    {
        private RecipePhoto() {}
        public WriteableBitmap Photo { get; set; }
        public DateTime CapturedDateTime { get; set; }
        public double Scale { get; set; }

        internal static RecipePhoto CreateFrom(WriteableBitmap photo)
        {
            var recipePhoto = new RecipePhoto { Photo = photo, CapturedDateTime = DateTime.Now, Scale = 100.0f};

            return recipePhoto;
        }

        internal static RecipePhoto CreateFrom(PhotoResult photoResult)
        {
            var recipePhoto = new RecipePhoto {Photo = PictureDecoder.DecodeJpeg(photoResult.ChosenPhoto), CapturedDateTime = DateTime.Now, Scale = 100.0f};
            
            return recipePhoto;
        }
    }
}
