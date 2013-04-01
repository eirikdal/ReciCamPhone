using System;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using ReciCam.Windows.Phone.Models;
using ReciCam.Windows.Phone.Services;

namespace ReciCam.Windows.Phone
{
    public partial class PageAddContent : PhoneApplicationPage
    {
        private readonly ReciCamOcrService _reciCamOcrService = ((App) Application.Current).ReciCamOcrService;
        private readonly CameraCaptureTask _cameraCaptureTask;
        private readonly PhotoChooserTask _photoChooserTask;
        private readonly RecipeService _recipeService = ((App) Application.Current).RecipeService;

        public PageAddContent()
        {
            InitializeComponent();

            _cameraCaptureTask = new CameraCaptureTask();
            _cameraCaptureTask.Completed += CameraCaptureTaskOnCompleted;

            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        private void ButtonDone_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        private void CameraCaptureTaskOnCompleted(object sender, PhotoResult photoResult)
        {
            ((App)Application.Current).RecipeService.AddRecipePhoto(RecipePhoto.CreateFrom(photoResult));

            NavigationService.Navigate(new Uri("/PageAddContent.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OcrTitleTextBox.DataContext = _recipeService.RecipeBaseTitle;
            ImageChosenPhoto.DataContext = _recipeService.RecipePhotos;
            ListBoxIngredients.ItemsSource = _recipeService.RecipeBaseIngredients;
            OcrContentTextBox.DataContext = _recipeService.RecipeBaseContents;

            if (_recipeService.CanFlushCroppedPhoto())
            {
                _recipeService.FlushCroppedPhoto();

                RecipePhoto recipePhoto;
                switch (_recipeService.RecipeContentType)
                {
                    case RecipeContentType.Content:
                        recipePhoto = _recipeService.RecipeBaseTitle.RecipePhoto;
                        break;
                    case RecipeContentType.Ingredient:
                        recipePhoto = _recipeService.RecipeBaseContents.RecipePhoto;
                        break;
                    case RecipeContentType.Title:
                        recipePhoto = _recipeService.RecipeBaseIngredients.RecipePhoto;
                        break;
                }
                _reciCamOcrService.StartOcrConversion(_recipeService.RecipeBaseTitle.RecipePhoto.Photo, _recipeService.RecipeBaseTitle.OnOcrCompleted);

                OcrTitleTextBox.Visibility = Visibility.Visible;
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
            _cameraCaptureTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var recipePhoto = RecipePhoto.CreateFrom(e);
                ((App)Application.Current).RecipeService.AddRecipePhoto(recipePhoto);

                _recipeService.SetPhotoToCrop(recipePhoto);

                NavigationService.Navigate(new Uri("/PageCropImage.xaml", UriKind.Relative));
            }
        }
		
		private void ButtonChoosePhoto() {
			_photoChooserTask.ShowCamera = true;
            _photoChooserTask.Show();
		}
		
        private void ButtonChoosePhotoForTitle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _recipeService.RecipeContentType = RecipeContentType.Title;
			ButtonChoosePhoto();
        }
		
		private void ButtonChoosePhotoForContent_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _recipeService.RecipeContentType = RecipeContentType.Content;
			ButtonChoosePhoto();
        }
		
		private void ButtonChoosePhotoForIngredient_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _recipeService.RecipeContentType = RecipeContentType.Ingredient;
			ButtonChoosePhoto();
        }
    }
}