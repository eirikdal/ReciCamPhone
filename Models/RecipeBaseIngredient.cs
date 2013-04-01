using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipeBaseIngredient : RecipeBase
    {
        public RecipePhoto RecipePhoto { get { return _RecipePhoto; } set { _RecipePhoto = value; NotifyPropertyChanged("RecipePhoto"); } }
        public OcrServiceResult OcrServiceResult { get { return _OcrServiceResult; } set { _OcrServiceResult = value; NotifyPropertyChanged("OcrResult"); } }
        public ObservableCollection<String> Ingredients { get; set; }
        public String ConvertedText { get { return _ConvertedText; } set { _ConvertedText = value; NotifyPropertyChanged("ConvertedText"); } }

        public override void OnOcrCompleted(OcrServiceResult result)
        {
            OcrServiceResult = result;
            ConvertedText = result.OcrResult.OcrTexts.Aggregate("", (accText, ocrText) => accText += ocrText.Text);
            result.OcrResult.OcrTexts.ForEach(ocrText => Ingredients.Add(ocrText.Text));
        }

        public static RecipeBaseIngredient CreateFrom(RecipePhoto photoToCrop)
        {
            return new RecipeBaseIngredient { _RecipePhoto = photoToCrop };
        }

    }
}
