using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;
using SnapBook.Windows.Phone.ViewModel;

namespace SnapBook.Windows.Phone.Models
{
    [Table]
    public abstract class RecipeBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //protected readonly RecipeService RecipeService = ((App)Application.Current).RecipeService;
        //protected readonly RecipePhotoService RecipePhotoService = ((App)Application.Current).RecipePhotoService;

        private RecipePhoto _recipePhoto;
        private OcrServiceResult _ocrServiceResult;
        protected String _text;

        public abstract String Text { get; set; }

        public OcrServiceResult OcrServiceResult { get { return _ocrServiceResult; } set { _ocrServiceResult = value; NotifyPropertyChanged("OcrServiceResult"); } }
        public RecipePhoto RecipePhoto { get { return _recipePhoto; } set { _recipePhoto = value; NotifyPropertyChanged("RecipePhoto"); } }

        public String FormatOcrText(OcrText text)
        {
            var groupWordsByBox = text.Words.GroupBy(word => word.Box.Split(',')[1]);
            var textLines = groupWordsByBox.Select(word => word.Aggregate("", (acc, ocrWord) => acc += (" " + ocrWord.Text)));
            
            return string.Join("\n", textLines);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
