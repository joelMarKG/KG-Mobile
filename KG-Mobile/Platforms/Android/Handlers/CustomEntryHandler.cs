using Android.Text;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;

namespace KG.Mobile.Platforms.Android.Handlers;

public class CustomEntryHandler : EntryHandler
{
    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        platformView.ShowSoftInputOnFocus = false;
        platformView.SetRawInputType(InputTypes.Null);
    }
}
