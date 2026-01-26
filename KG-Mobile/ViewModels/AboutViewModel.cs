using KG.Mobile.Services;
using System;
using System.Windows.Input;

namespace KG.Mobile.ViewModels
{
    public class AboutViewModel : ContentView
    {
        #region XAML Bound Tags

        //App Version
        public String Version
        {
            get { return "Version: " + DependencyService.Get<IAppVersion>().Version(); }
        }

        //App Version
        public String BuildNumber
        {
            get { return "Build Number: " + DependencyService.Get<IAppVersion>().Build(); }
        }

        #endregion
    }


}