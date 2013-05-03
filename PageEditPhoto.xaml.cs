using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SnapBook.Windows.Phone.Services;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone
{
    public partial class PageEditPhoto : PhoneApplicationPage
    {
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;

        private ApplicationBarIconButton btnReject;
        private ApplicationBarIconButton btnCrop;
        private ApplicationBarIconButton btnAccept;
        private ApplicationBarIconButton btnHelp;

        //Variable for the help popup
        Popup help = new Popup();

        //Variables for the crop feature    
        Point p1, p2;
        bool cropping = false;

        public PageEditPhoto()
        {
            InitializeComponent();

            TextStatus.Text = "Select the cropping region with your finger." +
                  " Once completed, tap the crop button to crop the image.";

            //Used for rendering the cropping rectangle on the image.
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            btnCrop = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            btnAccept = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            btnReject = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            btnHelp = ApplicationBar.Buttons[2] as ApplicationBarIconButton;

            //Begin storyboard for rectangle color effect.
            Rectangle.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DisplayCroppedImage.Source = _recipePhotoService.RecipeBaseTarget.RecipePhoto.Photo;
            //Sets the source to the Image control on the crop page to the WriteableBitmap object created previously.
            //DisplayedImageElement.Source = _recipePhotoService.RecipeBaseTarget.RecipePhoto.Photo;
        }

        #region Crop
        void close_Click(object sender, RoutedEventArgs e)
        {
            help.IsOpen = false;
        }


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

        void CropImage_MouseMove(object sender, MouseEventArgs e)
        {
            p2 = e.GetPosition(DisplayCroppedImage);
        }

        void CropImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            p2 = e.GetPosition(DisplayCroppedImage);
            cropping = false;
        }

        void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCrop.IsEnabled = true;
            p1 = e.GetPosition(DisplayCroppedImage);
            p2 = p1;
            rect.Visibility = Visibility.Visible;
            cropping = true;
        }
        #endregion

        #region ButtonEventHandlers
        
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

        void btnReject_Click(object sender, EventArgs e)
        {
            //Sets the cropped image back to the original image. For users that want to revert changes.
            DisplayCroppedImage.Source = _recipePhotoService.RecipeBaseTarget.RecipePhoto.Photo;

            //Buttons are disabled and user cannot proceed to use the below until they crop an image again.
            btnCrop.IsEnabled = false;
            btnAccept.IsEnabled = false;
            btnReject.IsEnabled = false;

            //Instructional Text
            TextStatus.Text = "Select the cropping region with your finger." + " Once completed, tap the crop button to crop the image.";

        }

        private void btnScale_Click(object sender, EventArgs e)
        {
            btnAccept.IsEnabled = true;
            btnCrop.IsEnabled = false;
            btnReject.IsEnabled = true;
            btnHelp.IsEnabled = true;

            btnCrop.Click -= new EventHandler(btnEdit_Click);
            btnCrop.Click += new EventHandler(btnEdit_Click);

            DisplayCroppedImageMultiTouch.AreManipulationsEnabled = true;
            DisplayCroppedImageMultiTouch.AreFingersVisible = true;
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            btnAccept.IsEnabled = true;
            btnCrop.IsEnabled = false;
            btnReject.IsEnabled = true;
            btnHelp.IsEnabled = true;


            DisplayCroppedImageMultiTouch.AreManipulationsEnabled = false;
            DisplayCroppedImageMultiTouch.AreFingersVisible = false;
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            //CropImage();

            RecipePhoto sourcePhoto = _recipePhotoService.RecipeBaseTarget.RecipePhoto;

            RecipePhoto targetPhoto = _recipePhotoService.CropImage(sourcePhoto.Photo, DisplayCroppedImage, p1, p2);
            sourcePhoto.Photo = targetPhoto.Photo;

            // Set the source of the image control to the new cropped bitmap
            DisplayCroppedImage.Source = sourcePhoto.Photo;
            rect.Visibility = Visibility.Collapsed;

            //Enable  accept and reject buttons to save or discard current cropped image.
            //Disable crop button until a new cropping region is selected.
            btnAccept.IsEnabled = true;
            btnReject.IsEnabled = true;
            btnCrop.IsEnabled = false;

            //Instructional text
            TextStatus.Text = "Continue to crop image, accept, or reject.";
        }

        void btnAccept_Click(object sender, EventArgs e)
        {
            _recipePhotoService.RecipeBaseTarget.RecipePhoto.Scale = DisplayCroppedImageMultiTouch.Scale;
            ProgressBar.Visibility = Visibility.Visible;
            btnAccept.IsEnabled = false;
            NavigationService.Navigate(new Uri("/PagePreviewImage.xaml", UriKind.Relative));
        }

        #endregion

        private void DisplayCroppedImage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btnReject.IsEnabled = true;
        }



    }
}