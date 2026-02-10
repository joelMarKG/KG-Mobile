using KG.Mobile.Helpers;
using KG.Mobile.ViewModels._01_Inventory;

namespace KG.Mobile.Views._01_Inventory
{
	public partial class InventoryShipPage : ContentPage
	{
		public InventoryShipPage(InventoryShipViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;

            viewModel.RequestLocationFocus += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LocationName.Focus();
                });
            };
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