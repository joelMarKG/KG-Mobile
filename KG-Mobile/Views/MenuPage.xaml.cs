using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using System;
using System.Collections.Generic;


namespace KG.Mobile.Views
{
    public partial class MenuPage : ContentPage
    {
        List<HomeMenuItem> menuItems;
        private MobileDatabase database = MobileDatabase.Instance;

        public MenuPage()
        {
            InitializeComponent();

            //Build Menu
            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.InventoryMove, Title ="Inventory Move" },
                new HomeMenuItem {Id = MenuItemType.LocationMove, Title = Settings.LocationMoveName + " Move"},
                new HomeMenuItem {Id = MenuItemType.JobTakeout, Title = "Job Takeout"},
                new HomeMenuItem {Id = MenuItemType.Quality, Title = "Defect"},
                new HomeMenuItem {Id = MenuItemType.InventoryShip, Title ="Inventory Ship" },
                new HomeMenuItem {Id = MenuItemType.Settings, Title="Settings" },
                new HomeMenuItem {Id = MenuItemType.Log, Title="Log" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About" },
                new HomeMenuItem {Id = MenuItemType.LogOut, Title="Logout" }
            };
            ListViewMenu.ItemsSource = menuItems;
            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var selected = (HomeMenuItem)e.SelectedItem;

                // Shell Navigation
                switch (selected.Id)
                {
                    case MenuItemType.InventoryMove:
                        await Shell.Current.GoToAsync("//InventoryMovePage");
                        break;

                    case MenuItemType.LocationMove:
                        await Shell.Current.GoToAsync("//LocationMovePage");
                        break;

                    case MenuItemType.JobTakeout:
                        await Shell.Current.GoToAsync("//JobTakeoutPage");
                        break;

                    case MenuItemType.Quality:
                        await Shell.Current.GoToAsync("//DataLogQualityPage");
                        break;

                    case MenuItemType.InventoryShip:
                        await Shell.Current.GoToAsync("//InventoryShipPage");
                        break;

                    case MenuItemType.Settings:
                        await Shell.Current.GoToAsync("//SettingsPage");
                        break;

                    case MenuItemType.Log:
                        await Shell.Current.GoToAsync("//SessionLogPage");
                        break;

                    case MenuItemType.About:
                        await Shell.Current.GoToAsync("//AboutPage");
                        break;

                    case MenuItemType.LogOut:
                        // Reset access token and trigger logout
                        Settings.AccessToken = "";
                        AuthToken token = new AuthToken();
                        WeakReferenceMessenger.Default.Send(token, "LogOut");
                        break;
                }

                // Save to Database
                await database.LogAdd(DateTime.Now, "Info", "Navigation", "Navigated to: " + selected.Title);

                // Deselect the item to avoid it staying highlighted
                ListViewMenu.SelectedItem = null;
            };
        }
    }
}