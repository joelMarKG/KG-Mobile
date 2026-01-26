using KG.Mobile.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KG.Mobile.CustomControls
{
    //implemented in CustomEntryRenderer class in the Android Version
    //iOS/UWP version lack an implemntation
    public class CustomEntry : Entry
    {
        //get set the setting from the settings helper
        public bool DisablePopupkeyboard { get { return Settings.DisablePopupKeyboard; } set { Settings.DisablePopupKeyboard = value; } }
    }
}
