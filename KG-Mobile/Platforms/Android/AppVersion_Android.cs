using KG.Mobile.Services;

[assembly: Microsoft.Maui.Controls.Dependency(typeof(KG.Mobile.Platforms.Android.AppVersion_Android))]
namespace KG.Mobile.Platforms.Android
{
    public class AppVersion_Android : IAppVersion
    {
        public string Version() => AppInfo.VersionString;
        public string Build() => AppInfo.BuildString;
    }
}
