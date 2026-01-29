
using KG.Mobile.Helpers;

namespace KG.Mobile.Views._01_Inventory
{
	public partial class InventoryShipPage : ContentPage
	{
		public InventoryShipPage()
		{
			InitializeComponent ();

            ////Messaging Subscription - Set LocationName Entry Focus
            //MessagingCenter.Subscribe<string>(this, "InventoryShipPage-SetMoveToLocationNameFocus", (msg) =>
            //{
            //    if (Settings.AutoSelectEntryField)
            //    {
            //        LocationName.Focus();
            //    }
            //});
        }

        protected async override void OnAppearing()
        {
            //if auto select is enabled, select the Location name Entry field on load
            if (Settings.AutoSelectEntryField)
            {
                base.OnAppearing();
                await Task.Delay(600);
                LocationName.Focus();
            }
        }
    }
}