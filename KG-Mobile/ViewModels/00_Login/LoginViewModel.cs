using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using KG.Mobile.Services;
using KG.Mobile.Helpers;
using KG.Mobile.Views;
using KG.Mobile.Models;

namespace KG.Mobile.ViewModels._00_Login
{
    
    public class LoginViewModel : ContentView
    {
        
        private GraphQLAPIServices _graphQLApiServices = new GraphQLAPIServices();
        public string GraphQLApiSecurityUrl { get { return Settings.GraphQLApiSecurityUrl; } set { Settings.GraphQLApiSecurityUrl = value.Replace(" ", ""); } } //remove spaces
        public string Username { get { return Settings.Username; } set { Settings.Username = value.Replace(" ",""); } } //remove spaces
        public string Password { get { return Settings.Password; } set { Settings.Password = value; }  }

        public LoginViewModel()
        {
            _graphQLApiServices.LoggedInCheckAsync();
        }

        //Login to WebAPI Command
        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    //Show Busy
                    BusyMessage msg = new BusyMessage(true, "Logging In");
                    MessagingCenter.Send(msg, "BusyPopup");

                    await _graphQLApiServices.LoginAsync(Username, Password);

                    //Hide Busy
                    msg.visible = false;
                    MessagingCenter.Send(msg, "BusyPopup");
                });
            }

        }
    }
}