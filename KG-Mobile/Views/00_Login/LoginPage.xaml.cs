using KG.Mobile.ViewModels._00_Login;

namespace KG.Mobile.Views._00_Login
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel Vm => (LoginViewModel)BindingContext;
        public LoginPage()
        {
            InitializeComponent();

            Vm.LoginSucceeded += OnLoginSucceeded;
        }
        private void OnLoginSucceeded()
        {
            // Switch MainPage to AppShell (with flyout)
            Application.Current.MainPage = new AppShell();

            Shell.Current.GoToAsync("//Home");

        }
    }
}