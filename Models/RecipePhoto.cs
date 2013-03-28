using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipePhoto
    {
        public WriteableBitmap Photo { get; set; }
        public DateTime CapturedDateTime { get; set; }

        internal static RecipePhoto CreateFrom(WriteableBitmap photo)
        {
            var recipePhoto = new RecipePhoto { Photo = photo, CapturedDateTime = DateTime.Now };

            return recipePhoto;
        }

        internal static RecipePhoto CreateFrom(PhotoResult photoResult)
        {
            var recipePhoto = new RecipePhoto {Photo = PictureDecoder.DecodeJpeg(photoResult.ChosenPhoto), CapturedDateTime = DateTime.Now};
            
            return recipePhoto;
        }
    }
}
