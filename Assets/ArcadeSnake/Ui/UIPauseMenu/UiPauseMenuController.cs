using System;
using UnityEngine;

namespace ArcadeSnake
{
    public class UiPauseMenuController
    {
        private IUIService _uiService;
        private UiPauseMenu _menu;
        private GameSettingsConfig _gameSettingsConfig;
        private readonly AudioPlayerController _audioPlayerController;
        private readonly SnakeMovement _snakeMovement;

        private bool _isAudioOn;
        private int _muteSettings;
        

        public UiPauseMenuController(
            IUIService uiService,
            GameSettingsConfig gameSettingsConfig,
            AudioPlayerController audioPlayerController,
            SnakeMovement snakeMovement
            )
        {
            _uiService = uiService;
            _gameSettingsConfig = gameSettingsConfig;
            _audioPlayerController = audioPlayerController;
            _snakeMovement = snakeMovement;

            _menu = _uiService.Get<UiPauseMenu>(); 
            _menu.OnUnPauseClickEvent += OnUnPauseClickHandler;
            _menu.OnRetryClickEvent += OnRetryClickHandler;
            _menu.OnMainMenuClickEvent += OnMainMenuClickHandler;
            _menu.OnAudioToggleValueChangedEvent += OnAudioToggleValueChangedHandler;

            _menu._toggleAudioMute.isOn = _gameSettingsConfig.IsAudioMute;
            _isAudioOn = _gameSettingsConfig.IsAudioMute;
            
            if (PlayerPrefs.HasKey("MuteSettings"))
            {
                _muteSettings = PlayerPrefs.GetInt("MuteSettings");
            }
            else
            {
                _muteSettings = 1;
            }
        }

        private void OnUnPauseClickHandler(object sender, EventArgs e)
        {
            _snakeMovement.GameIsStart = true;
            _snakeMovement.RetryMoving();

            
            _audioPlayerController.PlaySFX("ClickSound");

            _uiService.Hide<UiPauseMenu>();
            
            _audioPlayerController.UnpauseGamePlayMusic();
            _audioPlayerController.SetVolumeGameplayingMusic(0);
            _audioPlayerController.PauseMainMenuMusic();
        } 
        private void OnRetryClickHandler(object sender, EventArgs e)
        {
            _snakeMovement.GameIsStart = true;
            
            _audioPlayerController.PlaySFX("ClickSound");

            _uiService.Hide<UiPauseMenu>();
            
            _audioPlayerController.ResetGamePlayingMusic();
            _audioPlayerController.UnpauseGamePlayMusic();
            _audioPlayerController.PlayGameplayMusic();

            _snakeMovement.EndGame();
          
            _snakeMovement.StartGame();

        } 
        private void OnMainMenuClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");
           
           _uiService.Hide<UiPauseMenu>();
           _uiService.Hide<UiHud>();

           _uiService.Show<UiMainMenu>();

           _audioPlayerController.ResetGamePlayingMusic();
           _audioPlayerController.ResetMainMenuMusic();
           _audioPlayerController.PlayMainMenuMusic();
           
           _snakeMovement.EndGame();
        } 
        
        private void OnAudioToggleValueChangedHandler(object sender, bool e)
        {
            _uiService.Get<UiPauseMenu>()._toggleAudioMute.isOn = e;
            _audioPlayerController.MuteAllSound(e);
            _isAudioOn = e;
            _gameSettingsConfig.IsAudioMute = _isAudioOn;
            
            var temp =  PlayerPrefs.GetInt("MuteSettings");
            if (temp == 0)
            {
                PlayerPrefs.SetInt("MuteSettings", 1);
            }
            if (temp == 1)
            {
                PlayerPrefs.SetInt("MuteSettings", 0);
            }
        }
    }
}