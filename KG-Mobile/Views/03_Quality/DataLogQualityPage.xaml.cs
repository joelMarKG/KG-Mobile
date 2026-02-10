using KG.Mobile.Helpers;
using System.Threading.Tasks;
using KG.Mobile.ViewModels._03_Quality;

namespace KG.Mobile.Views._03_Quality
{
	public partial class DataLogQualityPage : ContentPage
	{
		public DataLogQualityPage(DataLogQualityViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;

            viewModel.RequestBarcodeFocus += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Barcode.Focus();
                });
            };
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