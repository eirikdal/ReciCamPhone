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

namespace OcrSampleAppLite
{
    public partial class MainPage : PhoneApplicationPage
    {
        private CameraCaptureTask ctask;

        public MainPage()
        {
            InitializeComponent();

            ctask = new CameraCaptureTask();
            
            ctask.Completed += CtaskOnCompleted;
        }

        private void CtaskOnCompleted(object sender, PhotoResult photoResult)
        {
            throw new NotImplementedException();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            ctask.Show();
        }
    }
}