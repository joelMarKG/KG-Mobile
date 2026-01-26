using System;
using System.Collections.Generic;
using System.Text;

namespace KG.Mobile.Models
{
    public enum MenuItemType
    {
        InventoryMove,
        InventoryShip,
        LocationMove,
        JobTakeout,
        Quality,
        Settings,
        Log,
        About,
        LogOut
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
