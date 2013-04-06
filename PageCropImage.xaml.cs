using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using ReciCam.Windows.Phone.Models;
using ReciCam.Windows.Phone.Services;

namespace ReciCam.Windows.Phone
{
    public partial class OcrCropImage : PhoneApplicationPage
    {
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;
        private readonly ReciCamOcrService _reciCamOcrService = ((App)Application.Current).ReciCamOcrService;

        private ApplicationBarIconButton btnReject;
        private ApplicationBarIconButton btnCrop;
        private ApplicationBarIconButton btnAccept;
        private ApplicationBarIconButton btnHelp;

        //Variable for the help popup
        Popup help = new Popup();

        //Variables for the crop feature    
        Point p1, p2;
        bool cropping = false;

        public OcrCropImage()
        {
            InitializeComponent();

            btnCrop = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            btnAccept = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            btnReject = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
            btnHelp = ApplicationBar.Buttons[3] as ApplicationBarIconButton;

            TextStatus.Text = "Select the cropping region with your finger." +
                              " Once completed, tap the crop button to crop the image.";

            //Sets the source to the Image control on the crop page to the WriteableBitmap object created previously.
            var recipePhoto = _recipePhotoService.RecipeBaseTarget.RecipePhoto;
            DisplayedImageElement.Source = recipePhoto.Photo;

            //Create event handlers for cropping selection on the picture.
            DisplayedImageElement.MouseLeftButtonDown += new MouseButtonEventHandler(CropImage_MouseLeftButtonDown);
            DisplayedImageElement.MouseLeftButtonUp += new MouseButtonEventHandler(CropImage_MouseLeftButtonUp);
            DisplayedImageElement.MouseMove += new MouseEventHandler(CropImage_MouseMove);

            //Used for rendering the cropping rectangle on the image.
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            //Begin storyboard for rectangle color effect.
            Rectangle.Begin();
        }

        void btnHelp_Click(object sender, EventArgs e)
        {
            // Create a popup/message box for help and add content to the popup
            //Stack panel with a black background
            StackPanel panelHelp = new StackPanel();
            panelHelp.Background = new SolidColorBrush(Colors.Black);
            panelHelp.Width = 400;
            panelHelp.Height = 550;

            //Create a white border
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(7.0);

            //Create a close button to exit popup
            Button close = new Button();
            close.Content = "Close";
            close.Margin = new Thickness(5.0);
            close.Click += new RoutedEventHandler(close_Click);


            //Create helper text
            TextBlock textblockHelp = new TextBlock();
            textblockHelp.FontSize = 24;
            textblockHelp.Foreground = new SolidColorBrush(Colors.White);
            textblockHelp.TextWrapping = TextWrapping.Wrap;
            textblockHelp.Text = "Use your finger on the image to define a cropping region." + " Once the region is selected, as seen with a rectangle, tap the crop button to crop the image." + " You may choose to save this image in the media library by tapping the check button on the application bar, or reject the cropping and return to the original image with the cancel button (X).";
            textblockHelp.Margin = new Thickness(5.0);

            //Add controls to stack panel
            panelHelp.Children.Add(textblockHelp);
            panelHelp.Children.Add(close);
            border.Child = panelHelp;

            // Set the Child property of Popup to the border 
            // that contains a stackpanel, textblock and button.
            help.Child = border;

            // Set where the popup will show up on the screen.   
            help.VerticalOffset = 150;
            help.HorizontalOffset = 40;

            // Open the popup.
            help.IsOpen = true;

        }


        /// <summary>
        /// Click event handler for the close button on the help popup.
        /// This will create a popup/message box for help and add content to the popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void close_Click(object sender, RoutedEventArgs e)
        {
            help.IsOpen = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (cropping)
            {

                rect.SetValue(Canvas.LeftProperty, (p1.X < p2.X) ? p1.X : p2.X);
                rect.SetValue(Canvas.TopProperty, (p1.Y < p2.Y) ? p1.Y : p2.Y);
                rect.Width = (int)Math.Abs(p2.X - p1.X);
                rect.Height = (int)Math.Abs(p2.Y - p1.Y);
            }
        }


        /// <summary>
        /// Click event handler for mouse move.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CropImage_MouseMove(object sender, MouseEventArgs e)
        {
            p2 = e.GetPosition(DisplayedImageElement);
        }

        /// <summary>
        /// Click event handler for mouse left button up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CropImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            p2 = e.GetPosition(DisplayedImageElement);
            cropping = false;


        }

        /// <summary>
        /// Click event handler for mouse left button down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCrop.IsEnabled = true;
            p1 = e.GetPosition(DisplayedImageElement);
            p2 = p1;
            rect.Visibility = Visibility.Visible;
            cropping = true;
        }

        /// <summary>
        /// Click event handler for the reject button on the application bar.
        /// This will allow you to reject the cropped image and set back to the original image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void btnReject_Click(object sender, EventArgs e)
        {
            //Sets the cropped image back to the original image. For users that want to revert changes.
            DisplayedImageElement.Source = _recipePhotoService.RecipeBaseTarget.RecipePhoto.Photo;

            //Buttons are disabled and user cannot proceed to use the below until they crop an image again.
            btnCrop.IsEnabled = false;
            btnAccept.IsEnabled = false;
            btnReject.IsEnabled = false;

            //Instructional Text
            TextStatus.Text = "Select the cropping region with your finger." + " Once completed, tap the crop button to crop the image.";

        }

        /// <summary>
        /// Click event handler for the accept button on the application bar.
        /// Saves cropped image to isolated storage, then into
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void btnAccept_Click(object sender, EventArgs e)
        {
            //Make progress bar visible for the event handler as there may be posible latency.
            ProgressBar.Visibility = Visibility.Visible;

            _reciCamOcrService.StartOcrConversion(_recipePhotoService.RecipeBaseTarget);

            NavigationService.Navigate(new Uri("/PageAddContent.xaml", UriKind.Relative));
        }


        /// <summary>
        /// Click event handler for the crop button on the application bar.
        /// This code creates the new cropped writeable bitmap object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCrop_Click(object sender, EventArgs e)
        {
            RecipePhoto sourcePhoto = _recipePhotoService.RecipeBaseTarget.RecipePhoto;

            RecipePhoto targetPhoto = _recipePhotoService.CropImage(sourcePhoto.Photo, DisplayedImageElement, p1, p2);

            sourcePhoto.Photo = targetPhoto.Photo;

            // Set the source of the image control to the new cropped bitmap
            DisplayedImageElement.Source = sourcePhoto.Photo;
            rect.Visibility = Visibility.Collapsed;

            //Enable  accept and reject buttons to save or discard current cropped image.
            //Disable crop button until a new cropping region is selected.
            btnAccept.IsEnabled = true;
            btnReject.IsEnabled = true;
            btnCrop.IsEnabled = false;

            //Instructional text
            TextStatus.Text = "Continue to crop image, accept, or reject.";
        }

    }
}