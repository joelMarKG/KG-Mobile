using Plugin.Maui.Audio;

namespace KG.Mobile.Services
{
    public class SoundHelper
    {
        private readonly IAudioManager _audioManager;

        public SoundHelper(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private async Task PlaySoundAsync(string fileName, int vibrationMs)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            var player = _audioManager.CreatePlayer(stream);

            player.Play();

            if (Vibration.Default.IsSupported)
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(vibrationMs));
            }
        }

        public Task PlayErrorAsync() => PlaySoundAsync("error.mp3", 2000);
        public Task PlaySuccessAsync() => PlaySoundAsync("success.mp3", 500);
        public Task PlayTestAsync() => PlaySoundAsync("test.mp3", 4000);
    }
}
