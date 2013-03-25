// -
// <copyright file="MainPage.xaml.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Hawaii;
using Microsoft.Hawaii.Ocr.Client;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace OcrSampleAppLite
{
    public partial class OcrPage : PhoneApplicationPage
    {
        /// <summary>
        /// The diagonal of the scaled-down image size.
        /// </summary>
        private const double ImageMaxSizeDiagonal = 600;

        // cameraCaptureTask is needed to show the Camera Capture Task of the WP7 OS.
        private CameraCaptureTask cameraCaptureTask;

        // showInProgress is used to prevent calling cameraCaptureTask.Show() a second time before the first call to Show returns.
        private bool showInProgress;

        public OcrPage()
        {
            InitializeComponent();

            this.VerifyHawaiiIdentity();

            this.cameraCaptureTask = new CameraCaptureTask();
            this.cameraCaptureTask.Completed += new System.EventHandler<PhotoResult>(this.PhotoChooserCompleted);
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
        private static Stream LimitImageSize(Stream imageStream, double imageMaxDiagonalSize)
        {
            // In order to determine the size of the image we will transfer it to a writable bitmap.
            WriteableBitmap wb = new WriteableBitmap(1, 1);
            wb.SetSource(imageStream);

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
                return imageStream;
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

        /// <summary>
        /// Verify that the Hawaii identity is set correctly.
        /// </summary>
        private void VerifyHawaiiIdentity()
        {
            if (string.IsNullOrEmpty(HawaiiClient.HawaiiApplicationId))
            {
                MessageBox.Show("Service credentials are not set. Please consult the \"Project Hawaii Installation Guide\" on how to obtain credentials");
                throw new Exception("Credentials are not set, exiting application");
            }
        }

        private void TakePicture_Click(object sender, RoutedEventArgs e)
        {
            // Make sure that we don't call this.cameraCaptureTask.Show() a second time before the first Show has returned. 
            // This may happen if we click the btnTakePicture button multiple times very quickly. If that happens the second Show will 
            // throw an InvalidOperationException exception.
            if (!this.showInProgress)
            {
                this.showInProgress = true;
                this.cameraCaptureTask.Show();
            }
        }

        private void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            this.HideAllAreas();
            btnTakePicture.Visibility = Visibility.Visible;
        }

        private void PhotoChooserCompleted(object sender, PhotoResult e)
        {
            this.showInProgress = false;
            if (e.TaskResult == TaskResult.OK)
            {
                this.ShowPhoto(e.ChosenPhoto);
                this.StartOcrConversion(e.ChosenPhoto);
            }
        }

        private void ShowPhoto(Stream photoStream)
        {
            this.HideAllAreas();

            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(photoStream);
            imgPhoto.Source = bmp;

            // Show the photo area for the duration of the OCR process.
            photoArea.Visibility = Visibility.Visible;
        }

        private void StartOcrConversion(Stream photoStream)
        {
            // Images that are too large will take too long to transfer to the Hawaii OCR service.
            // Also images that are too large may contain text that is too big and that will be excluded from the OCR process.
            // If necessary, we will scale-down the image.
            photoStream = OcrPage.LimitImageSize(photoStream, ImageMaxSizeDiagonal);

            // Convert the photo stream to bytes.
            byte[] photoBuffer = OcrPage.StreamToByteArray(photoStream);

            // Instantiate the service proxy, set the oncomplete event handler, and trigger the asynchronous call.
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                photoBuffer,
                (output) =>
                {
                    // This section defines the body of what is known as an anonymous method. 
                    // This anonymous method is the callback method 
                    // called on the completion of the OCR process.
                    // Using Dispatcher.BeginInvoke ensures that 
                    // OnOcrCompleted is invoked on the Main UI thread.
                    this.Dispatcher.BeginInvoke(() => OnOcrCompleted(output));
                });
        }

        private void OnOcrCompleted(OcrServiceResult result)
        {
            this.HideAllAreas();
            resultArea.Visibility = Visibility.Visible;
            if (result.Status == Status.Success)
            {
                int wordCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (OcrText item in result.OcrResult.OcrTexts)
                {
                    wordCount += item.Words.Count;
                    sb.Append(item.Text);
                    sb.Append("\n");
                }

                if (wordCount == 0)
                {
                    txtEmptyResult.Visibility = Visibility.Visible;
                }
                else
                {
                    txtResult.Text = sb.ToString();
                    txtResult.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtError.Text = "[The OCR conversion failed]\n" + result.Exception.Message;
                txtError.Visibility = Visibility.Visible;
            }
        }

        private void HideAllAreas()
        {
            btnTakePicture.Visibility = Visibility.Collapsed;
            photoArea.Visibility = Visibility.Collapsed;
            resultArea.Visibility = Visibility.Collapsed;
            txtResult.Visibility = Visibility.Collapsed;
            txtEmptyResult.Visibility = Visibility.Collapsed;
            txtError.Visibility = Visibility.Collapsed;
        }
    }
}
