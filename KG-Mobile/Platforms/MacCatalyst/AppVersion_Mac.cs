using KG.Mobile.Services;
using Microsoft.Maui.ApplicationModel;

namespace KG.Mobile.Platforms.MacCatalyst
{
    public class AppVersion_Mac : IAppVersion
    {
        public string Version() => AppInfo.VersionString;
        public string Build() => AppInfo.BuildString;
    }
}
