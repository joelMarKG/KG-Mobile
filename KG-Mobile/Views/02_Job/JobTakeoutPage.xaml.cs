
using KG.Mobile.Helpers;
using System.Threading.Tasks;

namespace KG.Mobile.Views._02_Job
{
	public partial class JobTakeoutPage : ContentPage
	{
		public JobTakeoutPage()
		{
			InitializeComponent ();

            //Messaging Subscription - Set Barcode Entry Focus
            MessagingCenter.Subscribe<string>(this, "JobTakeoutPage-SetBarcodeFocus", (msg) =>
            {
                if (Settings.AutoSelectEntryField)
                {
                    Barcode.Focus();
                }
            });
        }

        protected async override void OnAppearing()
        {
            //if auto select is enabled, select the Barcode Entry field on load
            if (Settings.AutoSelectEntryField)
            {
                if (string.IsNullOrEmpty(Barcode.Text))
                {
                    base.OnAppearing();
                    await Task.Delay(600);
                    Barcode.Focus();
                }
            }

            
        }
    }
}