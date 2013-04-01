using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;

namespace ReciCam.Windows.Phone.Models
{
    public abstract class RecipeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected RecipePhoto _RecipePhoto { get; set; }
        protected OcrServiceResult _OcrServiceResult { get; set; }
        protected String _ConvertedText { get; set; }

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void OnOcrCompleted(OcrServiceResult ocrServiceResult);
    }
}
