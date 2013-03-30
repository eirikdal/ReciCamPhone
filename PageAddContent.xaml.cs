using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using ReciCam.Windows.Phone.Models;
using ReciCam.Windows.Phone.Services;

namespace ReciCam.Windows.Phone
{
    public partial class PageAddContent : PhoneApplicationPage
    {
        private readonly ReciCamOcrService _reciCamOcrService = ((App) Application.Current).ReciCamOcrService;
        private CameraCaptureTask ctask;
        private readonly RecipeService _recipeService = ((App) Application.Current).RecipeService;

        private ApplicationBarIconButton btnNew;
        private ApplicationBarIconButton btnDone;

        public PageAddContent()
        {
            InitializeComponent();

            ctask = new CameraCaptureTask();

            ctask.Completed += CtaskOnCompleted;

            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            btnNew = new ApplicationBarIconButton(new Uri("/Assets/ModernUI/appbar.camera.png", UriKind.Relative));
            btnNew.Text = "New";
            btnNew.Click += new EventHandler(ButtonNewPhoto_Click);
            btnNew.IsEnabled = true;

            btnDone = new ApplicationBarIconButton(new Uri("/Assets/ModernUI/appbar.check.png", UriKind.Relative));
            btnDone.Text = "Done";
            btnDone.Click += new EventHandler(ButtonDone_Click);
            btnDone.IsEnabled = true;

            ApplicationBar.Buttons.Add(btnNew);
            ApplicationBar.Buttons.Add(btnDone);
        }

        private void ButtonDone_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Not implemented yet");
            //NavigationService.Navigate(new Uri("/PageEditContent.xaml", UriKind.Relative));
        }

        private void CtaskOnCompleted(object sender, PhotoResult photoResult)
        {
            ((App)Application.Current).RecipeService.AddRecipePhoto(RecipePhoto.CreateFrom(photoResult));

            NavigationService.Navigate(new Uri("/PageAddContent.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ImageTitle.DataContext = _recipeService.RecipeBaseTitle;
            ListBoxAddedPhotos.ItemsSource = _recipeService.RecipeBaseContents;
            ListBoxPhotos.ItemsSource = _recipeService.RecipePhotos;

            _reciCamOcrService.StartOcrConversion(_recipeService.RecipeBaseTitle.RecipePhoto.Photo, OnTitleCompleted);
            _requestCounter = 1;

            foreach (RecipeBaseContent recipeBaseContent in _recipeService.RecipeBaseContents)
            {
                _reciCamOcrService.StartOcrConversion(recipeBaseContent.RecipePhoto.Photo, OnOcrCompleted);
                _requestCounter++;
            }
        }

        private void PageAddContent_OnLoad(object sender, RoutedEventArgs e)
        {
            if (_recipeService.CanFlushCroppedPhoto())
            {
                _recipeService.FlushCroppedPhoto();
            }
        }

        private void ButtonNewPhoto_Click(object sender, EventArgs e)
        {
            ctask.Show();
        }

        private void ButtonAddTitle_Click(object sender, EventArgs e)
        {
            RecipePhoto photoToCrop = null;

            if (ListBoxPhotos.SelectedItem == null)
            {
                photoToCrop = (RecipePhoto)ListBoxPhotos.Items[0];
            }
            else
            {
                photoToCrop = (RecipePhoto)ListBoxPhotos.SelectedItem;
            }

            _recipeService.RecipeContentType = RecipeContentType.Title;
            _recipeService.SetPhotoToCrop(photoToCrop);

            NavigationService.Navigate(new Uri("/PageCropImage.xaml", UriKind.Relative));
        }
    }
}