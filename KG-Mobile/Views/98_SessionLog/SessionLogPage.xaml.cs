using KG.Mobile.ViewModels._98_SessionLog;

namespace KG.Mobile.Views._98_SessionLog
{
	public partial class SessionLogPage : ContentPage
	{
		public SessionLogPage()
		{
			InitializeComponent ();                       
        }

        //refresh on page reload
        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitializeComponent();
        }
    }
}