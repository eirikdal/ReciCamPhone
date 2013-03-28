using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipeBaseContent : RecipeBase
    {
        public RecipePhoto RecipePhoto { get { return _RecipePhoto; } set { _RecipePhoto = value; NotifyPropertyChanged("RecipePhoto"); } }
        public String ConvertedText { get { return _ConvertedText; } set { _ConvertedText = value; NotifyPropertyChanged("ConvertedText"); } }

        private RecipeBaseContent()
        {
        }

        public static RecipeBaseContent CreateFrom(RecipePhoto photoToCrop)
        {
            return new RecipeBaseContent {_RecipePhoto = photoToCrop};
        }
    }
}
