using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public partial class SplitterPivotPage : PhoneApplicationPage
    {
        private CameraCaptureTask ctask;
        private RecipeService recipeService = ((App) Application.Current).RecipeService;

        public SplitterPivotPage()
        {
            InitializeComponent();

            ctask = new CameraCaptureTask();

            ctask.Completed += CtaskOnCompleted;
        }

        private void CtaskOnCompleted(object sender, PhotoResult photoResult)
        {
            ((App)Application.Current).RecipeService.AddRecipePhoto(RecipePhoto.CreateFrom(photoResult));

            NavigationService.Navigate(new Uri("/SplitterPivotPage.xaml", UriKind.Relative));
        }

        private void SplitterPivotPage_OnLoad(object sender, RoutedEventArgs e)
        {
            ListBoxPhotos.ItemsSource = ((App) Application.Current).RecipeService.RecipePhotos;
        }

        private void ButtonNewPhoto_Click(object sender, RoutedEventArgs e)
        {
            ctask.Show();
        }

        private void ButtonAddTitle_Click(object sender, RoutedEventArgs e)
        {
            recipeService.RecipeContentType = RecipeContentType.Title;

            NavigationService.Navigate(new Uri("/OcrPivotPage.xaml", UriKind.Relative));
        }
    }
}