using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.ViewModels._01_Inventory;


namespace KG.Mobile.Views._01_Inventory
{

	public partial class InventoryMovePage : ContentPage
	{
		public InventoryMovePage(InventoryMoveViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;

            ////Messaging Subscription - Set MoveToLocationName Entry Focus
            //WeakReferenceMessenger.Default.Register<FocusRequestMessage>(
            //    this,
            //    (recipient, message) =>
            //    {
            //        if (!Settings.AutoSelectEntryField)
            //            return;

            //        switch (message.Target)
            //        {
            //            case FocusTarget.MoveToLocationName:
            //                MoveToLocationName.Focus();
            //                break;
            //        }
            //    });

            viewModel.RequestLotBarcodeFocus += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LotBarcodeEntry.Focus();
                });
            };
            viewModel.RequestLocationFocus += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MoveToLocationName.Focus();
                });
            };
        }

        protected async override void OnAppearing()
        {
            //if auto select is enabled, select the fields on load
            if (Settings.AutoSelectEntryField)
            {
                base.OnAppearing();
                await Task.Delay(600);

                if (string.IsNullOrEmpty(MoveToLocationName.Text))
                {
                    MoveToLocationName.Focus();
                }
                else
                {
                    LotBarcodeEntry.Focus();
                }
            }
        }

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    WeakReferenceMessenger.Default.UnregisterAll(this);
        //}
    }
}