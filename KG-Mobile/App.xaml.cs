using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Services;
using KG.Mobile.Views;
using KG.Mobile.Views._00_Login;

namespace KG.Mobile
{
    public partial class App : Application
    {
        //sound helper
        private readonly SoundHelper _soundHelper;
        //database definition
        static MobileDatabase database;

        BusyPopup? _busyPopup;

        public App(SoundHelper soundHelper)
        {
            //Startup, go to MainPage, which then goes to LoginPage
            InitializeComponent();

            //construct database
            database = MobileDatabase.Instance;

            _soundHelper = soundHelper;

            //Launch the Login Page, after login go to MainPage
            MainPage = new NavigationPage(new LoginPage());

            //Messaging Subscription - General Popup Message
            WeakReferenceMessenger.Default.Register<PopupMessageRequest>(this, async (r, msg) =>
            {
                var popup = msg.Popup;

                await App.Current.MainPage.DisplayAlert(
                    popup.title,
                    popup.message,
                    popup.buttonText
                );

                await database.LogAdd(
                    DateTime.Now,
                    "Info",
                    popup.component,
                    $"{popup.title}: {popup.message}"
                );
            });

            //Messaging Subscription - Error Popup Message
            WeakReferenceMessenger.Default.Register<PopupErrorMessage>(this, async (r, msg) =>
            {
                _soundHelper.PlayErrorAsync();

                var popup = msg.Popup;

                await App.Current.MainPage.DisplayAlert(
                    popup.title,
                    popup.message,
                    popup.buttonText
                );

                await database.LogAdd(
                    DateTime.Now,
                    "Error",
                    popup.component,
                    $"{popup.title}: {popup.message}"
                );
            });

            //Messaging Subscription - Show the Busy Popup
            WeakReferenceMessenger.Default.Register<BusyMessage>(this, async (r, msg) =>
            {
                if (msg.visible)
                {
                    if (_busyPopup == null)
                        _busyPopup = new BusyPopup(msg.message);

                    await MainPage.ShowPopupAsync(_busyPopup);
                }
                else
                {
                    _busyPopup?.Hide();
                    _busyPopup = null;
                }
            });

            //Messaging Subscription - No Visible Background for Loging.
            WeakReferenceMessenger.Default.Register<LogMessageRequest>(this, async (r, msg) =>
            {
                var log = msg.Log;

                //Write to logs
                await database.LogAdd(
                    DateTime.Now,
                    "Error",
                    log.type,
                    $"{log.component}: {log.comment}"
                );
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}