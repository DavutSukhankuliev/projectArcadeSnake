using System;
using UnityEngine;

namespace ArcadeSnake
{
    public class UiMainMenuController
    {
        private readonly SnakeMovement _snakeMovement;
        private readonly AudioPlayerController _audioPlayerController;
        
        private IUIService _uiService;
        private UiMainMenu _uiMainMenu;
        private GameSettingsConfig _gameSettingsConfig;

        private bool _isAudioOn;

        private int _muteSettings;

        public UiMainMenuController(
            IUIService uiService,
            SnakeMovement snakeMovement, 
            GameSettingsConfig gameSettingsConfig,
            AudioPlayerController audioPlayerController
            )
        {
            _uiService = uiService;
            _snakeMovement = snakeMovement;
            _gameSettingsConfig = gameSettingsConfig;
            _audioPlayerController = audioPlayerController;

            _uiMainMenu = _uiService.Get<UiMainMenu>();
            _uiMainMenu.OnPlayClickEvent += OnPlayClickHandler;
            _uiMainMenu.OnCalibrationClickEvent += OnCalibrationClickHandler;
            _uiMainMenu.OnAudioToggleValueChangedEvent += OnAudioToggleValueChangedHandler;
           
            _uiMainMenu._toggleAudioMute.isOn = _gameSettingsConfig.IsAudioMute;
            _isAudioOn = _gameSettingsConfig.IsAudioMute;
            
            _uiService.Show<UiMainMenu>();

            _audioPlayerController.MainMenuMusic("Main menu music");
            
            if (PlayerPrefs.HasKey("MuteSettings"))
            {
                _muteSettings = PlayerPrefs.GetInt("MuteSettings");
            }
            else
            {
                _muteSettings = 1;
            }
        }

        private void OnPlayClickHandler(object sender, EventArgs e)
        {
            //Time.timeScale = 1;
            _snakeMovement.GameIsStart = true;
            
            _audioPlayerController.PlaySFX("ClickSound");
            
            _uiService.Hide<UiMainMenu>();
            
            _uiService.Show<UiHud>();
            _audioPlayerController.GamePlayingMusic("GamePlayMusic");

            _audioPlayerController.PauseMainMenuMusic();

            _snakeMovement.StartGame();
            
        }
        private void OnCalibrationClickHandler(object sender, EventArgs e)
        {
            Time.timeScale = 1;
            _audioPlayerController.PlaySFX("ClickSound");
            _audioPlayerController.SetVolumeMainMenuMusicToHalf();

            _uiService.Hide<UiMainMenu>();
            
            _uiService.Show<UiCalibrationMenu>();
        }
        private void OnAudioToggleValueChangedHandler(object sender, bool e)
        {
            _uiService.Get<UiMainMenu>()._toggleAudioMute.isOn = e;
            _audioPlayerController.MuteAllSound(e);
            _isAudioOn = e;
            _gameSettingsConfig.IsAudioMute = _isAudioOn;
            
            if (!_isAudioOn)
            {
                PlayerPrefs.SetInt("MuteSettings", 1);
            }
            if (_isAudioOn)
            {
                PlayerPrefs.SetInt("MuteSettings", 0);
            }
        }
    }
}