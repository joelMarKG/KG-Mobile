using System;
using Microsoft.Maui.Devices.Sensors;

namespace KG.Mobile.Helpers
{
    // Vibrate Helper Class
    public static class Vibrate
    {
        /// <summary>
        /// Vibrates the device for the specified duration (0-5000 ms).
        /// </summary>
        public static void VibrationDuration(int milliseconds)
        {
            try
            {
                // Ensure the duration is valid
                if (milliseconds < 0) milliseconds = 0;
                if (milliseconds > 5000) milliseconds = 5000;

                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(milliseconds));
            }
            catch (FeatureNotSupportedException)
            {
                // Feature not supported on device
            }
            catch (Exception)
            {
                // Other error has occurred.
            }
        }
    }
}
