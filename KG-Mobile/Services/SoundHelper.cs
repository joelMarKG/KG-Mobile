using KG.Mobile.Helpers;
using Plugin.SimpleAudioPlayer;
using System;
using System.IO;
using System.Reflection;

namespace KG.Mobile.Services
{
    public class SoundHelper
    {
        private readonly ISimpleAudioPlayer player;

        public SoundHelper()
        {
            player = CrossSimpleAudioPlayer.Current;
        }

        private void PlaySound(string fileName, int vibrationDurationMs)
        {
            // Load the audio from embedded resources
            var assembly = Assembly.GetExecutingAssembly();
            var resource = $"SBMOM.Mobile.Assets.Sounds.{fileName}"; // Make sure your MP3s are in this folder and marked as EmbeddedResource

            using Stream stream = assembly.GetManifestResourceStream(resource);
            if (stream != null)
            {
                player.Load(stream);
                player.Play();
            }

            // Vibrate the device
            Vibrate.VibrationDuration(vibrationDurationMs);
        }

        // Play the Error sound
        public void PlayError()
        {
            PlaySound("Error.mp3", 2000);
        }

        // Play the Success sound
        public void PlaySuccess()
        {
            PlaySound("Success.mp3", 500);
        }

        // Play the Test sound
        public void PlayTest()
        {
            PlaySound("Test.mp3", 4000);
        }
    }
}
