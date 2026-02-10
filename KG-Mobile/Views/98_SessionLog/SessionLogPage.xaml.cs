using CommunityToolkit.Mvvm.Messaging;
using KG.Mobile.ViewModels._98_SessionLog;
using static KG.Mobile.Helpers.MobileDatabase;
using KG.Mobile.Models;

namespace KG.Mobile.Views._98_SessionLog
{
    public partial class SessionLogPage : ContentPage
    {
        public SessionLogPage()
        {
            InitializeComponent();

            // Set the BindingContext to the ViewModel
            BindingContext = new SessionLogViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load logs asynchronously
            if (BindingContext is SessionLogViewModel vm)
                await vm.LoadLog();
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is not Log log)
                return;

            WeakReferenceMessenger.Default.Send(
                new PopupMessageRequest(
                    new PopupMessage(
                        "Log Record",
                        "Record",
                        $"Date/Time:\n{log.dateTime}\n\n" +
                        $"Type: {log.type}\n" +
                        $"Component: {log.component}\n\n" +
                        $"{log.comment}",
                        "Ok")));

            ((CollectionView)sender).SelectedItem = null;
        }

    }
}
