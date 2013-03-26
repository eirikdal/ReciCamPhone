using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ReciCam.Windows.Phone
{
    public partial class SplitterPivotPage : PhoneApplicationPage
    {
        public SplitterPivotPage()
        {
            InitializeComponent();
        }

        private void SplitterPivotPage_OnLoad(object sender, RoutedEventArgs e)
        {
            ListBoxPhotos.ItemsSource = ((App) Application.Current).RecipeService.RecipePhotos;
        }
    }
}