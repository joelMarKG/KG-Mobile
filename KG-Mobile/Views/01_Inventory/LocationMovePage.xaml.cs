
using KG.Mobile.Helpers;
using System.Threading.Tasks;

namespace KG.Mobile.Views._01_Inventory
{
	public partial class LocationMovePage : ContentPage
	{
		public LocationMovePage()
		{
			InitializeComponent ();

            //Messaging Subscription - Set LocationName Entry Focus
            MessagingCenter.Subscribe<string>(this, "LocationMovePage-SetLocationNameFocus", (msg) =>
            {
                if (Settings.AutoSelectEntryField)
                {
                    LocationName.Focus();
                }
            });

            //Messaging Subscription - Set MoveToLocationName Entry Focus
            MessagingCenter.Subscribe<string>(this, "LocationMovePage-SetMoveToLocationNameFocus", (msg) =>
            {
                if (Settings.AutoSelectEntryField)
                {
                    MoveToLocationName.Focus();
                }
            });
        }

        protected async override void OnAppearing()
        {
            //title comes from settings
            Title = Settings.LocationMoveName + " Move";

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