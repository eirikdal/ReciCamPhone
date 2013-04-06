using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Hawaii;
using Microsoft.Hawaii.Ocr.Client;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using ReciCam.Windows.Phone.Models;

namespace ReciCam.Windows.Phone.Services
{
    public class ReciCamOcrService
    {
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;

        /// <summary>
        /// The diagonal of the scaled-down image size.
        /// </summary>
        private const double ImageMaxSizeDiagonal = 600;

        private ReciCamOcrService()
        {
        }

        private void OnOcrRecipeBaseComplete(OcrServiceResult result)
        {
            _recipePhotoService.RecipeBaseTarget.OcrServiceResult = result;
            RecipeBase recipeBaseTarget = _recipePhotoService.RecipeBaseTarget;
            result.OcrResult.OcrTexts.ForEach(ocrText => recipeBaseTarget.Text += recipeBaseTarget.FormatOcrText(ocrText));
        }

        public void StartOcrConversion(RecipeBase recipeBaseTarget)
        {
            var photo = recipeBaseTarget.RecipePhoto.Photo;
            // Images that are too large will take too long to transfer to the Hawaii OCR service.
            // Also images that are too large may contain text that is too big and that will be excluded from the OCR process.
            // If necessary, we will scale-down the image.
            var photoStream = RecipePhotoService.LimitImageSize(photo, ImageMaxSizeDiagonal);

            // Convert the photo stream to bytes.
            byte[] photoBuffer = RecipePhotoService.StreamToByteArray(photoStream);
            
            // Instantiate the service proxy, set the oncomplete event handler, and trigger the asynchronous call.
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                photoBuffer,
                (output) => Deployment.Current.Dispatcher.BeginInvoke(() => OnOcrRecipeBaseComplete(output)));
        }

        // Private 'instance' variable
        static private ReciCamOcrService instance;

        // Public property to get at the single instance
        static public ReciCamOcrService Instance
        {
            get
            {
                // If not created yet, create it
                if (instance == null)
                {
                    instance = new ReciCamOcrService();
                }
                return instance;
            }
        }
    }
}
