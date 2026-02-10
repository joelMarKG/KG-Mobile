using KG.Mobile.ViewModels._99_Settings;

namespace KG.Mobile.Views._99_Settings
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage (SettingsViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
        }
	}
}