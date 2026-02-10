using KG.Mobile.Services;

[assembly: Microsoft.Maui.Controls.Dependency(typeof(KG.Mobile.Platforms.iOS.AppVersion_iOS))]
namespace KG.Mobile.Platforms.iOS
{
    public class AppVersion_iOS : IAppVersion
    {
        public string Version() => AppInfo.VersionString;
        public string Build() => AppInfo.BuildString;
    }
}
