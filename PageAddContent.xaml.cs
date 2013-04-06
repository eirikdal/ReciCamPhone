using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
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
        private readonly PhotoChooserTask _photoChooserTask;
        private readonly RecipePhotoService _recipePhotoService = ((App)Application.Current).RecipePhotoService;
        private readonly RecipeService _recipeService = ((App)Application.Current).RecipeService;

        public PageAddContent()
        {
            InitializeComponent();

            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        private void ButtonDone_Click(object sender, EventArgs e)
        {
            Recipe recipe = RecipeService.CreateRecipe(_recipePhotoService.RecipeBaseTitle,
                            _recipePhotoService.RecipeBaseIngredients, _recipePhotoService.RecipeBaseContents);
            _recipeService.Recipes.Add(recipe);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OcrTitleTextBox.DataContext = _recipePhotoService.RecipeBaseTitle;
            ListBoxIngredients.ItemsSource = _recipePhotoService.RecipeBaseIngredients;
            ListBoxDescription.ItemsSource = _recipePhotoService.RecipeBaseContents;
            RecipeImage.DataContext = _recipePhotoService.RecipeBaseTarget;
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var recipePhoto = RecipePhoto.CreateFrom(e);
                
                _recipePhotoService.RecipeBaseTarget.RecipePhoto = recipePhoto;

                NavigationService.Navigate(new Uri("/PageCropImage.xaml", UriKind.Relative));
            }
        }
		
		private void ButtonChoosePhoto() {
			_photoChooserTask.ShowCamera = true;
            _photoChooserTask.Show();
		}
		
        private void ButtonChoosePhotoForTitle_Click(object sender, EventArgs e)
        {
            _recipePhotoService.RecipeBaseTarget = _recipePhotoService.RecipeBaseTitle;
			ButtonChoosePhoto();
        }
		
		private void ButtonChoosePhotoForIngredients_Click(object sender, EventArgs e)
        {
            var recipeBaseTarget = new RecipeBase();
		    _recipePhotoService.RecipeBaseIngredients.Add(recipeBaseTarget);
            _recipePhotoService.RecipeBaseTarget = recipeBaseTarget;
			ButtonChoosePhoto();
        }

		private void ButtonChoosePhotoForDescription_Click(object sender, EventArgs e)
		{
		    var recipeBaseContents = new RecipeBase();
            _recipePhotoService.RecipeBaseContents.Add(recipeBaseContents);
		    _recipePhotoService.RecipeBaseTarget = recipeBaseContents;
			ButtonChoosePhoto();
		}

		private void ButtonChoosePhotoForContent_Click(object sender, EventArgs e)
		{
		    _recipePhotoService.RecipeBaseTarget = _recipePhotoService.RecipeBaseDescription;
		    ButtonChoosePhoto();
		}
    }
}