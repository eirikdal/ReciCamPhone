using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Hawaii.Ocr.Client.Model;
using Microsoft.Hawaii.Ocr.Client.ServiceResults;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipeBaseContent : RecipeBase
    {
        public RecipePhoto RecipePhoto { get { return _RecipePhoto; } set { _RecipePhoto = value; NotifyPropertyChanged("RecipePhoto"); } }
        public OcrServiceResult OcrServiceResult { get { return _OcrServiceResult; } set { _OcrServiceResult = value; NotifyPropertyChanged("ConvertedText"); } }
        public String ConvertedText { get { return _ConvertedText; } set { _ConvertedText = value; NotifyPropertyChanged("ConvertedText"); } }

        private RecipeBaseContent()
        {
        }

        public static RecipeBaseContent CreateFrom(RecipePhoto photoToCrop)
        {
            return new RecipeBaseContent {_RecipePhoto = photoToCrop};
        }

        public override void OnOcrCompleted(OcrServiceResult result)
        {
            OcrServiceResult = result;
            ConvertedText = result.OcrResult.OcrTexts.Aggregate("", (accText, ocrText) => accText += ocrText.Text);
        }
    }
}
