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
using RestSharp;

namespace ReciCam.Windows.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private CameraCaptureTask ctask;

        private ApplicationBarIconButton btnNew;

        public MainPage()
        {
            InitializeComponent();

            ctask = new CameraCaptureTask();
            ctask.Completed += CtaskOnCompleted;

            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            btnNew = new ApplicationBarIconButton(new Uri("/Assets/ModernUI/appbar.new.png", UriKind.Relative));
            btnNew.Text = "New";
            ApplicationBar.Buttons.Add(btnNew);
            btnNew.Click += new EventHandler(New_Click);
            btnNew.IsEnabled = true;

        }

        private void CtaskOnCompleted(object sender, PhotoResult photoResult)
        {
            ((App)Application.Current).RecipeService.AddRecipePhoto(RecipePhoto.CreateFrom(photoResult));

            NavigationService.Navigate(new Uri("/PageAddContent.xaml", UriKind.Relative));
        }

        private void New_Click(object sender, EventArgs e)
        {
            ctask.Show();
        }
    }
}