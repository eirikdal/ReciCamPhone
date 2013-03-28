using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReciCam.Windows.Phone.Models
{
    public class RecipeBaseTitle : RecipeBase
    {
        public RecipePhoto RecipePhoto { get { return _RecipePhoto; } set { _RecipePhoto = value; NotifyPropertyChanged("RecipePhoto"); } }
        public String ConvertedText { get { return _ConvertedText;  } set { _ConvertedText = value; NotifyPropertyChanged("ConvertedText"); } }
    }
}
