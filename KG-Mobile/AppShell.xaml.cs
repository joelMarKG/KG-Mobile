using KG.Mobile.Helpers;
using System.Windows.Input;

namespace KG.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public ICommand NavigateCommand => new Command<string>(async (page) =>
    {
        // Navigate using route name
        if (!string.IsNullOrEmpty(page))
        {
            await Shell.Current.GoToAsync(page);
        }
    });

    public ICommand LogoutCommand => new Command(() =>
    {
        Settings.AccessToken = "";
        //MessagingCenter.Send(new AuthToken(), "LogOut");
    });
}
