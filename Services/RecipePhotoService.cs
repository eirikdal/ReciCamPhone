using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using SnapBook.Windows.Phone.Models;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone.Services
{
    public class RecipePhotoService
    {
        public RecipeModel Recipe { get; set; }
        public WriteableBitmap OriginalImageSource { get; set; }

        public RecipeBase RecipeBaseTarget { get; set; }

        public RecipePhotoService()
        {
            Recipe = new RecipeModel();
        }

        public RecipePhoto CropImage(WriteableBitmap photo, Image displaydImagElement, Point p1, Point p2)
        {
            // Get the size of the source image captured by the camera
            double originalImageWidth = photo.PixelWidth;
            double originalImageHeight = photo.PixelHeight;

            // Get the size of the image when it is displayed on the phone
            double displayedWidth = displaydImagElement.ActualWidth;
            double displayedHeight = displaydImagElement.ActualHeight;

            // Calculate the ratio of the original image to the displayed image
            double widthRatio = originalImageWidth / displayedWidth;
            double heightRatio = originalImageHeight / displayedHeight;

            // Create a new WriteableBitmap. The size of the bitmap is the size of the cropping rectangle
            // drawn by the user, multiplied by the image size ratio.
            var croppedPhoto = RecipePhoto.CreateFrom(new WriteableBitmap((int)(widthRatio * Math.Abs(p2.X - p1.X)), (int)(heightRatio * Math.Abs(p2.Y - p1.Y))));

            // Calculate the offset of the cropped image. This is the distance, in pixels, to the top left corner
            // of the cropping rectangle, multiplied by the image size ratio.
            int xoffset = (int)(((p1.X < p2.X) ? p1.X : p2.X) * widthRatio);
            int yoffset = (int)(((p1.Y < p2.Y) ? p1.Y : p2.X) * heightRatio);

            // Copy the pixels from the targeted region of the source image into the target image, 
            // using the calculated offset
            for (int i = 0; i < croppedPhoto.Photo.Pixels.Length; i++)
            {
                int x = (int)((i % croppedPhoto.Photo.PixelWidth) + xoffset);
                int y = (int)((i / croppedPhoto.Photo.PixelWidth) + yoffset);
                croppedPhoto.Photo.Pixels[i] = photo.Pixels[y * photo.PixelWidth + x];
            }

            return croppedPhoto;
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
        public static Stream LimitImageSize(WriteableBitmap wb, double imageMaxDiagonalSize)
        {
            // Check if we need to scale it down.
            double imageDiagonalSize = Math.Sqrt(wb.PixelWidth * wb.PixelWidth + wb.PixelHeight * wb.PixelHeight);
            if (imageDiagonalSize > imageMaxDiagonalSize)
            {
                // Calculate the new image size that corresponds to imageMaxDiagonalSize for the 
                // diagonal size and that preserves the aspect ratio.
                int newWidth = (int)(wb.PixelWidth * imageMaxDiagonalSize / imageDiagonalSize);
                int newHeight = (int)(wb.PixelHeight * imageMaxDiagonalSize / imageDiagonalSize);

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

        public static byte[] StreamToByteArray(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];

            long seekPosition = stream.Seek(0, SeekOrigin.Begin);
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            seekPosition = stream.Seek(0, SeekOrigin.Begin);

            return buffer;
        }

        // Private 'instance' variable
        static private RecipePhotoService instance;

        // Public property to get at the single instance
        static public RecipePhotoService Instance
        {
            get
            {
                // If not created yet, create it
                if (instance == null)
                {
                    instance = new RecipePhotoService();
                }
                return instance;
            }
        }
    }
}
