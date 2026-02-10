using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.Helpers;
using KG.Mobile.Models;
using KG.Mobile.ViewModels._01_Inventory;
using System.Threading.Tasks;

namespace KG.Mobile.Views._01_Inventory
{
	public partial class LocationMovePage : ContentPage
	{
		public LocationMovePage(LocationMoveViewModel viewModel)
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

            viewModel.RequestMoveToLocationFocus += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MoveToLocationName.Focus();
                });
            };
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
                if (string.IsNullOrEmpty(LocationName.Text))
                {
                    LocationName.Focus();
                }
                else
                {
                    MoveToLocationName.Focus();
                }
            }
        }
    }
}