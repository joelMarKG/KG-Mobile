using CommunityToolkit.Maui.Views;

namespace KG.Mobile.Views
{
    public partial class BusyPopup : Popup
    {
        public BusyPopup(string message)
        {
            InitializeComponent();
            BusyLabel.Text = message;
        }
        public void Hide()
        {
            CloseAsync(); // <-- VALID here
        }
    }
}