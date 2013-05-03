using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Hawaii;
using Microsoft.Hawaii.Ocr.Client;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using SnapBook.Windows.Phone.Models;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone.Services
{
    public class RecipeOcrService
    {
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;

        private RecipeOcrService()
        {
        }

        public void StartOcrConversion(RecipeBase recipeBaseTarget, ServiceAgent<OcrServiceResult>.OnCompleteDelegate onComplete, int maxSizeDiagonal = 600)
        {
            var photo = recipeBaseTarget.RecipePhoto.Photo;
            // Images that are too large will take too long to transfer to the Hawaii OCR service.
            // Also images that are too large may contain text that is too big and that will be excluded from the OCR process.
            // If necessary, we will scale-down the image.
            var photoStream = RecipePhotoService.LimitImageSize(photo, maxSizeDiagonal);

            // Convert the photo stream to bytes.
            byte[] photoBuffer = RecipePhotoService.StreamToByteArray(photoStream);

            // Instantiate the service proxy, set the oncomplete event handler, and trigger the asynchronous call.
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                photoBuffer,
                (output) => Deployment.Current.Dispatcher.BeginInvoke(() => onComplete(output)), recipeBaseTarget);
        }

        // Private 'instance' variable
        static private RecipeOcrService instance;

        // Public property to get at the single instance
        static public RecipeOcrService Instance
        {
            get
            {
                // If not created yet, create it
                if (instance == null)
                {
                    instance = new RecipeOcrService();
                }
                return instance;
            }
        }
    }
}
