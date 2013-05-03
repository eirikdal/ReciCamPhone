using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using SnapBook.Windows.Phone.Models;
using SnapBook.Windows.Phone.Services;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone
{
    public partial class PageAddContent : PhoneApplicationPage
    {
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;
        private readonly RecipeService _recipeService = ((App)Application.Current).RecipeService;
        private readonly PhotoChooserTask _photoChooserTask;

        public PageAddContent()
        {
            InitializeComponent();

            var themeResource = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible ? "/Assets/light/" : "/Assets/dark/";

            OcrTitleTextBox.ActionIcon = new BitmapImage(new Uri(themeResource + "appbar.image.select.png", UriKind.Relative));
            OcrDescriptionTextBox.ActionIcon = new BitmapImage(new Uri(themeResource + "appbar.image.select.png", UriKind.Relative));

            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        private void ButtonDone_Click(object sender, EventArgs e)
        {
            App.ViewModel.AddRecipeModel(_recipePhotoService.Recipe);
            App.ViewModel.SaveChangesToDB();
            NavigationService.Navigate(
                new Uri(string.Format("/PageShowRecipe.xaml?id={0}", _recipePhotoService.Recipe.RecipeId),
                        UriKind.Relative));
            _recipePhotoService.Recipe = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OcrTitleTextBox.DataContext = _recipePhotoService.Recipe.Title;
            ListBoxIngredients.ItemsSource = _recipePhotoService.Recipe.Ingredients;
            ListBoxContents.ItemsSource = _recipePhotoService.Recipe.Contents;
            OcrDescriptionTextBox.DataContext = _recipePhotoService.Recipe.Description;
            RecipeImage.DataContext = _recipePhotoService.RecipeBaseTarget;
        }

        private void ShowCamera()
        {
            _photoChooserTask.ShowCamera = true;
            _photoChooserTask.Show();
        }

        private void ButtonChoosePhotoForTitle_Click(object sender, EventArgs e)
        {
            _recipePhotoService.RecipeBaseTarget = _recipePhotoService.Recipe.Title;
            ShowCamera();
        }
		
		private void ButtonChoosePhotoForIngredients_Click(object sender, EventArgs e)
        {
            var ingredient = new RecipeIngredient();
		    _recipePhotoService.Recipe.Ingredients.Add(ingredient);
            _recipePhotoService.RecipeBaseTarget = ingredient;
            ShowCamera();
        }

		private void ButtonChoosePhotoForContent_Click(object sender, EventArgs e)
		{
		    var content = new RecipeContent();
            _recipePhotoService.Recipe.Contents.Add(content);
		    _recipePhotoService.RecipeBaseTarget = content;
            ShowCamera();
		}

		private void ButtonChoosePhotoForDescription_Click(object sender, EventArgs e)
		{
		    _recipePhotoService.RecipeBaseTarget = _recipePhotoService.Recipe.Description;
            ShowCamera();
		}

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var recipePhoto = RecipePhoto.CreateFrom(e);
                _recipePhotoService.OriginalImageSource = recipePhoto.Photo;
                _recipePhotoService.RecipeBaseTarget.RecipePhoto = recipePhoto;

                NavigationService.Navigate(new Uri("/PagePreviewImage.xaml", UriKind.Relative));
            }
        }
    }
}