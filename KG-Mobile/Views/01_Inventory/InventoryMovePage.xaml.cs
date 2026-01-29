using KG.Mobile.Helpers;
using KG.Mobile.Models;
using CommunityToolkit.Mvvm.Messaging;


namespace KG.Mobile.Views._01_Inventory
{

	public partial class InventoryMovePage : ContentPage
	{
		public InventoryMovePage()
		{
			InitializeComponent ();

            //Messaging Subscription - Set MoveToLocationName Entry Focus
            WeakReferenceMessenger.Default.Register<FocusRequestMessage>(
                this,
                (recipient, message) =>
                {
                    if (!Settings.AutoSelectEntryField)
                        return;

                    switch (message.Target)
                    {
                        case FocusTarget.MoveToLocationName:
                            MoveToLocationName.Focus();
                            break;
                    }
                });
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
                    ItemBarcode.Focus();
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}