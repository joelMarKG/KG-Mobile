
using KG.Mobile.Helpers;
using System.Threading.Tasks;

namespace KG.Mobile.Views._03_Quality
{
	public partial class DataLogQualityPage : ContentPage
	{
		public DataLogQualityPage()
		{
			InitializeComponent ();

            //Messaging Subscription - Set Barcode Entry Focus
            MessagingCenter.Subscribe<string>(this, "DataLogQualityPage-SetBarcodeFocus", (msg) =>
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