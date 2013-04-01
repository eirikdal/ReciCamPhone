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
        public MainPage()
        {
            InitializeComponent();
        }

        private void New_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PageAddContent.xaml", UriKind.Relative));
        }
    }
}