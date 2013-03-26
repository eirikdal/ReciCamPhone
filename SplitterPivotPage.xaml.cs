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

namespace ReciCam.Windows.Phone
{
    public partial class SplitterPivotPage : PhoneApplicationPage
    {
        private CameraCaptureTask ctask;

        public SplitterPivotPage()
        {
            InitializeComponent();

            ctask = new CameraCaptureTask();

            ctask.Completed += CtaskOnCompleted;
        }

        private void CtaskOnCompleted(object sender, PhotoResult photoResult)
        {
            ((App)Application.Current).RecipeService.AddRecipe(RecipePhoto.CreateFrom(photoResult));

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
            throw new NotImplementedException();
        }
    }
}