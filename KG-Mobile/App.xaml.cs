using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Services;
using KG.Mobile.Views;

namespace KG.Mobile
{
    public partial class App : Application
    {


        //database definition
        static MobileDatabase database;

        BusyPopup? _busyPopup;

        public App()
        {
            //Startup, go to MainPage, which then goes to LoginPage
            InitializeComponent();

            //construct database
            database = MobileDatabase.Instance;

            //Launch the Login Page, after login go to MainPage
            MainPage = new AppShell();

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
                _soundHelper.PlayError();

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

            ////Messaging Subscription - Login View Model: Login Success
            //MessagingCenter.Subscribe<AuthToken>(this, "LoginSuccess", async (authToken) =>
            //{
            //    //check for successful login
            //    if (authToken != null && authToken.StatusCode == "OK")
            //    {
            //        //Go To Main Page
            //        MainPage = new MainPage();

            //        //Save to Database
            //        await database.LogAdd(DateTime.Now, "Info", "Security", "Login Success");
            //    }
            //    else
            //    {
            //        //Play the Error sound
            //        _soundHelper.PlayError();

            //        //Notify User with Popup
            //        await App.Current.MainPage.DisplayAlert("Login Failed", "Login Failure, Check Username/Password", "Ok");

            //        //Save to Database
            //        await database.LogAdd(DateTime.Now, "Error", "Security", "Login Failure, Check Username/Password");
            //    }

            //});

            ////Messaging Subscription - Login View Model: Already Logged In
            //MessagingCenter.Subscribe<AuthToken>(this, "AlreadyLoggedIn", async (authToken) =>
            //{
            //    //Go To Main Page
            //    MainPage = new MainPage();

            //    //Save to Database
            //    await database.LogAdd(DateTime.Now, "Info", "Security", "Login Resumed");

            //});

            ////Messaging Subscription - MainPage.xaml.cs: Log Out
            //MessagingCenter.Subscribe<AuthToken>(this, "LogOut", async (authToken) =>
            //{
            //    //Go To Main Page
            //    MainPage = new LoginPage();

            //    //Save to Database
            //    await database.LogAdd(DateTime.Now, "Info", "Security", "Log Out");

            //});

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


            ////Messaging Subscription - General Popup Message with no Database Logging
            //MessagingCenter.Subscribe<PopupMessage>(this, "PopupMessageNoLog", async (msg) =>
            //{
            //    //Notify User with Popup
            //    await App.Current.MainPage.DisplayAlert(msg.title, msg.message, msg.buttonText);

            //});

            ////Messaging Subscription - LogUnhandledException (From Main Activity)
            //MessagingCenter.Subscribe<PopupMessage>(this, "LogUnhandledException", async (msg) =>
            //{
            //    //Save to Database
            //    await database.LogAdd(DateTime.Now, "Unhandled Exception", msg.component, msg.title + ": " + msg.message);

            //});

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