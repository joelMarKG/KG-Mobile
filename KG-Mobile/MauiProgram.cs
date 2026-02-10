using CommunityToolkit.Maui;

#if ANDROID
using KG.Mobile.CustomControls;
using KG.Mobile.Platforms.Android.Handlers;
#endif
using KG.Mobile.Services;
using KG.Mobile.ViewModels;
using KG.Mobile.ViewModels._00_Login;
using KG.Mobile.ViewModels._01_Inventory;
using KG.Mobile.ViewModels._03_Quality;
using KG.Mobile.ViewModels._98_SessionLog;
using KG.Mobile.ViewModels._99_Settings;
using KG.Mobile.Views;
using KG.Mobile.Views._00_Login;
using KG.Mobile.Views._01_Inventory;
using KG.Mobile.Views._03_Quality;
using KG.Mobile.Views._98_SessionLog;
using KG.Mobile.Views._99_Settings;
using Plugin.Maui.Audio;

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
            builder.Services.AddTransient<GraphQLApiServicesHelper>();


            // Register IAppVersion implementation per platform
#if ANDROID
            builder.Services.AddSingleton<IAppVersion, KG.Mobile.Platforms.Android.AppVersion_Android>();
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<CustomEntry, CustomEntryHandler>();
            });
#elif IOS
                    builder.Services.AddSingleton<IAppVersion, KG.Mobile.Platforms.iOS.AppVersion_iOS>();
#elif MACCATALYST
            builder.Services.AddSingleton<IAppVersion, KG.Mobile.Platforms.MacCatalyst.AppVersion_Mac>();
#elif WINDOWS
                    builder.Services.AddSingleton<IAppVersion, KG.Mobile.Platforms.Windows.AppVersion_Windows>();
#endif

            // Register your view models
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<InventoryMoveViewModel>();
            builder.Services.AddTransient<InventoryShipViewModel>();
            builder.Services.AddTransient<LocationMoveViewModel>();
            builder.Services.AddTransient<DataLogQualityViewModel>();
            builder.Services.AddTransient<SessionLogViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<AboutViewModel>();

            // Register pages that need constructor injection
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<InventoryMovePage>();
            builder.Services.AddTransient<InventoryShipPage>();
            builder.Services.AddTransient<LocationMovePage>();
            builder.Services.AddTransient<DataLogQualityPage>();
            builder.Services.AddTransient<SessionLogPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<AboutPage>();

            return builder.Build();
        }
    }
}
