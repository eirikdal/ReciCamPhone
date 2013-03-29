using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ReciCam.Windows.Phone.Models;
using ReciCam.Windows.Phone.Services;

namespace ReciCam.Windows.Phone
{
    public partial class OcrPivotPage : PhoneApplicationPage
    {
        private RecipeService recipeService = ((App) Application.Current).RecipeService;

        private ApplicationBarIconButton btnForward;

        public OcrPivotPage()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            btnForward = new ApplicationBarIconButton(new Uri("/Assets/ModernUI/appbar.arrow.right.png", UriKind.Relative));
            btnForward.Text = "New";
            btnForward.Click += new EventHandler(ButtonSelectPhoto_Click);
            btnForward.IsEnabled = true;

            ApplicationBar.Buttons.Add(btnForward);
        }

        private void OcrPivotPage_Load(object sender, RoutedEventArgs e)
        {
            ListPickerPhotos.ItemsSource = recipeService.RecipePhotos;
        }

        private void ButtonSelectPhoto_Click(object sender, EventArgs e)
        {
            recipeService.SetPhotoToCrop((RecipePhoto) ListPickerPhotos.SelectedItem);

            NavigationService.Navigate(new Uri("/PageCropImage.xaml", UriKind.Relative));            
        }
    }
}