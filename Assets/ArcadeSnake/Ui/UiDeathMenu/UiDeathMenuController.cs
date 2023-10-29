using System;
using UnityEngine;

namespace ArcadeSnake
{
    public class UiDeathMenuController
    {
        private readonly AudioPlayerController _audioPlayerController;
        private readonly SnakeMovement _snakeMovement;
        
        private IUIService _uiService;
        private UiDeathMenu _menu;

        public UiDeathMenuController(
            IUIService uiService,
            AudioPlayerController audioPlayerController,
            SnakeMovement snakeMovement
        )
        {
            _uiService = uiService;
            _audioPlayerController = audioPlayerController;
            _snakeMovement = snakeMovement;
        }
        
        public void InitDeathMenu()
        {
            _menu = _uiService.Get<UiDeathMenu>();
            _menu.OnMainMenuClickEvent += OnMainMenuClickHandler;
            _menu.OnRetryClickEvent += OnRetryClickHandler;
            _snakeMovement.OnDeathEvent += OnDeathHandler;
        }

        private void OnRetryClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");

            Time.timeScale = 1;
            
            _uiService.Hide<UiDeathMenu>();
            
            _audioPlayerController.ResetGamePlayingMusic();
            _audioPlayerController.PlayGameplayMusic();
            
            _snakeMovement.EndGame();
            
            _snakeMovement.StartGame();
        } 
        private void OnMainMenuClickHandler(object sender, EventArgs e)
        {
            _audioPlayerController.PlaySFX("ClickSound");
           
            _uiService.Hide<UiDeathMenu>();
            _uiService.Hide<UiHud>();
           
            _uiService.Show<UiMainMenu>();
            
            _audioPlayerController.ResetGamePlayingMusic();
            _audioPlayerController.ResetMainMenuMusic();
            _audioPlayerController.PlayMainMenuMusic();

            _snakeMovement.EndGame();
        }
        
        
        private void OnDeathHandler(object sender, EventArgs e)
        {
            ShowDeathMenu();
        }

        private void ShowDeathMenu()
        {
            _uiService.Show<UiDeathMenu>();
            _audioPlayerController.PauseGamePlayingMusic();
            Time.timeScale = 1;
        }
    }
}