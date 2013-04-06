using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using ReciCam.Windows.Phone.Services;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipeBase : INotifyPropertyChanged
    {
        //protected readonly RecipeService RecipeService = ((App)Application.Current).RecipeService;
        //protected readonly RecipePhotoService RecipePhotoService = ((App)Application.Current).RecipePhotoService;

        public event PropertyChangedEventHandler PropertyChanged;
        private RecipePhoto _recipePhoto;
        private OcrServiceResult _ocrServiceResult;
        private String _text;

        public OcrServiceResult OcrServiceResult { get { return _ocrServiceResult; } set { _ocrServiceResult = value; NotifyPropertyChanged("OcrServiceResult"); } }
        public RecipePhoto RecipePhoto { get { return _recipePhoto; } set { _recipePhoto = value; NotifyPropertyChanged("RecipePhoto"); } }
        public String Text { get { return _text; } set { _text = value; NotifyPropertyChanged("Text"); } }

        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public String FormatOcrText(OcrText text)
        {
            var groupWordsByBox = text.Words.GroupBy(word => word.Box.Split(',')[1]);
            var textLines = groupWordsByBox.Select(word => word.Aggregate("", (acc, ocrWord) => acc += (" " + ocrWord.Text)));

            return string.Join("\n", textLines);
        } 

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
