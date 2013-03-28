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
using Microsoft.Hawaii.Ocr.Client.ServiceResults;

namespace ReciCam.Windows.Phone.Services
{
    public class ReciCamOcrService
    {
        /// <summary>
        /// The diagonal of the scaled-down image size.
        /// </summary>
        private const double ImageMaxSizeDiagonal = 600;

        private ReciCamOcrService()
        {
        }

        /// <summary>
        /// If the image contained in the imageStream has a diagonal greater than imageMaxSizeDiagonal then 
        /// LimitImageSize will scale-down the image so that its diagonal will be equal to imageMaxSizeDiagonal  
        /// preserving the aspect ratio.
        /// If the image contained in the imageStream has a diagonal less than or equal to imageMaxSizeDiagonal  then 
        /// LimitImageSize will simply return the original stream.
        /// </summary>
        /// <param name="imageStream">
        /// A stream containing the image.
        /// </param>
        /// <param name="imageMaxDiagonalSize">
        /// The maximum value for the diagonal of the image.
        /// </param>
        /// <returns>
        /// It is either the original imageStream or a stream containing a scaled down version of the image.
        /// </returns>
        private static Stream LimitImageSize(WriteableBitmap wb, double imageMaxDiagonalSize)
        {
            // Check if we need to scale it down.
            double imageDiagonalSize = Math.Sqrt(wb.PixelWidth * wb.PixelWidth + wb.PixelHeight * wb.PixelHeight);
            if (imageDiagonalSize > imageMaxDiagonalSize)
            {
                // Calculate the new image size that corresponds to imageMaxDiagonalSize for the 
                // diagonal size and that preserves the aspect ratio.
                int newWidth = (int) (wb.PixelWidth*imageMaxDiagonalSize/imageDiagonalSize);
                int newHeight = (int) (wb.PixelHeight*imageMaxDiagonalSize/imageDiagonalSize);

                Stream resizedStream = null;
                Stream tempStream = null;

                // This try/finally block is needed to avoid CA2000: Dispose objects before losing scope
                // See http://msdn.microsoft.com/en-us/library/ms182289.aspx for more details.
                try
                {
                    tempStream = new MemoryStream();
                    Extensions.SaveJpeg(wb, tempStream, newWidth, newHeight, 0, 100);
                    resizedStream = tempStream;
                    tempStream = null;
                }
                finally
                {
                    if (tempStream != null)
                    {
                        tempStream.Close();
                        tempStream = null;
                    }
                }

                return resizedStream;
            }
            else
            {
                // No need to scale down. The image diagonal is less than or equal to imageMaxSizeDiagonal.
                var tempStream = new MemoryStream();
                Extensions.SaveJpeg(wb, tempStream, wb.PixelWidth, wb.PixelHeight, 0, 100);

                return tempStream;
            }
        }

        private static byte[] StreamToByteArray(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];

            long seekPosition = stream.Seek(0, SeekOrigin.Begin);
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            seekPosition = stream.Seek(0, SeekOrigin.Begin);

            return buffer;
        }

        public void StartOcrConversion(WriteableBitmap photo, ServiceAgent<OcrServiceResult>.OnCompleteDelegate onComplete)
        {
            // Images that are too large will take too long to transfer to the Hawaii OCR service.
            // Also images that are too large may contain text that is too big and that will be excluded from the OCR process.
            // If necessary, we will scale-down the image.
            var photoStream = LimitImageSize(photo, ImageMaxSizeDiagonal);

            // Convert the photo stream to bytes.
            byte[] photoBuffer = StreamToByteArray(photoStream);

            // Instantiate the service proxy, set the oncomplete event handler, and trigger the asynchronous call.
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                photoBuffer,
                (output) => Deployment.Current.Dispatcher.BeginInvoke(() => onComplete(output)));
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
