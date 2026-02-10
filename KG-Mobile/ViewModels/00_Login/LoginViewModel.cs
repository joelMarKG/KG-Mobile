using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Services;
using System.Windows.Input;

namespace KG.Mobile.ViewModels._00_Login
{
    
    public class LoginViewModel : ObservableObject
    {
        
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        public string GraphQLApiSecurityUrl { get { return Settings.GraphQLApiSecurityUrl; } set { Settings.GraphQLApiSecurityUrl = value.Replace(" ", ""); } } //remove spaces
        public string Username { get { return Settings.Username; } set { Settings.Username = value.Replace(" ",""); } } //remove spaces
        public string Password { get { return Settings.Password; } set { Settings.Password = value; }  }

        public event Action? LoginSucceeded;

        //Login to WebAPI Command
        public ICommand LoginCommand => new Command(async () =>
        {
            try
            {
                // Show Busy
                var msg = new BusyMessage(true, "Logging In");
                WeakReferenceMessenger.Default.Send(msg);

                var (success, token) = await _graphQLApiServices.LoginAsync(Username, Password);

                // Hide Busy
                WeakReferenceMessenger.Default.Send(new BusyMessage(false, ""));

                if (success  && token != "")
                {
                    WeakReferenceMessenger.Default.Send(
                        new LogMessageRequest(
                            new LogMessage(
                                "Info",
                                "Security",
                                "Login Success for user: " + Username
                            )
                        )
                    );

                    // Raise login success event
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    // Show PopUp Error
                    var errormsg = new PopupErrorMessage(
                        new PopupMessage(
                            "Login Failed",
                            "Security",
                            "Login failed. Please try again",
                            "ok"
                        )
                    );
                    WeakReferenceMessenger.Default.Send(errormsg);
                }
            }
            catch (Exception ex)
            {
                // Hide Busy if exception
                var msg = new BusyMessage(false, "");
                WeakReferenceMessenger.Default.Send(msg);

                // Show generic error popup
                var errormsg = new PopupErrorMessage(
                    new PopupMessage(
                        "Login Error",
                        "Exception",
                        ex.Message,
                        "ok"
                    )
                );
                WeakReferenceMessenger.Default.Send(errormsg);
            }
        });

    }
}