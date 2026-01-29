using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.Services;
using KG.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KG.Mobile.ViewModels._00_Login
{
    
    public class LoginViewModel : ContentView
    {
        
        private GraphQLApiServices _graphQLApiServices = new GraphQLApiServices();
        private GraphQLApiServicesHelper _graphQLApiServicesHelper = new GraphQLApiServicesHelper();
        public string GraphQLApiSecurityUrl { get { return Settings.GraphQLApiSecurityUrl; } set { Settings.GraphQLApiSecurityUrl = value.Replace(" ", ""); } } //remove spaces
        public string Username { get { return Settings.Username; } set { Settings.Username = value.Replace(" ",""); } } //remove spaces
        public string Password { get { return Settings.Password; } set { Settings.Password = value; }  }

        public LoginViewModel()
        {
            _graphQLApiServicesHelper.LoggedInCheck(Username);
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
                    WeakReferenceMessenger.Default.Send(msg);

                    await _graphQLApiServices.LoginAsync(Username, Password);

                    //Hide Busy
                    msg.visible = false;
                    WeakReferenceMessenger.Default.Send(msg);
                });
            }

        }
    }
}