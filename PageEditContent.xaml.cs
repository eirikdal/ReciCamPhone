using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ReciCam.Windows.Phone.Models;
using ReciCam.Windows.Phone.Services;

namespace ReciCam.Windows.Phone
{
    public partial class PageEditContent : PhoneApplicationPage
    {
        private int _requestCounter = 0;

        private readonly ReciCamOcrService _reciCamOcrService = ((App) Application.Current).ReciCamOcrService;
        private readonly RecipeService _recipeService = ((App) Application.Current).RecipeService;

        public PageEditContent()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Make progress bar visible for the event handler as there may be posible latency.
            ProgressBar.Visibility = Visibility.Visible;

            _reciCamOcrService.StartOcrConversion(_recipeService.RecipeBaseTitle.RecipePhoto.Photo, OnTitleCompleted);
            _requestCounter = 1;
            
            foreach (RecipeBaseContent recipeBaseContent in _recipeService.RecipeBaseContents)
            {
                _reciCamOcrService.StartOcrConversion(recipeBaseContent.RecipePhoto.Photo, OnOcrCompleted);
                _requestCounter++;
            }

        }

        private void UpdateRequestCounter()
        {
            if (--_requestCounter == 0)
            {
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTitleCompleted(OcrServiceResult result)
        {
            UpdateRequestCounter();

            TextBoxTitle.Text = result.OcrResult.OcrTexts.First().Text;
        }

        private void OnOcrCompleted(OcrServiceResult result)
        {
            UpdateRequestCounter();

/*            foreach (OcrText text in result.OcrResult.OcrTexts)
            {
                Te.Text += text.Text;
            }*/
        }
    }
}