using CommunityToolkit.Maui.Core.Platform;
using KG.Mobile.Helpers;

namespace KG.Mobile.Behaviors
{
    public class DisableKeyboardOnFocusBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.Focused += OnFocused;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.Focused -= OnFocused;
            base.OnDetachingFrom(entry);
        }

        private async void OnFocused(object? sender, FocusEventArgs e)
        {
            if (!Settings.DisablePopupKeyboard)
                return;

            if (sender is not Entry entry)
                return;

            // 🔑 Let Android finish focus + IME request
            await Task.Delay(50);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await entry.HideKeyboardAsync(CancellationToken.None);
            });
        }
    }
}
