using KG.Mobile.Helpers;
using System.Windows.Input;
using KG.Mobile.Views._00_Login;

namespace KG.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = this;

        this.Navigated += OnShellNavigated;

    }

    private MobileDatabase database = MobileDatabase.Instance;

    private async void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
    {
        var from = e.Previous?.Location?.ToString() ?? "Unknown";
        var to = e.Current?.Location?.ToString() ?? "Unknown";

        await database.LogAdd(
            DateTime.Now,
            "Info",
            "Navigation",
            $"Navigated from {from} to {to}"
        );
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
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    });
}
