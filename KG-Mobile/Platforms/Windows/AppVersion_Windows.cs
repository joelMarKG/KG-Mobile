using KG.Mobile.Services;
using Microsoft.Maui.ApplicationModel;

namespace KG.Mobile.Platforms.Windows
{
    public class AppVersion_Windows : IAppVersion
    {
        public string Version() => AppInfo.VersionString;
        public string Build() => AppInfo.BuildString;
    }
}
