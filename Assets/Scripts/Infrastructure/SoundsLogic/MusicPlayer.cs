using HDH.Audio;
using HDH.Audio.Confgis;
using Zenject;

namespace Infrastructure.SoundsLogic
{
    public class MusicPlayer
    {
        private readonly AudioService _audioService;

        [Inject]
        public MusicPlayer(AudioService audioService, AudioConfig musicConfig)
        {
            _audioService = audioService;
            audioService.MusicPlayer.Play(musicConfig);
        }

        public void Pause()
        {
            _audioService.SetPauseGroup(SoundGroupNames.MusicGroup, true);
        }

        public void Unpause()
        {
            _audioService.SetPauseGroup(SoundGroupNames.MusicGroup, false);
        }
    }
}