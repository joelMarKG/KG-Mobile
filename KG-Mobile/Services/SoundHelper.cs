using Plugin.Maui.Audio;

namespace KG.Mobile.Services
{
    public class SoundHelper
    {
        private readonly IAudioManager _audioManager;

        private IAudioPlayer? _player;
        private Stream? _audioStream;
        private bool _isPlaying;

        public SoundHelper(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private async Task PlaySoundAsync(string fileName, int vibrationMs)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            try
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    Stop();

                    _audioStream = await FileSystem.OpenAppPackageFileAsync(fileName);
                    _player = _audioManager.CreatePlayer(_audioStream);

                    _player.Play();

                    if (Vibration.Default.IsSupported)
                    {
                        Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(vibrationMs));
                    }
                });
            }
            finally
            {
                _isPlaying = false;
            }
        }



        public void Stop()
        {
            _player?.Stop();
            _player?.Dispose();
            _audioStream?.Dispose();

            _player = null;
            _audioStream = null;
        }

        public Task PlayErrorAsync() => PlaySoundAsync("Error.mp3", 2000);
        public Task PlaySuccessAsync() => PlaySoundAsync("Success.mp3", 500);
        public Task PlayTestAsync() => PlaySoundAsync("Test.mp3", 4000);
    }
}
