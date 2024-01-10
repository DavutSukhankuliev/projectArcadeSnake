using UnityEngine.Audio;

namespace ArcadeSnake
{
    public class AudioPlayerController
    {
        private readonly AudioSourceController.Factory _soundFactory;
        private readonly AudioConfig _audioConfig;
        private readonly GameSettingsConfig _gameSettingsConfig;

        private AudioModel _audioModel;
        
        private AudioSourceController _sound;
        
        private AudioSourceController _soundMainMenu;
        
        private AudioSourceController _soundGamePlaying;

        public AudioPlayerController(
            AudioSourceController.Factory soundFactory,
            AudioConfig audioConfig,
            GameSettingsConfig gameSettingsConfig)
        {
            _soundFactory = soundFactory;
            _audioConfig = audioConfig;
            _gameSettingsConfig = gameSettingsConfig;
        }
        
        public void PlaySFX(string id)
        {
            _audioModel = _audioConfig.Get(id);
            _sound = _soundFactory.Create();
            _sound.Mute(_gameSettingsConfig.IsAudioMute);
            _sound.SetAudioSource(_audioModel);
        }

        public void MainMenuMusic(string id)
        {
            _audioModel = _audioConfig.Get(id);
            _soundMainMenu =_soundFactory.Create();
            _soundMainMenu.Mute(_gameSettingsConfig.IsAudioMute);
            _soundMainMenu.SetAudioSource(_audioModel);
        }
        public void PauseMainMenuMusic()
        {
            _soundMainMenu.PauseMusic();
        }
        
        public void UnpauseMainMenuMusic()
        {
            //_soundMainMenu.ResetMusic();
            _soundMainMenu.UnPauseMusic();
        }

        public void SetVolumeGameplayingMusic(float vol)
        {
            _soundGamePlaying.SetVolume(vol);
        }

        public void SetVolumeGamePlayingMusicToHalf()
        {
            _soundGamePlaying.SetVolumeToHalf();
        }

        public void GamePlayingMusic(string id)
        {
           _audioModel = _audioConfig.Get(id);
           _soundGamePlaying = _soundFactory.Create();
           _soundGamePlaying.Mute(_gameSettingsConfig.IsAudioMute);
           _soundGamePlaying.SetAudioSource(_audioModel);
        }

        public void PauseGamePlayingMusic()
        {
            _soundGamePlaying.PauseMusic();
        }

        public void ResetGamePlayingMusic()
        {
            _soundGamePlaying.ResetMusic();
            _soundGamePlaying.ResetMusicVolume();
            
        }
        public void ResetMainMenuMusic()
        {
            _soundMainMenu.ResetMusic();
            _soundMainMenu.ResetMusicVolume();
        }

        public void UnpauseGamePlayMusic()
        {
            _soundGamePlaying.UnPauseMusic();
        }
        
        public void SetVolumeMainMenuMusic(float vol)
        {
            _soundMainMenu.SetVolume(vol);
        }

        public void SetVolumeMainMenuMusicToHalf()
        {
            _soundMainMenu.SetVolumeToHalf();
        }

        public void PlayGameplayMusic()
        {
            _soundGamePlaying.PlayMusic();
        }
        
        public void PlayMainMenuMusic()
        {
            _soundMainMenu.PlayMusic();
        }

        public void MuteAllSound(bool e)
        {
            if (_sound!=null)
            {
                _sound.Mute(e);
            }

            if (_soundGamePlaying!=null)
            {
                _soundGamePlaying.Mute(e);
            }
            if (_soundMainMenu!=null)
            {
                _soundMainMenu.Mute(e);
            }
        }
    }
}
