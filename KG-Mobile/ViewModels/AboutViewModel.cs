using KG.Mobile.Services;

namespace KG.Mobile.ViewModels
{
    public class AboutViewModel
    {
        private readonly IAppVersion _appVersion;

        public AboutViewModel(IAppVersion appVersion)
        {
            _appVersion = appVersion;
        }

        public string Version => "Version: " + _appVersion.Version();
        public string BuildNumber => "Build Number: " + _appVersion.Build();
    }
}