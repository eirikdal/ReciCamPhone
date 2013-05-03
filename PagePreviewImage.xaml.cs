using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using SnapBook.Windows.Phone.Models;
using SnapBook.Windows.Phone.Services;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone
{
    public partial class OcrCropImage : PhoneApplicationPage
    {
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;
        private readonly RecipeOcrService _recipeOcrService = ((App)Application.Current).RecipeOcrService;

        private ApplicationBarIconButton btnReject;
        private ApplicationBarIconButton btnCrop;
        private ApplicationBarIconButton btnAccept;
        private ApplicationBarIconButton btnHelp;

        public OcrCropImage()
        {
            InitializeComponent();

            btnCrop = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            btnAccept = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            btnReject = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            btnHelp = ApplicationBar.Buttons[2] as ApplicationBarIconButton;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TextBlockContents.DataContext = _recipePhotoService.RecipeBaseTarget;
            //TextBlockOcr.DataContext = _recipePhotoService.RecipeBaseTarget;
            DisplayedImageElement.Source = _recipePhotoService.RecipeBaseTarget.RecipePhoto.Photo;
        }

        void btnAccept_Click(object sender, EventArgs e)
        {
            _recipePhotoService.RecipeBaseTarget.Text = "";
            ProgressBar.Visibility = Visibility.Visible;
            btnAccept.IsEnabled = false;

            _recipeOcrService.StartOcrConversion(_recipePhotoService.RecipeBaseTarget, OnOcrRecipeBaseComplete, (int)((_recipePhotoService.RecipeBaseTarget.RecipePhoto.Scale / 100f) * 600f));
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PageEditPhoto.xaml", UriKind.Relative));
        }


        private void OnOcrRecipeBaseComplete(OcrServiceResult result)
        {
            var recipeBase = result.StateObject as RecipeBase;
            recipeBase.OcrServiceResult = result;
            result.OcrResult.OcrTexts.ForEach(ocrText => recipeBase.Text += recipeBase.FormatOcrText(ocrText));

            ProgressBar.Visibility = Visibility.Collapsed;
            btnAccept.IsEnabled = true;

            NavigationService.Navigate(new Uri("/PageAddContent.xaml", UriKind.Relative));
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}