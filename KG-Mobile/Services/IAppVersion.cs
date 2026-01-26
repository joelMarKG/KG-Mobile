using System;
using System.Collections.Generic;
using System.Text;

namespace KG.Mobile.Services
{
    //interface to be written from from android/ios implementations for the about page
    public interface IAppVersion
    {
        string Version();
        int Build();
    }
}
