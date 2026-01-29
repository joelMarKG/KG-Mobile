using CommunityToolkit.Maui;
using Plugin.Maui.Audio;
using KG.Mobile.Services;

namespace KG.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()              
                .UseMauiCommunityToolkit()      
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<SoundHelper>();

            return builder.Build();
        }
    }
}
